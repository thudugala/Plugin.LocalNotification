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

                // Take action based on identifier
                if (!response.IsDefaultAction)
                {
                    return;
                }

                var dictionary = response.Notification.Request.Content.UserInfo;

                if (!dictionary.ContainsKey(NotificationCenter.ExtraReturnDataIos))
                {
                    return;
                }

                var subscribeItem = new NotificationTappedEventArgs
                {
                    Data = dictionary[NotificationCenter.ExtraReturnDataIos].ToString()
                };

                UIApplication.SharedApplication.InvokeOnMainThread(() =>
                {
                    var appBadges = UIApplication.SharedApplication.ApplicationIconBadgeNumber -
                                    Convert.ToInt32(response.Notification.Request.Content.Badge.ToString(), CultureInfo.CurrentCulture);
                    UIApplication.SharedApplication.ApplicationIconBadgeNumber = appBadges;

                    NotificationCenter.OnNotificationTapped(subscribeItem);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <inheritdoc />
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            try
            {
                var presentationOptions = UNNotificationPresentationOptions.Alert;
                var notificationContent = notification?.Request.Content;

                if (notificationContent != null)
                {
                    var dictionary = notificationContent.UserInfo;
                    var args = new NotificationReceivedEventArgs
                    {
                        Title = notificationContent.Title,
                        Description = notificationContent.Body,
                        Data = dictionary.ContainsKey(NotificationCenter.ExtraReturnDataIos)
                                        ? dictionary[NotificationCenter.ExtraReturnDataIos].ToString()
                                        : ""
                    };

                    NotificationCenter.OnNotificationReceived(args);

                    if (dictionary.ContainsKey(NotificationCenter.ExtraNotificationReceivedIos))
                    {
                        var customOptions = dictionary[NotificationCenter.ExtraNotificationReceivedIos].ToString()
                            .ToUpperInvariant();
                        if (customOptions == "TRUE")
                        {
                            presentationOptions = UNNotificationPresentationOptions.None;
                        }
                    }
                    
                    if (dictionary.ContainsKey(NotificationCenter.ExtraSoundInForegroundIos))
                    {
                        var customOptions = dictionary[NotificationCenter.ExtraSoundInForegroundIos].ToString()
                                                                                                    .ToUpperInvariant();
                        if (customOptions == "TRUE")
                        {
                            presentationOptions = presentationOptions == UNNotificationPresentationOptions.Alert ? 
                                UNNotificationPresentationOptions.Sound|UNNotificationPresentationOptions.Alert :
                                UNNotificationPresentationOptions.Sound;
                        }
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
    }
}