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
        /// Returning received notification.
        /// </summary>
        public NotificationRequest Request { get; internal set; }
    }
}