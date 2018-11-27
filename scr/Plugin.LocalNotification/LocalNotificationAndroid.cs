using System;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// LocalNotification for Android
    /// </summary>
    public class LocalNotificationAndroid
    {
        /// <summary>
        /// Setting this flag will make it so the notification is automatically canceled when the user clicks it in the panel.
        /// </summary>
        public bool AutoCancel { get; set; } = true;

        /// <summary>
        /// Specifies the time at which this notification should be canceled, if it is not already canceled.
        /// </summary>
        public TimeSpan? TimeoutAfter { get; set; }
    }
}