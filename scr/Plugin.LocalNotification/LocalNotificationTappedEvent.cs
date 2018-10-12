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
        public string Data { get; internal set; }
    }
}