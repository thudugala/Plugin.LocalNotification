using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;
using System.Threading.Tasks;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// Notification Request
    /// </summary>
    public class NotificationRequest
    {
        /// <summary>
        /// Number of the badge displays on the Home Screen.
        /// </summary>
        public int BadgeNumber { get; set; }

        /// <summary>
        /// Notification category for actions
        /// </summary>
        public NotificationCategoryType CategoryType { get; set; } = NotificationCategoryType.None;

        /// <summary>
        /// Details for the notification.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Set this notification to be part of a group of notifications sharing the same key.
        /// Grouped notifications may display in a cluster or stack on devices which support such rendering.
        /// </summary>
        public string Group { get; set; } = string.Empty;

        /// <summary>
        /// Image for notification.
        /// </summary>
        public NotificationImage Image { get; set; } = new NotificationImage();

        /// <summary>
        /// Android specific properties.
        /// </summary>
        public AndroidOptions Android { get; set; } = new();

        /// <summary>
        /// iOS specific properties.
        /// </summary>
        public iOSOptions iOS { get; set; } = new();

        /// <summary>
        /// A unique identifier for the request
        /// (if identifier is not unique, a new notification request object is not created).
        /// You can use this identifier later to cancel a request that is still pending.
        /// </summary>
        public int NotificationId { get; set; }

        /// <summary>
        /// Returning data when tapped or received notification.
        /// </summary>
        public string ReturningData { get; set; } = string.Empty;

        /// <summary>
        /// Schedule notification (cannot be mixed with geofence)
        /// </summary>
        public NotificationRequestSchedule Schedule { get; set; } = new();

        /// <summary>
        /// Set the location aware geofence (cannot be mixed with schedule)
        /// </summary>
        public NotificationRequestGeofence Geofence { get; set; } = new();

        /// <summary>
        /// Silences this instance of the notification, regardless of the sounds or vibrations set on the notification or notification channel.
        /// </summary>
        public bool Silent { get; set; }

        /// <summary>
        /// Sound file name for the notification.
        /// In Android, Only used if Android Api below 26.
        /// Use NotificationCenter.CreateNotificationChannel when Android Api equal or above 26
        /// </summary>
        public string Sound { get; set; } = string.Empty;

        /// <summary>
        /// Subtitle for the notification.
        /// </summary>
        public string Subtitle { get; set; } = string.Empty;

        /// <summary>
        /// Title for the notification.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Directly call Cancel(...) on this <see cref="NotificationRequest"/> instance.
        /// <para>Notification Id set for this instance will be used to cancel this notification.</para>
        /// </summary>
        /// <returns></returns>
        public bool Cancel() => LocalNotificationCenter.Current.Cancel(NotificationId);

        /// <summary>
        /// Directly call Show() on this <see cref="NotificationRequest"/> instance.
        /// </summary>
        /// <returns></returns>
        public Task<bool> Show() => LocalNotificationCenter.Current.Show(this);
    }
}