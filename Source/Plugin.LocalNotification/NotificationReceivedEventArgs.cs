using System;

namespace Plugin.LocalNotification
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="e"></param>

    public delegate void NotificationReceivedEventHandler(NotificationReceivedEventArgs e);

    /// <summary>
    /// Returning event when a notification is received.
    /// On iOS this event is fired only when the app is in foreground
    /// </summary>
    public class NotificationReceivedEventArgs : EventArgs
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