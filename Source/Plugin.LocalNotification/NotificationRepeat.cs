namespace Plugin.LocalNotification
{
    /// <summary>
    /// Set id Notification should repeat
    /// </summary>
    public enum NotificationRepeat
    {
        /// <summary>
        /// Notification should not repeat
        /// </summary>
        No,

        /// <summary>
        /// Notification should repeat next day at same time
        /// </summary>
        Daily,

        /// <summary>
        /// Notification should repeat next week at same day, same time
        /// </summary>
        Weekly,

        /// <summary>
        /// Notification to be delivered after the specified amount of time elapses
        /// </summary>
        TimeInterval
    }
}