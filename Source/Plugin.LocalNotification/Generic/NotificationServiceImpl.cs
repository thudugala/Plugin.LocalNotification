using Plugin.LocalNotification.EventArgs;

namespace Plugin.LocalNotification.Platforms;


internal class NotificationServiceImpl : INotificationService
{
    public Func<NotificationRequest, Task<NotificationEventReceivingArgs>>? NotificationReceiving { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    // Generic .NET target (non-Android/iOS): not supported at runtime
    public bool IsSupported => false;

#pragma warning disable CS0067 // Event is never used
    public event NotificationActionTappedEventHandler? NotificationActionTapped;
    public event NotificationReceivedEventHandler? NotificationReceived;
    public event NotificationDisabledEventHandler? NotificationsDisabled;
#pragma warning restore CS0067

    public Task<bool> AreNotificationsEnabled(NotificationPermission? permission = null) => throw new NotImplementedException();
    public bool Cancel(params int[] notificationIdList) => throw new NotImplementedException();
    public bool CancelAll() => throw new NotImplementedException();
    public bool Clear(params int[] notificationIdList) => throw new NotImplementedException();
    public bool ClearAll() => throw new NotImplementedException();
    public Task<IList<NotificationRequest>> GetDeliveredNotificationList() => throw new NotImplementedException();
    public Task<IList<NotificationRequest>> GetPendingNotificationList() => throw new NotImplementedException();
    public void OnNotificationActionTapped(NotificationActionEventArgs e) => throw new NotImplementedException();
    public void OnNotificationReceived(NotificationEventArgs e) => throw new NotImplementedException();
    public void OnNotificationsDisabled() => throw new NotImplementedException();
    public void RegisterCategoryList(HashSet<NotificationCategory> categoryList) => throw new NotImplementedException();
    public Task<bool> RequestNotificationPermission(NotificationPermission? permission = null) => throw new NotImplementedException();
    public Task<bool> Show(NotificationRequest request) => throw new NotImplementedException();
}


