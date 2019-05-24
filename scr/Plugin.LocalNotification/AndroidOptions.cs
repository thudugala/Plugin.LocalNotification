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
        /// </summary>
        public bool AutoCancel { get; set; } = true;

        /// <summary>
        /// If set, the notification icon and application name will have the provided ARGB color.
        /// </summary>
        public int? Color { get; set; }

        /// <summary>
        /// If set, the LED will have the provided ARGB color.
        /// </summary>
        public int? LedColor { get; set; }

        /// <summary>
        /// Set the relative priority for this notification.
        /// </summary>
        public NotificationPriority Priority { get; set; } = NotificationPriority.Default;

        /// <summary>
        /// Specifies the time at which this notification should be canceled, if it is not already canceled.
        /// </summary>
        public TimeSpan? TimeoutAfter { get; set; }

        /// <summary>
        /// The channel name, default is General
        /// </summary>
        public string ChannelName { get; set; } = "General";

        /// <summary>
        /// The channel description
        /// </summary>
        public string ChannelDescription { get; set; }
    }
}