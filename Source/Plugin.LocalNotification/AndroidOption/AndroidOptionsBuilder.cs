using System;

namespace Plugin.LocalNotification.AndroidOption
{
    /// <summary>
    /// NotificationRequest for Android
    /// </summary>
    public class AndroidOptionsBuilder
    {
        private readonly AndroidOptions _options;

        /// <summary>
        ///
        /// </summary>
        public AndroidOptionsBuilder()
        {
            _options = new AndroidOptions();
        }

        /// <summary>
        /// Builds the request to <see cref="AndroidOptions"/>
        /// </summary>
        /// <returns></returns>
        public AndroidOptions Build() => _options;

        /// <summary>
        /// Set this notification to be the group summary for a group of notifications.
        /// Grouped notifications may display in a cluster or stack on devices which support such rendering
        /// </summary>
        public AndroidOptionsBuilder ShouldGroupSummary(bool isGroupSummary)
        {
            _options.IsGroupSummary = isGroupSummary;
            return this;
        }

        /// <summary>
        /// Set whether this progress bar is in indeterminate mode
        /// </summary>
        public AndroidOptionsBuilder ShouldProgressBarIndeterminate(bool? progressBarIndeterminate)
        {
            _options.IsProgressBarIndeterminate = progressBarIndeterminate;
            return this;
        }

        /// <summary>
        /// Setting this flag will make it so the notification is automatically canceled when the user clicks it in the panel.
        /// Default is true
        /// </summary>
        public AndroidOptionsBuilder WithAutoCancel(bool shouldCancel)
        {
            _options.AutoCancel = shouldCancel;
            return this;
        }

        /// <summary>
        /// Sets or gets, The id of the channel. Must be unique per package. The value may be truncated if it is too lon
        /// Use this to target the Notification Channel.
        /// </summary>
        public AndroidOptionsBuilder WithChannelId(string channelId)
        {
            _options.ChannelId = channelId ?? AndroidOptions.DefaultChannelId;
            return this;
        }

        /// <summary>
        /// If set, the notification icon and application name will have the provided ARGB color.
        /// </summary>
        public AndroidOptionsBuilder WithColor(string color)
        {
            _options.Color = color;
            return this;
        }

        /// <summary>
        /// Set this notification to be part of a group of notifications sharing the same key.
        /// Grouped notifications may display in a cluster or stack on devices which support such rendering.
        /// </summary>
        public AndroidOptionsBuilder WithGroup(string group)
        {
            _options.Group = group;
            return this;
        }

        /// <summary>
        /// if Set, find the icon by name from drawable and set it has the Large Icon to use in the notification layouts.
        /// if not set, application Icon will we used.
        /// </summary>
        public AndroidOptionsBuilder WithIconLargeName(AndroidNotificationIcon icon)
        {
            _options.IconLargeName = icon;
            return this;
        }

        /// <summary>
        /// if Set, find the icon by name from drawable and set it has the Small Icon to use in the notification layouts.
        /// if not set, application Icon will we used.
        /// </summary>
        public AndroidOptionsBuilder WithIconSmallName(AndroidNotificationIcon icon)
        {
            _options.IconSmallName = icon;
            return this;
        }

        /// <summary>
        /// If set, the LED will have the provided ARGB color.
        /// </summary>
        public AndroidOptionsBuilder WithLedColor(int? ledColor)
        {
            _options.LedColor = ledColor;
            return this;
        }

        /// <summary>
        /// Set whether this is an ongoing notification.
        /// Ongoing notifications differ from regular notifications in the following ways,
        /// Ongoing notifications are sorted above the regular notifications in the notification panel.
        /// Ongoing notifications do not have an 'X' close button, and are not affected by the "Clear all" button.
        /// Default is false
        /// </summary>
        public AndroidOptionsBuilder WithOngoing(bool isOngoing)
        {
            _options.Ongoing = isOngoing;
            return this;
        }

        /// <summary>
        /// Set the relative priority for this notification.
        /// In Android, Only used if Android Api below 26.
        /// Use NotificationCenter.CreateNotificationChannel when Android Api equal or above 26
        /// </summary>
        public AndroidOptionsBuilder WithPriority(NotificationPriority priority)
        {
            _options.Priority = priority;
            return this;
        }

        /// <summary>
        /// Set Upper limit of this progress bar's range
        /// </summary>
        public AndroidOptionsBuilder WithProgressBarMax(int? progressBarMax)
        {
            _options.ProgressBarMax = progressBarMax;
            return this;
        }

        /// <summary>
        /// Set progress bar's current level of progress
        /// </summary>
        public AndroidOptionsBuilder WithProgressBarProgress(int? progress)
        {
            _options.ProgressBarProgress = progress;
            return this;
        }

        /// <summary>
        /// Specifies the time at which this notification should be canceled, if it is not already canceled.
        /// </summary>
        public AndroidOptionsBuilder WithTimeout(TimeSpan? timeoutAfter)
        {
            _options.TimeoutAfter = timeoutAfter;
            return this;
        }

        /// <summary>
        /// Vibrate with a given pattern.
        /// Pass in an array of int`s that are the durations for which to turn on or off the vibrator in milliseconds.
        /// The first value indicates the number of milliseconds to wait before turning the vibrator on.
        /// The next value indicates the number of milliseconds for which to keep the vibrator on before turning it off.
        /// Subsequent values alternate between durations in milliseconds to turn the vibrator off or to turn the vibrator on.
        /// </summary>
        public AndroidOptionsBuilder WithVibrationPattern(long[] pattern)
        {
            _options.VibrationPattern = pattern;
            return this;
        }

        /// <summary>
        /// Sphere of visibility of this notification,
        /// which affects how and when the SystemUI reveals the notification's presence and contents in untrusted situations (namely, on the secure lockscreen).
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public AndroidOptionsBuilder WithVisibilityType(AndroidVisibilityType type)
        {
            _options.VisibilityType = type;
            return this;
        }
    }
}