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
        public TimeSpanExt? NotifyRepeatInterval { get; set; }

        /// <summary>
        /// Time to show the notification.
        /// </summary>
        public DateTime? NotifyTime { get; set; }

        /// <summary>
        /// If true, will repeat again at the time specifies in NotifyTime or NotifyRepeatInterval
        /// </summary>
        public NotificationRepeat Repeats { get; set; } = NotificationRepeat.No;
    }
}