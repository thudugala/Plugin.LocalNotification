using System;

namespace Plugin.LocalNotification
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="e"></param>
    public delegate void NotificationTappedEventHandler(NotificationTappedEventArgs e);

    /// <summary>
    /// Returning event when tapped on notification.
    /// </summary>
    public class NotificationTappedEventArgs : EventArgs
    {
        /// <summary>
        /// Returning data when a notification is received.
        /// </summary>
        public string Data { get; internal set; }

        /// <summary>
        /// Returning details when a notification is received.
        /// </summary>
        public string Description { get; internal set; } = string.Empty;

        /// <summary>
        /// Returning title when a notification is received.
        /// </summary>
        public string Title { get; internal set; }
    }
}