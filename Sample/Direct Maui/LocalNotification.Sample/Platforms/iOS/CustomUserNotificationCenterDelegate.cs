using Plugin.LocalNotification;
using Plugin.LocalNotification.Platforms;
using UserNotifications;

namespace LocalNotification.Sample;

public class CustomUserNotificationCenterDelegate : UserNotificationCenterDelegate
{
    public override void DidReceiveNotificationResponse(UNUserNotificationCenter center,
        UNNotificationResponse response,
        Action completionHandler)
    {
        // if the Notification is type Plugin.LocalNotification.NotificationRequest
        // call the base method, else handel it by your self.

        var notificationRequest = LocalNotificationCenter.GetRequest(response.Notification.Request.Content);
        if (notificationRequest != null)
        {
            base.DidReceiveNotificationResponse(center, response, completionHandler);
        }
    }

    public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification,
        Action<UNNotificationPresentationOptions> completionHandler)
    {
        // if the Notification is type Plugin.LocalNotification.NotificationRequest
        // call the base method, else handel it by your self.

        if (notification is null)
        {
            return;
        }

        var notificationRequest = LocalNotificationCenter.GetRequest(notification?.Request.Content);

        if (notificationRequest != null)
        {
            base.WillPresentNotification(center, notification, completionHandler);
        }
    }
}
