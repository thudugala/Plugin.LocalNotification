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
                var localNotification = notificationService.GetRequest(response?.Notification?.Request?.Content);

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
                                Request = localNotification
                            };
                            notificationService.OnNotificationActionTapped(actionArgs);
                            return;
                        }
                    }

                    if (localNotification != null && response.Notification.Request.Content.Badge != null)
                    {
                        var appBadges = UIApplication.SharedApplication.ApplicationIconBadgeNumber -
                                        Convert.ToInt32(response.Notification.Request.Content.Badge.ToString(), CultureInfo.CurrentCulture);
                        UIApplication.SharedApplication.ApplicationIconBadgeNumber = appBadges;
                    }
                    var args = new NotificationEventArgs
                    {
                        Request = localNotification
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
                var localNotification = notificationService.GetRequest(notification?.Request?.Content);
                if (localNotification != null)
                {
                    if (localNotification.Schedule.NotifyAutoCancelTime.HasValue && localNotification.Schedule.NotifyAutoCancelTime <= DateTime.Now)
                    {
                        notificationService.Cancel(localNotification.NotificationId);
                        Debug.WriteLine("Notification Auto Canceled");
                        return;
                    }

                    var args = new NotificationEventArgs
                    {
                        Request = localNotification
                    };
                    notificationService.OnNotificationReceived(args);

                    if (localNotification.iOS.HideForegroundAlert)
                    {
                        presentationOptions = UNNotificationPresentationOptions.None;
                    }

                    if (localNotification.iOS.PlayForegroundSound)
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