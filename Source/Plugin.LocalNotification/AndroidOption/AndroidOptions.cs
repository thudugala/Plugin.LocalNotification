using System;

namespace Plugin.LocalNotification.AndroidOption
{
    /// <summary>
    /// NotificationRequest for Android
    /// </summary>
    public class AndroidOptions
    {
        /// <summary>
        /// Default Channel Id.
        /// </summary>
        public static string DefaultChannelId => "Plugin.LocalNotification.GENERAL";

        /// <summary>
        /// Default Channel Name
        /// </summary>
        public static string DefaultChannelName => "General";

        /// <summary>
        /// Default Group Id
        /// </summary>
        public static string DefaultGroupId => "Plugin.LocalNotification.Group";

        /// <summary>
        /// Default Group Name
        /// </summary>
        public static string DefaultGroupName => "GeneralGroup";

        /// <summary>
        /// Setting this flag will make it so the notification is automatically canceled when the user clicks it in the panel.
        /// Default is true
        /// </summary>
        public bool AutoCancel { get; set; } = true;

        /// <summary>
        /// Sets or gets, The id of the channel. Must be unique per package. The value may be truncated if it is too lon
        /// Use this to target the Notification Channel.
        /// </summary>
        public string ChannelId { get; set; } = DefaultChannelId;

        /// <summary>
        /// If set, the notification icon and application name will have the provided ARGB color.
        /// </summary>
        public AndroidColor Color { get; set; } = new();

        /// <summary>
        /// if Set, find the icon by name from drawable and set it has the Large Icon to use in the notification layouts.
        /// if not set, application Icon will we used.
        /// </summary>
        public AndroidIcon IconLargeName { get; set; } = new();

        /// <summary>
        /// if Set, find the icon by name from drawable and set it has the Small Icon to use in the notification layouts.
        /// if not set, application Icon will we used.
        /// </summary>
        public AndroidIcon IconSmallName { get; set; } = new();

        /// <summary>
        /// Set this notification to be the group summary for a group of notifications.
        /// Grouped notifications may display in a cluster or stack on devices which support such rendering
        /// </summary>
        public bool IsGroupSummary { get; set; }

        /// <summary>
        /// Launch the App instead of posting the notification to the status bar.
        /// Only for use with extremely high-priority notifications demanding the user's immediate attention,
        /// such as an incoming phone call or alarm clock that the user has explicitly set to a particular time.
        /// If this facility is used for something else, please give the user an option to turn it off and use a normal notification, as this can be extremely disruptive.
        /// The system UI may choose to display a heads-up notification, instead of launching this app, while the user is using the device.
        /// Apps targeting Android Q 10 (29) and above will have to request a permission (Manifest.permission.USE_FULL_SCREEN_INTENT) in order to use full screen intents.
        /// To be launched, the notification must also be posted to a channel with importance level set to IMPORTANCE_HIGH or higher.
        /// </summary>
        public AndroidLaunch LaunchApp { get; set; }

        /// <summary>
        /// Default is true
        /// </summary>
        public bool LaunchAppWhenTapped { get; set; } = true;

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
        public bool Ongoing { get; set; }

        /// <summary>
        ///
        /// </summary>
        public AndroidPendingIntentFlags PendingIntentFlags { get; set; } = AndroidPendingIntentFlags.UpdateCurrent;

        /// <summary>
        /// Set the relative priority for this notification.
        /// In Android, Only used if Android Api below 26.
        /// Use NotificationCenter.CreateNotificationChannel when Android Api equal or above 26
        /// </summary>
        public AndroidPriority Priority { get; set; } = AndroidPriority.Default;

        /// <summary>
        /// Set the progress this notification represents. The platform template will represent this using a ProgressBar.
        /// </summary>
        public AndroidProgressBar? ProgressBar { get; set; }

        /// <summary>
        /// Specifies the time at which this notification should be canceled, if it is not already canceled.
        /// </summary>
        public TimeSpan? TimeoutAfter { get; set; }

        /// <summary>
        /// Vibrate with a given pattern.
        /// Pass in an array of int`s that are the durations for which to turn on or off the vibrator in milliseconds.
        /// The first value indicates the number of milliseconds to wait before turning the vibrator on.
        /// The next value indicates the number of milliseconds for which to keep the vibrator on before turning it off.
        /// Subsequent values alternate between durations in milliseconds to turn the vibrator off or to turn the vibrator on.
        ///
        /// This method was deprecated in API level 26. use NotificationChannel
        /// </summary>
        public long[] VibrationPattern { get; set; }

        /// <summary>
        /// Sphere of visibility of this notification,
        /// which affects how and when the SystemUI reveals the notification's presence and contents in untrusted situations (namely, on the secure lockscreen).
        /// </summary>
        public AndroidVisibilityType VisibilityType { get; set; } = AndroidVisibilityType.Private;

        /// <summary>
        /// DateTime set with When is shown in the content view.
        /// </summary>
        public DateTime? When { get; set; }
    }
}