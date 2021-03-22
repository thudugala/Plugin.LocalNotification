using System;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// NotificationRequest for Android
    /// </summary>
    public class AndroidOptionsBuilder
    {
        private bool AutoCancel;
        private string ChannelId;
        private string Group;
        private bool IsGroupSummary;
        private int? Color;
        private string IconName;
        private int? LedColor;
        private bool Ongoing;
        private NotificationPriority Priority;
        private bool? ProgressBarIndeterminate;
        private int? ProgressBarMax;
        private int? ProgressBarProgress;
        private TimeSpan? TimeoutAfter;
        private long[] VibrationPattern;

        internal AndroidOptionsBuilder()
        {
            Priority = NotificationPriority.Default;
            ChannelId = "Plugin.LocalNotification.GENERAL";
            AutoCancel = true;
        }

        /// <summary>
        /// Builds the request to <see cref="AndroidOptions"/>
        /// </summary>
        /// <returns></returns>
        public AndroidOptions Build()
        {
            return new()
            {
                AutoCancel = AutoCancel,
                VibrationPattern = VibrationPattern,
                ChannelId = ChannelId,
                Color = Color,
                Group = Group,
                IconName = IconName,
                IsGroupSummary = IsGroupSummary,
                LedColor = LedColor,
                Ongoing = Ongoing,
                Priority = Priority,
                ProgressBarIndeterminate = ProgressBarIndeterminate,
                ProgressBarMax = ProgressBarMax,
                ProgressBarProgress = ProgressBarProgress,
                TimeoutAfter = TimeoutAfter
            };
        }

        /// <summary>
        /// Sets or gets, The id of the channel. Must be unique per package. The value may be truncated if it is too lon
        /// Use this to target the Notification Channel.
        /// </summary>
        public AndroidOptionsBuilder WithChannelId(string channelId)
        {
            ChannelId = channelId;
            return this;
        }

        /// <summary>
        /// Set this notification to be part of a group of notifications sharing the same key.
        /// Grouped notifications may display in a cluster or stack on devices which support such rendering.
        /// </summary>
        public AndroidOptionsBuilder WithGroup(string group)
        {
            Group = group;
            return this;
        }

        /// <summary>
        /// Setting this flag will make it so the notification is automatically canceled when the user clicks it in the panel.
        /// Default is true
        /// </summary>
        public AndroidOptionsBuilder WithAutoCancel(bool shouldCancel)
        {
            AutoCancel = shouldCancel;
            return this;
        }

        /// <summary>
        /// Set this notification to be the group summary for a group of notifications.
        /// Grouped notifications may display in a cluster or stack on devices which support such rendering
        /// </summary>
        public AndroidOptionsBuilder WithGroupSummaryStatus(bool isGroupSummary)
        {
            IsGroupSummary = isGroupSummary;
            return this;
        }

        /// <summary>
        /// If set, the notification icon and application name will have the provided ARGB color.
        /// </summary>
        public AndroidOptionsBuilder WithColor(int? color)
        {
            Color = color;
            return this;
        }

        /// <summary>
        /// if Set, find the icon by name from drawable and set it has the Small Icon to use in the notification layouts.
        /// if not set, application Icon will we used.
        /// </summary>
        public AndroidOptionsBuilder WithIconName(string iconName)
        {
            IconName = iconName;
            return this;
        }

        /// <summary>
        /// If set, the LED will have the provided ARGB color.
        /// </summary>
        public AndroidOptionsBuilder WithLedColor(int? ledColor)
        {
            LedColor = ledColor;
            return this;
        }

        /// <summary>
        /// Set whether this is an ongoing notification.
        /// Ongoing notifications differ from regular notifications in the following ways,
        /// Ongoing notifications are sorted above the regular notifications in the notification panel.
        /// Ongoing notifications do not have an 'X' close button, and are not affected by the "Clear all" button.
        /// Default is false
        /// </summary>
        public AndroidOptionsBuilder WithOngoingStatus(bool isOngoing)
        {
            Ongoing = isOngoing;
            return this;
        }

        /// <summary>
        /// Set the relative priority for this notification.
        /// In Android, Only used if Android Api below 26.
        /// Use NotificationCenter.CreateNotificationChannel when Android Api equal or above 26
        /// </summary>
        public AndroidOptionsBuilder WithPriority(NotificationPriority priority)
        {
            Priority = priority;
            return this;
        }

        /// <summary>
        /// Set whether this progress bar is in indeterminate mode
        /// </summary>
        public AndroidOptionsBuilder WithProgressBarIndeterminate(bool? progressBarIndeterminate)
        {
            ProgressBarIndeterminate = progressBarIndeterminate;
            return this;
        }

        /// <summary>
        /// Set Upper limit of this progress bar's range
        /// </summary>
        public AndroidOptionsBuilder WithProgressBarMax(int? progressBarMax)
        {
            ProgressBarMax = progressBarMax;
            return this;
        }

        /// <summary>
        /// Set progress bar's current level of progress
        /// </summary>
        public AndroidOptionsBuilder WithProgressBarProgress(int? progress)
        {
            ProgressBarProgress = progress;
            return this;
        }

        /// <summary>
        /// Specifies the time at which this notification should be canceled, if it is not already canceled.
        /// </summary>
        public AndroidOptionsBuilder WithTimeout(TimeSpan? timeoutAfter)
        {
            TimeoutAfter = timeoutAfter;
            return this;
        }

        /// <summary>
        /// Vibrate with a given pattern.
        /// Pass in an array of ints that are the durations for which to turn on or off the vibrator in milliseconds.
        /// The first value indicates the number of milliseconds to wait before turning the vibrator on.
        /// The next value indicates the number of milliseconds for which to keep the vibrator on before turning it off.
        /// Subsequent values alternate between durations in milliseconds to turn the vibrator off or to turn the vibrator on.
        /// </summary>
        public AndroidOptionsBuilder WithVibrationPattern(long[] pattern)
        {
            VibrationPattern = pattern;
            return this;
        }
    }
}