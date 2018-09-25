using System.Collections.Generic;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// Returning event when tapped on notification.
    /// </summary>
    public class LocalNotificationTappedEvent
    {
        /// <summary>
        /// Returning data when tapped on notification.
        /// </summary>
        public IList<string> Data { get; set; }
    }
}