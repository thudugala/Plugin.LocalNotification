using System;
using System.Diagnostics;
using System.Globalization;
using UIKit;
using UserNotifications;

namespace Plugin.LocalNotification.Platform.iOS
{
    /// <inheritdoc />
    public class LocalNotificationDelegate : UNUserNotificationCenterDelegate
    {
        /// <inheritdoc />
        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center,
            UNNotificationResponse response, Action completionHandler)
        {
            try
            {
                if (response is null)
                {
                    return;
                }

                var notificationService = TryGetDefaultIOsNotificationService();
                var request = notificationService.GetRequest(response?.Notification?.Request?.Content);

                UIApplication.SharedApplication.InvokeOnMainThread(() =>
                {
                    // Take action based on identifier
                    if (!response.IsDefaultAction)
                    {
                        if (string.IsNullOrWhiteSpace(response.ActionIdentifier) == false &&
                            int.TryParse(response.ActionIdentifier, out var actionId))
                        {
                            var actionArgs = new NotificationActionEventArgs
                            {
                                ActionId = actionId,
                                Request = request
                            };
                            notificationService.OnNotificationActionTapped(actionArgs);
                            return;
                        }
                    }

                    if (request != null && response.Notification.Request.Content.Badge != null)
                    {
                        var appBadges = UIApplication.SharedApplication.ApplicationIconBadgeNumber -
                                        Convert.ToInt32(response.Notification.Request.Content.Badge.ToString(), CultureInfo.CurrentCulture);
                        UIApplication.SharedApplication.ApplicationIconBadgeNumber = appBadges;
                    }

                    var args = new NotificationEventArgs
                    {
                        Request = request
                    };

                    notificationService.OnNotificationTapped(args);
                });
            }
            catch (Exception ex)
            {
                NotificationCenter.Log(ex);
            }
        }

        /// <inheritdoc />
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            try
            {
                var presentationOptions = UNNotificationPresentationOptions.Alert;
                var notificationService = TryGetDefaultIOsNotificationService();
                var request = notificationService.GetRequest(notification?.Request?.Content);
                if (request != null)
                {
                    if (request.Schedule.NotifyAutoCancelTime.HasValue && request.Schedule.NotifyAutoCancelTime <= DateTime.Now)
                    {
                        notificationService.Cancel(request.NotificationId);
                        Debug.WriteLine("Notification Auto Canceled");
                        return;
                    }

                    var args = new NotificationEventArgs
                    {
                        Request = request
                    };

                    notificationService.OnNotificationReceived(args);

                    if (request.iOS.HideForegroundAlert)
                    {
                        presentationOptions = UNNotificationPresentationOptions.None;
                    }

                    if (request.iOS.PlayForegroundSound)
                    {
                        presentationOptions = presentationOptions == UNNotificationPresentationOptions.Alert ?
                            UNNotificationPresentationOptions.Sound | UNNotificationPresentationOptions.Alert :
                            UNNotificationPresentationOptions.Sound;
                    }
                }

                if (completionHandler is null)
                {
                    return;
                }

                completionHandler(presentationOptions);
            }
            catch (Exception ex)
            {
                NotificationCenter.Log(ex);
            }
        }

        private static NotificationServiceImpl TryGetDefaultIOsNotificationService()
        {
            return NotificationCenter.Current is NotificationServiceImpl notificationService
                ? notificationService
                : new NotificationServiceImpl();
        }
    }
}