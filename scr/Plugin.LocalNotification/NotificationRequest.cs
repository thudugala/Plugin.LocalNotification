using System;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// Notification Request
    /// </summary>
    public class NotificationRequest
    {
        /// <summary>
        /// Android specific properties.
        /// </summary>
        public AndroidOptions Android { get; set; } = new AndroidOptions();

        /// <summary>
        /// Number of the badge displays on the Home Screen.
        /// </summary>
        public int BadgeNumber { get; set; }

        /// <summary>
        /// Details for the notification.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A unique identifier for the request
        /// (if identifier is not unique, a new notification request object is not created).
        /// You can use this identifier later to cancel a request that is still pending.
        /// </summary>
        public int NotificationId { get; set; }

        /// <summary>
        /// Time to show the notification.
        /// </summary>
        public DateTime? NotifyTime { get; set; }

        /// <summary>
        /// Returning data when tapped on notification.
        /// </summary>
        public string ReturningData { get; set; }

        /// <summary>
        /// Sound file name for the notification.
        /// In Android, Only used if Android Api below 26.
        /// Use NotificationCenter.CreateNotificationChannel when Android Api equal or above 26
        /// </summary>
        public string Sound { get; set; }

        /// <summary>
        /// Title for the notification.
        /// </summary>
        public string Title { get; set; }
    }
}