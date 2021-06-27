namespace Plugin.LocalNotification
{
    /// <summary>
    ///
    /// </summary>
    public class NotificationAction
    {
        /// <summary>
        /// A unique identifier for the Action
        /// </summary>
        public string ActionId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///
        /// </summary>
        public iOSActionType iOSAction { get; set; } = iOSActionType.None;
    }
}