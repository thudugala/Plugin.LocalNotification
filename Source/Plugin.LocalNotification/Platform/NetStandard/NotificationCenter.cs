using System;

namespace Plugin.LocalNotification
{
    public static partial class NotificationCenter
    {
        private static void PlatformCancel(int notificationId)
        {
            throw new NotImplementedException();
        }

        private static void PlatformCancelAll()
        {
            throw new NotImplementedException();
        }

        private static void OnPlatformNotificationTapped(NotificationTappedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void OnPlatformNotificationReceived(NotificationReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void PlatformShow(NotificationRequest notificationRequest)
        {
            throw new NotImplementedException();
        }
    }
}