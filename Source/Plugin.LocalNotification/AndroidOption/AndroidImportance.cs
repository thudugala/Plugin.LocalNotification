namespace Plugin.LocalNotification.AndroidOption
{
    /// <summary>
    /// 
    /// </summary>
    public enum AndroidImportance
    {
        /// <summary>
        /// Value signifying that the user has not expressed an importance.
        /// This value is for persisting preferences, and should never be associated with an actual notification.
        /// </summary>
        Unspecified = -1000,
        /// <summary>
        /// Does not show in the shade
        /// </summary>
        None = 0,
        /// <summary>
        /// Only shows in the shade, below the fold. 
        /// This should not be used with Service.startForeground since a foreground service is supposed to be something the user cares about so it does not make semantic sense to mark its notification as minimum importance.
        /// If you do this as of Android version Build.VERSION_CODES.O, 
        /// the system will show a higher-priority notification about your app running in the background.
        /// </summary>
        Min = 1,
        /// <summary>
        /// Shows in the shade, and potentially in the status bar, but is not audibly intrusive.
        /// </summary>
        Low = 2,
        /// <summary>
        /// Shows everywhere, makes noise, but does not visually intrude
        /// </summary>
        Default = 3,
        /// <summary>
        /// Shows everywhere, makes noise and peeks. May use full screen intents
        /// </summary>
        High = 4,
        /// <summary>
        /// Unused
        /// </summary>
        Max = 5
    }
}
