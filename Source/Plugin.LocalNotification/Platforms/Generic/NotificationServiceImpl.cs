using Plugin.LocalNotification.EventArgs;

namespace Plugin.LocalNotification.Platforms.Generic
{
    internal class NotificationServiceImpl : INotificationService
    {
        public Func<NotificationRequest, Task<NotificationEventReceivingArgs>>? NotificationReceiving { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        // This usually is a placeholder as .NET MAUI apps typically don't run on .NET generic targets unless through unit tests and such
        public bool IsSupported => false;

        public event NotificationActionTappedEventHandler? NotificationActionTapped;
        public event NotificationReceivedEventHandler? NotificationReceived;
        public event NotificationDisabledEventHandler? NotificationsDisabled;

        public Task<bool> AreNotificationsEnabled() => throw new NotImplementedException();
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
}
