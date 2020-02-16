using System;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// NotificationRequest for Android
    /// </summary>
    public class AndroidOptions
    {
        /// <summary>
        /// Setting this flag will make it so the notification is automatically canceled when the user clicks it in the panel.
        /// Default is true
        /// </summary>
        public bool AutoCancel { get; set; } = true;

        /// <summary>
        /// Sets or gets, The id of the channel. Must be unique per package. The value may be truncated if it is too lon
        /// Use this to target the Notification Channel.
        /// </summary>
        public string ChannelId { get; set; } = "Plugin.LocalNotification.GENERAL";

        /// <summary>
        /// If set, the notification icon and application name will have the provided ARGB color.
        /// </summary>
        public int? Color { get; set; }

        /// <summary>
        /// if Set, find the icon by name from drawable and set it has the Small Icon to use in the notification layouts.
        /// if not set, application Icon will we used.
        /// </summary>
        public string IconName { get; set; } = string.Empty;

        /// <summary>
        /// If set, the LED will have the provided ARGB color.
        /// </summary>
        public int? LedColor { get; set; }

        /// <summary>
        /// Set whether this is an ongoing notification.
        /// Ongoing notifications differ from regular notifications in the following ways,
        /// Ongoing notifications are sorted above the regular notifications in the notification panel.
        /// Ongoing notifications do not have an 'X' close button, and are not affected by the "Clear all" button.
        /// Default is false
        /// </summary>
        public bool Ongoing { get; set; } = false;

        /// <summary>
        /// Set the relative priority for this notification.
        /// In Android, Only used if Android Api below 26.
        /// Use NotificationCenter.CreateNotificationChannel when Android Api equal or above 26
        /// </summary>
        public NotificationPriority Priority { get; set; } = NotificationPriority.Default;

        /// <summary>
        /// Set whether this progress bar is in indeterminate mode
        /// </summary>
        public bool? ProgressBarIndeterminate { get; set; }

        /// <summary>
        /// Set Upper limit of this progress bar's range
        /// </summary>
        public int? ProgressBarMax { get; set; }

        /// <summary>
        /// Set progress bar's current level of progress
        /// </summary>
        public int? ProgressBarProgress { get; set; }

        /// <summary>
        /// Specifies the time at which this notification should be canceled, if it is not already canceled.
        /// </summary>
        public TimeSpan? TimeoutAfter { get; set; }

        /// <summary>
        /// Vibrate with a given pattern.
        /// Pass in an array of ints that are the durations for which to turn on or off the vibrator in milliseconds.
        /// The first value indicates the number of milliseconds to wait before turning the vibrator on.
        /// The next value indicates the number of milliseconds for which to keep the vibrator on before turning it off.
        /// Subsequent values alternate between durations in milliseconds to turn the vibrator off or to turn the vibrator on.
        /// </summary>
#pragma warning disable CA1819 // Properties should not return arrays
        public long[]? VibrationPattern { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays
    }
}