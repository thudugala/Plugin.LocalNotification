using System;
using Plugin.LocalNotification.AndroidOption;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// Schedule notification Builder
    /// </summary>
    public class NotificationRequestScheduleBuilder
    {
        private readonly NotificationRequestSchedule _schedule;

        /// <summary>
        ///
        /// </summary>
        public NotificationRequestScheduleBuilder()
        {
            _schedule = new NotificationRequestSchedule();
        }

        /// <summary>
        /// Creates the notification request
        /// </summary>
        /// <returns>The notification request</returns>
        public NotificationRequestSchedule Build() => _schedule;

        /// <summary>
        /// Time to show the notification.
        /// </summary>
        public NotificationRequestScheduleBuilder NotifyAt(DateTime? startTime, DateTime? autoCancelTime = null)
        {
            _schedule.NotifyTime = startTime;
            _schedule.NotifyAutoCancelTime = autoCancelTime;
            return this;
        }

        /// <summary>
        /// if Repeats = TimeInterval, then repeat again after specified amount of time elapses
        /// </summary>
        public NotificationRequestScheduleBuilder SetNotificationRepeatInterval(NotificationRepeat repeatType, TimeSpan? repeatInterval = null)
        {
            _schedule.RepeatType = repeatType;
            _schedule.NotifyRepeatInterval = repeatInterval;
            return this;
        }

        /// <summary>
        /// Android specific properties builder.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public NotificationRequestScheduleBuilder WithAndroidOptions(Func<AndroidScheduleOptionsBuilder, AndroidScheduleOptions> builder)
        {
            _schedule.Android = builder.Invoke(new AndroidScheduleOptionsBuilder());
            return this;
        }

        /// <summary>
        /// Android specific properties.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public NotificationRequestScheduleBuilder WithAndroidOptions(AndroidScheduleOptions options)
        {
            _schedule.Android = options;
            return this;
        }
    }
}