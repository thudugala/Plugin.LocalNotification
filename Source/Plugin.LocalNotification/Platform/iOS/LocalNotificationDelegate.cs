using Foundation;
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

                var localNotification = GetRequest(response.Notification);

                // Take action based on identifier
                if (!response.IsDefaultAction)
                {
                    var actionIdentifier = response.ActionIdentifier;
                    var userInfo = response.Notification.Request.Content.UserInfo;

                    NotificationCenter.Current.NotificationActions[actionIdentifier].Handler?.Invoke(localNotification.NotificationId, localNotification.ReturningData);

                    return;
                }

                
                
                var args = new NotificationTappedEventArgs
                {
                    Request = localNotification
                };

                UIApplication.SharedApplication.InvokeOnMainThread(() =>
                {
                    if (localNotification != null && response.Notification.Request.Content.Badge != null)
                    {
                        var appBadges = UIApplication.SharedApplication.ApplicationIconBadgeNumber -
                                        Convert.ToInt32(response.Notification.Request.Content.Badge.ToString(), CultureInfo.CurrentCulture);
                        UIApplication.SharedApplication.ApplicationIconBadgeNumber = appBadges;
                    }

                    var notificationService = TryGetDefaultDroidNotificationService();
                    notificationService.OnNotificationTapped(args);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private NotificationRequest GetRequest(UNNotification notification)
        {
            var notificationContent = notification?.Request.Content;
            if (notificationContent == null)
            {
                return null;
            }
            var dictionary = notificationContent.UserInfo;

            if (!dictionary.ContainsKey(new NSString(NotificationCenter.ReturnRequest)))
            {
                return null;
            }
            var requestSerialize = dictionary[NotificationCenter.ReturnRequest].ToString();
            var requestSerializeSchedule = string.Empty;
            var requestSerializeAndroid = string.Empty;
            var requestSerializeIos = string.Empty;
            if (dictionary.ContainsKey(new NSString(NotificationCenter.ReturnRequestSchedule)))
            {
                requestSerializeSchedule = dictionary[NotificationCenter.ReturnRequestSchedule].ToString();
            }
            if (dictionary.ContainsKey(new NSString(NotificationCenter.ReturnRequestAndroid)))
            {
                requestSerializeAndroid = dictionary[NotificationCenter.ReturnRequestAndroid].ToString();
            }
            if (dictionary.ContainsKey(new NSString(NotificationCenter.ReturnRequestIos)))
            {
                requestSerializeIos = dictionary[NotificationCenter.ReturnRequestIos].ToString();
            }
            var request = NotificationCenter.GetRequest(requestSerialize, requestSerializeSchedule, requestSerializeAndroid, requestSerializeIos);

            return request;
        }

        /// <inheritdoc />
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            try
            {
                var presentationOptions = UNNotificationPresentationOptions.Alert;
                var localNotification = GetRequest(notification);

                if (localNotification != null)
                {
                    if (localNotification.Schedule.NotifyAutoCancelTime.HasValue && localNotification.Schedule.NotifyAutoCancelTime <= DateTime.Now)
                    {
                        var notificationService = TryGetDefaultDroidNotificationService();
                        notificationService.Cancel(localNotification.NotificationId);
                        Debug.WriteLine("Notification Auto Canceled");
                        return;
                    }

                    var args = new NotificationReceivedEventArgs
                    {
                        Request = localNotification
                    };
                    NotificationCenter.Current.OnNotificationReceived(args);

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
                Debug.WriteLine(ex);
            }
        }

        private static NotificationServiceImpl TryGetDefaultDroidNotificationService()
        {
            if (NotificationCenter.Current is NotificationServiceImpl notificationService)
            {
                return notificationService;
            }
            return new NotificationServiceImpl();
        }
    }
}