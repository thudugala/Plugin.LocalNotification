using System;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// Returning event when tapped on notification action.
    /// </summary>
    /// <param name="e"></param>
    public delegate void NotificationActionTappedEventHandler(NotificationActionEventArgs e);

    /// <summary>
    /// Returning error
    /// </summary>
    /// <param name="e"></param>
    public delegate void NotificationLogHandler(NotificationLogArgs e);

    /// <summary>
    /// Returning event when a notification is received.
    /// On iOS this event is fired only when the app is in foreground
    /// </summary>
    /// <param name="e"></param>

    public delegate void NotificationReceivedEventHandler(NotificationEventArgs e);

    /// <summary>
    /// Returning event when tapped on notification.
    /// </summary>
    /// <param name="e"></param>
    public delegate void NotificationTappedEventHandler(NotificationEventArgs e);

    /// <summary>
    ///
    /// </summary>
    public class NotificationActionEventArgs : EventArgs
    {
        /// <summary>
        /// Tapped Action Id
        /// </summary>
        public int ActionId { get; set; }

        /// <summary>
        /// Returning notification.
        /// </summary>
        public NotificationRequest Request { get; internal set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class NotificationLogArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public string Message { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public Exception Error { get; internal set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class NotificationEventArgs : EventArgs
    {
        /// <summary>
        /// Returning notification.
        /// </summary>
        public NotificationRequest Request { get; internal set; }
    }
}