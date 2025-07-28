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
        // If the notification is typed Plugin.LocalNotification.NotificationRequest
        // Call the base method else handle it by yourself.

        var notificationRequest = LocalNotificationCenter.GetRequest(response.Notification.Request.Content);
        if (notificationRequest is not null)
        {
            base.DidReceiveNotificationResponse(center, response, completionHandler);
        }
        else
        {
            // Write your code here
        }
    }

    public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification,
        Action<UNNotificationPresentationOptions> completionHandler)
    {
        // If the notification is typed Plugin.LocalNotification.NotificationRequest
        // Call the base method else handle it by yourself.

        if (notification is null)
        {
            return;
        }

        var notificationRequest = LocalNotificationCenter.GetRequest(notification.Request.Content);

        if (notificationRequest is not null)
        {
            base.WillPresentNotification(center, notification, completionHandler);
        }
        else
        {
            // Write your code here
        }
    }
}
