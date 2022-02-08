namespace Plugin.LocalNotification.EventArgs
{
    /// <summary>
    /// Returning event when tapped on notification action.
    /// </summary>
    /// <param name="e"></param>
    public delegate void NotificationActionTappedEventHandler(NotificationActionEventArgs e);

    /// <summary>
    ///
    /// </summary>
    public class NotificationActionEventArgs : NotificationEventArgs
    {
        /// <summary>
        /// Tapped Action Id
        /// </summary>
        public int ActionId { get; set; }
    }
}