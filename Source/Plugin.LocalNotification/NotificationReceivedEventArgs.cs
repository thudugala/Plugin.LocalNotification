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
        /// Returning received notification.
        /// </summary>
        public NotificationRequest Request { get; internal set; }
    }
}