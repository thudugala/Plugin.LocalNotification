namespace Plugin.LocalNotification
{
    /// <summary>
    /// NotificationRequest for iOS
    /// </summary>
    public class iOSOptions
    {
        /// <summary>
        /// Setting this flag will prevent iOS from displaying the default banner when a Notification is received in foreground
        /// Default is false
        /// </summary>
        public bool HideForegroundAlert { get; set; }
    }
}
