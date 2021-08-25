using System;

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
        /// In Android, do not Schedule or show notification if NotifyTime is earlier than DateTime.Now and this time delay.
        /// Defualt is 1 min
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        public NotificationRequestScheduleBuilder SetAndroidAllowedDelay(TimeSpan delay)
        {
            _schedule.AndroidAllowedDelay = delay;
            return this;
        }
    }
}