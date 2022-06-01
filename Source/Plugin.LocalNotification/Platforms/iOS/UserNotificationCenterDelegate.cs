using Foundation;
using Plugin.LocalNotification.EventArgs;
using System;
using System.Globalization;
using UIKit;
using UserNotifications;

namespace Plugin.LocalNotification.Platforms
{
    /// <inheritdoc />
    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
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
                var notificationRequest = notificationService.GetRequest(response.Notification.Request.Content);

                // if notificationRequest is null this maybe not a notification from this plugin.
                if (notificationRequest is null)
                {
                    completionHandler?.Invoke();

                    LocalNotificationCenter.Log("Notification request not found");
                    return;
                }

                if (response.Notification.Request.Content.Badge != null)
                {
                    UIApplication.SharedApplication.InvokeOnMainThread(() =>
                    {
                        var appBadges = UIApplication.SharedApplication.ApplicationIconBadgeNumber -
                                        Convert.ToInt32(response.Notification.Request.Content.Badge.ToString(),
                                            CultureInfo.CurrentCulture);
                        UIApplication.SharedApplication.ApplicationIconBadgeNumber = appBadges;
                    });
                }

                // Take action based on identifier
                if (!response.IsDefaultAction)
                {
                    if (string.IsNullOrWhiteSpace(response.ActionIdentifier) == false &&
                        int.TryParse(response.ActionIdentifier, out var actionId))
                    {
                        var actionArgs = new NotificationActionEventArgs
                        {
                            ActionId = actionId,
                            Request = notificationRequest
                        };
                        notificationService.OnNotificationActionTapped(actionArgs);

                        completionHandler?.Invoke();
                        return;
                    }
                }

                if (response.IsDismissAction)
                {
                    var actionArgs = new NotificationActionEventArgs
                    {
                        ActionId = NotificationActionEventArgs.DismissedActionId,
                        Request = notificationRequest
                    };
                    notificationService.OnNotificationActionTapped(actionArgs);

                    completionHandler?.Invoke();
                    return;
                }

                var args = new NotificationEventArgs
                {
                    Request = notificationRequest
                };
                notificationService.OnNotificationTapped(args);

                completionHandler?.Invoke();
            }
            catch (Exception ex)
            {
                LocalNotificationCenter.Log(ex);
            }
        }

        /// <inheritdoc />
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification,
            Action<UNNotificationPresentationOptions> completionHandler)
        {
            try
            {
                var presentationOptions = UNNotificationPresentationOptions.None;

                var notificationService = TryGetDefaultIOsNotificationService();
                var notificationRequest = notificationService.GetRequest(notification?.Request.Content);

                // if notificationRequest is null this maybe not a notification from this plugin.
                if (notificationRequest is null)
                {
                    completionHandler?.Invoke(presentationOptions);

                    LocalNotificationCenter.Log("Notification request not found");
                    return;
                }

                if (notificationRequest.Schedule.NotifyAutoCancelTime.HasValue &&
                    notificationRequest.Schedule.NotifyAutoCancelTime <= DateTime.Now)
                {
                    notificationService.Cancel(notificationRequest.NotificationId);

                    completionHandler?.Invoke(presentationOptions);

                    LocalNotificationCenter.Log("Notification Auto Canceled");
                    return;
                }

                var requestHandled = false;
                var dictionary = notification?.Request.Content.UserInfo;
                if (dictionary != null)
                {
                    if (dictionary.ContainsKey(new NSString(LocalNotificationCenter.ReturnRequestHandled)))
                    {
                        var handled = bool.Parse(dictionary[LocalNotificationCenter.ReturnRequestHandled].ToString());
                        if (handled)
                        {
                            LocalNotificationCenter.Log("Notification handled");
                            requestHandled = true;
                        }
                    }
                }

                if (requestHandled == false)
                {
                    if (
#if XAMARINIOS
                        UIDevice.CurrentDevice.CheckSystemVersion(14, 0)
#elif IOS
                        OperatingSystem.IsIOSVersionAtLeast(14)
#endif
                        )
                    {
                        if (notificationRequest.iOS.PresentAsBanner)
                        {
                            presentationOptions |= UNNotificationPresentationOptions.Banner;
                        }

                        if (notificationRequest.iOS.ShowInNotificationCenter)
                        {
                            presentationOptions |= UNNotificationPresentationOptions.List;
                        }
                    }
                    else
                    {
                        presentationOptions |= UNNotificationPresentationOptions.Alert;
                    }

                    if (notificationRequest.iOS.ApplyBadgeValue)
                    {
                        presentationOptions |= UNNotificationPresentationOptions.Badge;
                    }
                    if (notificationRequest.iOS.PlayForegroundSound)
                    {
                        presentationOptions |= UNNotificationPresentationOptions.Sound;
                    }

                    if (notificationRequest.iOS.HideForegroundAlert)
                    {
                        presentationOptions = UNNotificationPresentationOptions.None;
                    }
                }

                var args = new NotificationEventArgs
                {
                    Request = notificationRequest
                };
                notificationService.OnNotificationReceived(args);

                completionHandler?.Invoke(presentationOptions);
            }
            catch (Exception ex)
            {
                LocalNotificationCenter.Log(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static NotificationServiceImpl TryGetDefaultIOsNotificationService()
        {
            return LocalNotificationCenter.Current is NotificationServiceImpl notificationService
                ? notificationService
                : new NotificationServiceImpl();
        }
    }
}