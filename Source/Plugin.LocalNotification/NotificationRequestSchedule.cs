using System;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// Schedule notification
    /// </summary>
    public class NotificationRequestSchedule
    {
        /// <summary>
        /// Time to cancel the notification automatically.
        /// </summary>
        public DateTime? NotifyAutoCancelTime { get; set; }

        /// <summary>
        /// if Repeats = TimeInterval, then repeat again after specified amount of time elapses
        /// </summary>
        public TimeSpan? NotifyRepeatInterval { get; set; }

        /// <summary>
        /// Time to show the notification.
        /// </summary>
        public DateTime? NotifyTime { get; set; }

        /// <summary>
        /// If true, will repeat again at the time specifies in NotifyTime or NotifyRepeatInterval
        /// </summary>
        public NotificationRepeat RepeatType { get; set; } = NotificationRepeat.No;

        /// <summary>
        /// In Android, do not Schedule or show notification if NotifyTime is earlier than DateTime.Now and this time delay.
        /// Default is 1 min
        /// </summary>
        public TimeSpan AndroidAllowedDelay { get; set; } = TimeSpan.FromMinutes(1);

        internal DateTime? AndroidNotifyTimeWithDelay => NotifyTime.HasValue ? NotifyTime.Value.Add(AndroidAllowedDelay) : (DateTime?)null;

        internal bool AndroidIsValidNotifyTime => AndroidNotifyTimeWithDelay != null && AndroidNotifyTimeWithDelay > DateTime.Now;
    }
}