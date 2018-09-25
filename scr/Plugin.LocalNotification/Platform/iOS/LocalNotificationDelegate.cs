using System;
using System.Linq;
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
            // Take action based on identifier
            if (!response.IsDefaultAction)
            {
                return;
            }

            var identifier = response.Notification.Request.Content.UserInfo.Values.Select(v => v.ToString()).ToList();
            var subscribeItem = new LocalNotificationTappedEvent
            {
                Data = identifier
            };
            MessagingCenter.Send(subscribeItem, typeof(LocalNotificationTappedEvent).FullName);
        }

        /// <inheritdoc />
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            completionHandler(UNNotificationPresentationOptions.Alert);
        }
    }
}