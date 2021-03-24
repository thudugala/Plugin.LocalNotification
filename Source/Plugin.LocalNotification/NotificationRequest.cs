﻿using System;
using System.Threading.Tasks;

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
        public AndroidOptions Android { get; set; } = new ();

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
        public iOSOptions iOS { get; set; } = new ();

        /// <summary>
        /// A unique identifier for the request
        /// (if identifier is not unique, a new notification request object is not created).
        /// You can use this identifier later to cancel a request that is still pending.
        /// </summary>
        public int NotificationId { get; set; }

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
        /// Creates a NotificationRequestBuilder instance with specified notificationId.
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        public static NotificationRequestBuilder CreateBuilder(int notificationId) => new (notificationId);

        /// <summary>
        /// Creates a NotificationRequestBuilder instance with default values.
        /// </summary>
        /// <returns></returns>
        public static NotificationRequestBuilder CreateBuilder() => new ();

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