﻿using System.Threading.Tasks;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// Notification Request
    /// </summary>
    public class NotificationRequest
    {
        /// <summary>
        /// Android specific properties.
        /// </summary>
        public AndroidOptions Android { get; set; } = new AndroidOptions();

        /// <summary>
        /// Number of the badge displays on the Home Screen.
        /// </summary>
        public int BadgeNumber { get; set; }

        /// <summary>
        /// Details for the notification.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// iOS specific properties.
        /// </summary>
        public iOSOptions iOS { get; set; } = new iOSOptions();

        /// <summary>
        /// A unique identifier for the request
        /// (if identifier is not unique, a new notification request object is not created).
        /// You can use this identifier later to cancel a request that is still pending.
        /// </summary>
        public int NotificationId { get; set; }

        /// <summary>
        /// Schedule notification
        /// </summary>
        public NotificationRequestSchedule Schedule { get; set; } = new NotificationRequestSchedule();

        /// <summary>
        /// Returning data when tapped or received notification.
        /// </summary>
        public string ReturningData { get; set; } = string.Empty;

        /// <summary>
        /// Sound file name for the notification.
        /// In Android, Only used if Android Api below 26.
        /// Use NotificationCenter.CreateNotificationChannel when Android Api equal or above 26
        /// </summary>
        public string Sound { get; set; } = string.Empty;

        /// <summary>
        /// Title for the notification.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Subtitle for the notification.
        /// </summary>
        public string Subtitle { get; set; } = string.Empty;


        /// <summary>
        /// Image for notification.
        /// </summary>
        public NotificationImage Image { get; set; } = new NotificationImage();

        /// <summary>
        /// Notification category for actions
        /// </summary>
        public NotificationCategoryType CategoryType { get; set; } = NotificationCategoryType.None;

        /// <summary>
        /// Creates a NotificationRequestBuilder instance with specified notificationId.
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        public static NotificationRequestBuilder CreateBuilder(int notificationId) => new NotificationRequestBuilder(notificationId);

        /// <summary>
        /// Creates a NotificationRequestBuilder instance with default values.
        /// </summary>
        /// <returns></returns>
        public static NotificationRequestBuilder CreateBuilder() => new NotificationRequestBuilder();

        /// <summary>
        /// Directly call Show() on this <see cref="NotificationRequest"/> instance.
        /// </summary>
        /// <returns></returns>
        public Task<bool> Show() => NotificationCenter.Current.Show(this);

        /// <summary>
        /// Directly call Cancel(...) on this <see cref="NotificationRequest"/> instance.
        /// <para>Notification Id set for this instance will be used to cancel this notification.</para>
        /// </summary>
        /// <returns></returns>
        public bool Cancel() => NotificationCenter.Current.Cancel(this.NotificationId);
    }
}