using System;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// LocalNotification for Android
    /// </summary>
    public class AndroidOptions
    {
        /// <summary>
        /// Setting this flag will make it so the notification is automatically canceled when the user clicks it in the panel.
        /// </summary>
        public bool AutoCancel { get; set; } = true;

        /// <summary>
        /// Specifies the time at which this notification should be canceled, if it is not already canceled.
        /// </summary>
        public TimeSpan? TimeoutAfter { get; set; }

        /// <summary>
        /// Set the relative priority for this notification.
        /// </summary>
        public NotificationPriority Priority { get; set; } = NotificationPriority.Default;
    }

    /// <summary>
    /// Set the relative priority for this notification.
    /// Priority is an indication of how much of the user's valuable attention should be consumed by this notification.
    /// Low-priority notifications may be hidden from the user in certain situations, while the user might be interrupted for a higher-priority notification.
    /// The system sets a notification's priority based on various factors including the setPriority value.
    /// The effect may differ slightly on different platforms.
    /// </summary>
    public enum NotificationPriority
    {
        /// <summary>
        /// Lowest notification priority, these items might not be shown to the user except under special circumstances, such as detailed notification logs. 
        /// </summary>
        Min = -2,
        /// <summary>
        /// Lower notification priority, for items that are less important.
        /// The UI may choose to show these items smaller, or at a different position in the list, compared with your app's Default items. 
        /// </summary>
        Low = -1,
        /// <summary>
        /// If your application does not prioritize its own notifications, use this value for all notifications. 
        /// </summary>
        Default = 0,
        /// <summary>
        /// Higher notification priority, for more important notifications or alerts.
        /// The UI may choose to show these items larger, or at a different position in notification lists, compared with your app's Default items. 
        /// </summary>
        High = 1,
        /// <summary>
        /// Highest notification priority, for your application's most important items that require the user's prompt attention or input. 
        /// </summary>
        Max = 2,
        
    }
}