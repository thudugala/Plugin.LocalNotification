using System;
using System.Diagnostics;
using UserNotifications;
using Xamarin.Forms;

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
                // Take action based on identifier
                if (!response.IsDefaultAction)
                {
                    return;
                }

                var dictionary = response.Notification.Request.Content.UserInfo;

                if (dictionary.ContainsKey(LocalNotificationService.ExtraReturnData))
                {
                    var subscribeItem = new LocalNotificationTappedEvent
                    {
                        Data = dictionary[LocalNotificationService.ExtraReturnData].ToString()
                    };
                    MessagingCenter.Send(subscribeItem, typeof(LocalNotificationTappedEvent).FullName);
                }
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
                completionHandler(UNNotificationPresentationOptions.Alert);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}