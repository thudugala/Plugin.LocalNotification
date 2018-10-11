using System;
using System.Collections.Generic;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// LocalNotification
    /// </summary>
    public class LocalNotification
    {
        /// <summary>
        ///
        /// </summary>
        public LocalNotification()
        {
            
        }

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
        /// Title for the notification.
        /// </summary>
        public string Title { get; set; }
    }
}