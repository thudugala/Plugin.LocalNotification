using System;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// Schedule notification Builder
    /// </summary>
    public class NotificationRequestScheduleBuilder
    {
        private TimeSpanExt? NotifyRepeatInterval;
        private DateTime? NotifyStopTime;
        private DateTime? NotifyTime;
        private NotificationRepeat RepeatType = NotificationRepeat.No;

        /// <summary>
        /// Creates the notification request
        /// </summary>
        /// <returns>The notification request</returns>
        public NotificationRequestSchedule Build() => new NotificationRequestSchedule()
        {
            NotifyRepeatInterval = NotifyRepeatInterval,
            NotifyAutoCancelTime = NotifyStopTime,
            NotifyTime = NotifyTime,
            Repeats = RepeatType
        };

        /// <summary>
        /// Time to show the notification.
        /// </summary>
        public NotificationRequestScheduleBuilder NotifyAt(DateTime? startTime, DateTime? autoCancelTime = null)
        {
            NotifyTime = startTime;
            NotifyStopTime = autoCancelTime;
            return this;
        }

        /// <summary>
        /// if Repeats = TimeInterval, then repeat again after specified amount of time elapses
        /// </summary>
        public NotificationRequestScheduleBuilder SetNotificationRepeatInterval(NotificationRepeat repeatType, TimeSpanExt? repeatInterval = null)
        {
            RepeatType = repeatType;
            NotifyRepeatInterval = repeatInterval;
            return this;
        }
    }
}