namespace Plugin.LocalNotification.EventArgs
{
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
    public class NotificationEventArgs : System.EventArgs
    {
        /// <summary>
        /// Returning notification.
        /// </summary>
        public NotificationRequest Request { get; set; }
    }
}