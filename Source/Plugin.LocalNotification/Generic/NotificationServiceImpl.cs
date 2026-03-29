using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.EventArgs;

namespace Plugin.LocalNotification.Platforms;


internal class NotificationServiceImpl : INotificationService
{
    public Func<NotificationRequest, Task<NotificationEventReceivingArgs>>? NotificationReceiving { get; set; }

    // Generic .NET target (non-Android/IOS): not supported at runtime
    public bool IsSupported => false;

#pragma warning disable CS0067 // Event is never used
    public event NotificationActionTappedEventHandler? NotificationActionTapped;
    public event NotificationReceivedEventHandler? NotificationReceived;
    public event NotificationDisabledEventHandler? NotificationsDisabled;
#pragma warning restore CS0067

    public Task<bool> AreNotificationsEnabled(NotificationPermission? permission = null) => Task.FromResult(false);
    public Task<NotificationPermissionStatus> GetNotificationPermissionStatus() => Task.FromResult(new NotificationPermissionStatus());
    public bool Cancel(params int[] notificationIdList) => false;
    public bool CancelAll() => false;
    public bool Clear(params int[] notificationIdList) => false;
    public bool ClearAll() => false;
    public Task<IList<NotificationRequest>> GetDeliveredNotificationList() => Task.FromResult<IList<NotificationRequest>>([]);
    public Task<IList<ActiveNotification>> GetActiveNotifications() => Task.FromResult<IList<ActiveNotification>>([]);
    public Task<IList<NotificationRequest>> GetPendingNotificationList() => Task.FromResult<IList<NotificationRequest>>([]);
    public void OnNotificationActionTapped(NotificationActionEventArgs e) => NotificationActionTapped?.Invoke(e);
    public void OnNotificationReceived(NotificationEventArgs e) => NotificationReceived?.Invoke(e);
    public void OnNotificationsDisabled() => NotificationsDisabled?.Invoke();
    public void RegisterCategoryList(HashSet<NotificationCategory> categoryList) { }
    public Task<bool> RequestNotificationPermission(NotificationPermission? permission = null) => Task.FromResult(false);
    public Task<bool> Show(NotificationRequest request) => Task.FromResult(false);
    public bool Cancel(int notificationId, string? tag) => false;
}


