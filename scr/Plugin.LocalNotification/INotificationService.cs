namespace Plugin.LocalNotification
{
    /// <summary>
    /// Used to display platform specific local notifications.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Cancel a notification match with the Id
        /// </summary>
        /// <param name="notificationId">A unique identifier for the already displaying local notification.</param>
        void Cancel(int notificationId);

        /// <summary>
        /// Cancel all notification.
        /// </summary>
        void CancelAll();

        /// <summary>
        /// Send a local notification to the device.
        /// </summary>
        /// <param name="notificationRequest"></param>
        void Show(NotificationRequest notificationRequest);

        /// <summary>
        /// fires when notification popup is tapped.
        /// </summary>
        event NotificationTappedEventHandler NotificationTapped;

        /// <summary>
        /// Internal use Only
        /// </summary>
        /// <param name="e"></param>
        void OnNotificationTapped(NotificationTappedEventArgs e);
    }
}