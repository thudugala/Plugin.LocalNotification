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
        /// fires when notification popup action is tapped.
        /// </summary>
        event NotificationActionTappedEventHandler NotificationActionTapped;

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
        /// <param name="notificationIdList">A unique identifier for the already displaying local notification.</param>
        bool Cancel(params int[] notificationIdList);

        /// <summary>
        /// Cancel all notification.
        /// </summary>
        bool CancelAll();

        /// <summary>
        /// Use this method to selectively remove notifications that you no longer want displayed in Notification Center. This will not cancel future notifications
        /// </summary>
        /// <param name="notificationIdList"></param>
        bool Clear(params int[] notificationIdList);

        /// <summary>
        /// Use this method to remove all notifications displayed in Notification Center. This will not cancel future notifications.
        /// </summary>
        bool ClearAll();

        /// <summary>
        /// Get notifications that are currently delivered
        /// </summary>
        /// <returns></returns>
        Task<IList<NotificationRequest>> GetDeliveredNotificationList();

        /// <summary>
        /// Internal use Only
        /// </summary>
        /// <param name="e"></param>
        void OnNotificationActionTapped(NotificationActionEventArgs e);

        /// <summary>
        /// Internal use Only
        /// </summary>
        /// <param name="e"></param>
        void OnNotificationReceived(NotificationEventArgs e);

        /// <summary>
        /// Internal use Only
        /// </summary>
        /// <param name="e"></param>
        void OnNotificationTapped(NotificationEventArgs e);

        /// <summary>
        /// Get pending notifications
        /// </summary>
        /// <returns></returns>
        Task<IList<NotificationRequest>> GetPendingNotificationList();

        /// <summary>
        /// Register notification categories and their corresponding actions
        /// </summary>
        void RegisterCategoryList(HashSet<NotificationCategory> categoryList);

        /// <summary>
        /// Send a local notification to the device.
        /// </summary>
        Task<bool> Show(Func<NotificationRequestBuilder, NotificationRequest> builder);

        /// <summary>
        /// Send a local notification to the device.
        /// </summary>
        /// <param name="request"></param>
        Task<bool> Show(NotificationRequest request);
    }
}