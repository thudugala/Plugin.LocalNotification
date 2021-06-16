using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// Used to display platform specific local notifications.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// fires when notification is received.
        /// On iOS this event is fired only when the app is in foreground
        /// </summary>
        event NotificationReceivedEventHandler NotificationReceived;

        /// <summary>
        /// fires when notification popup is tapped.
        /// </summary>
        event NotificationTappedEventHandler NotificationTapped;

        /// <summary>
        /// Cancel a notification match with the Id
        /// </summary>
        /// <param name="notificationId">A unique identifier for the already displaying local notification.</param>
        bool Cancel(int notificationId);

        /// <summary>
        /// Cancel all notification.
        /// </summary>
        bool CancelAll();

        /// <summary>
        /// Internal use Only
        /// </summary>
        /// <param name="e"></param>
        void OnNotificationReceived(NotificationReceivedEventArgs e);

        /// <summary>
        /// Internal use Only
        /// </summary>
        /// <param name="e"></param>
        void OnNotificationTapped(NotificationTappedEventArgs e);

        /// <summary>
        /// Send a local notification to the device.
        /// </summary>
        Task<bool> Show(Func<NotificationRequestBuilder, NotificationRequest> builder);

        /// <summary>
        /// Send a local notification to the device.
        /// </summary>
        /// <param name="request"></param>
        Task<bool> Show(NotificationRequest request);


        /// <summary>
        /// Storage for actions
        /// </summary>
        Dictionary<string, NotificationAction> NotificationActions { get; }

        /// <summary>
        /// Register notification categories and their corresponding actions
        /// </summary>
        void RegisterCategories(NotificationCategory[] notificationCategories);
    }
}