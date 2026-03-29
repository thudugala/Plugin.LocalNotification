using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.EventArgs;

namespace Plugin.LocalNotification;

/// <summary>
/// Used to display platform specific local notifications.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Gets a value indicating whether local notification is supported on this device.
    /// </summary>
    bool IsSupported { get; }

    /// <summary>
    /// fires when notification popup action is tapped.
    /// </summary>
    event NotificationActionTappedEventHandler? NotificationActionTapped;

    /// <summary>
    /// fires when notification is received.
    /// On IOS this event is fired only when the app is in foreground
    /// </summary>
    event NotificationReceivedEventHandler? NotificationReceived;

    /// <summary>
    /// fires when notification is disabled.
    /// </summary>
    event NotificationDisabledEventHandler? NotificationsDisabled;

    /// <summary>
    /// Cancel a notification match with the Id
    /// </summary>
    /// <param name="notificationIdList">A unique identifier for the already displaying local notification.</param>
    bool Cancel(params int[] notificationIdList);

    /// <summary>
    /// Cancel a notification that was posted with an Android tag and the given Id.
    /// On non-Android platforms this is equivalent to <see cref="Cancel(int[])"/>.
    /// </summary>
    /// <param name="notificationId">The notification identifier.</param>
    /// <param name="tag">The Android tag used when the notification was posted, or <c>null</c>.</param>
    bool Cancel(int notificationId, string? tag);

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
    /// Returns the notifications that are currently visible in the device's notification center,
    /// along with rich metadata such as channel identifier, group key, Android tag, and big-text body.
    /// On Android this queries the native notification manager (Android 6.0+ / API 23+ required;
    /// returns an empty list on older versions). On iOS and macOS it returns delivered notifications.
    /// On Windows and other platforms an empty list is returned.
    /// </summary>
    /// <returns>A list of <see cref="ActiveNotification"/> objects for each displayed notification.</returns>
    Task<IList<ActiveNotification>> GetActiveNotifications();

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
    void OnNotificationsDisabled();

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
    /// <param name="request"></param>
    Task<bool> Show(NotificationRequest request);

    /// <summary>
    /// When Notification is about to be shown, this allow it to be modified.
    /// </summary>
    Func<NotificationRequest, Task<NotificationEventReceivingArgs>>? NotificationReceiving { get; set; }

    /// <summary>
    /// Returns whether user as allowed Notifications
    /// </summary>
    /// <returns></returns>
    Task<bool> AreNotificationsEnabled(NotificationPermission? permission = null);

    /// <summary>
    /// Request Notification Permission
    /// Ask the user for permission to show notifications on IOS 10.0+ and Android 33+.
    /// Returns true if Allowed.
    /// </summary>
    /// <returns></returns>
    Task<bool> RequestNotificationPermission(NotificationPermission? permission = null);

    /// <summary>
    /// Returns a granular breakdown of the notification permissions currently granted to the app,
    /// including individual capability states such as sound, badge, alerts, critical alerts, and
    /// (on Android) whether exact alarms can be scheduled.
    /// </summary>
    /// <returns>A <see cref="NotificationPermissionStatus"/> describing each permission capability.</returns>
    Task<NotificationPermissionStatus> GetNotificationPermissionStatus();
}