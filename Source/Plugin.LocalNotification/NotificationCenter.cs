namespace Plugin.LocalNotification
{
    public static partial class NotificationCenter
    {
        /// <summary>
        /// fires when notification popup is tapped.
        /// </summary>
        public static event NotificationTappedEventHandler NotificationTapped;

        /// <summary>
        /// fires when notification is received.
        /// On iOS this event is fired only when the app is in foreground
        /// </summary>
        public static event NotificationReceivedEventHandler NotificationReceived;

        /// <summary>
        /// Cancel a notification match with the Id
        /// </summary>
        /// <param name="notificationId">A unique identifier for the already displaying local notification.</param>
        public static void Cancel(int notificationId)
        {
            PlatformCancel(notificationId);
        }

        /// <summary>
        /// Cancel all notification.
        /// </summary>
        public static void CancelAll()
        {
            PlatformCancelAll();
        }

        /// <summary>
        /// Internal use Only
        /// </summary>
        /// <param name="e"></param>
        public static void OnNotificationTapped(NotificationTappedEventArgs e)
        {
            OnPlatformNotificationTapped(e);
        }

        /// <summary>
        /// Internal use Only
        /// </summary>
        /// <param name="e"></param>
        public static void OnNotificationReceived(NotificationReceivedEventArgs e)
        {
            OnPlatformNotificationReceived(e);
        }

        /// <summary>
        /// Send a local notification to the device.
        /// </summary>
        /// <param name="notificationRequest"></param>
        public static void Show(NotificationRequest notificationRequest)
        {
            PlatformShow(notificationRequest);
        }
    }
}