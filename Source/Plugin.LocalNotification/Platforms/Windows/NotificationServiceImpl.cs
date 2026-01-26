using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Plugin.LocalNotification.EventArgs;
using System.Globalization;

namespace Plugin.LocalNotification.Platforms;

/// <inheritdoc />
internal class NotificationServiceImpl : INotificationService
{
    public Func<NotificationRequest, Task<NotificationEventReceivingArgs>>? NotificationReceiving { get; set; }

    public bool IsSupported => OperatingSystem.IsWindows();

    public event NotificationReceivedEventHandler? NotificationReceived;
    public event NotificationActionTappedEventHandler? NotificationActionTapped;
    public event NotificationDisabledEventHandler? NotificationsDisabled;

    public void OnNotificationReceived(NotificationEventArgs e) => NotificationReceived?.Invoke(e);
    public void OnNotificationActionTapped(NotificationActionEventArgs e) => NotificationActionTapped?.Invoke(e);
    public void OnNotificationsDisabled() => NotificationsDisabled?.Invoke();

    public bool Cancel(params int[] notificationIdList)
    {
        var idList = notificationIdList.Select(id => (uint)id);
        foreach (var id in idList)
        {
            AppNotificationManager.Default.RemoveByIdAsync(id).GetAwaiter().GetResult();
        }
        return true;
    }

    public bool CancelAll()
    {
        AppNotificationManager.Default.RemoveAllAsync().GetAwaiter().GetResult();
        return true;
    }

    public bool Clear(params int[] notificationIdList)
    {
        var idList = notificationIdList.Select(id => (uint)id);
        foreach (var id in idList)
        {
            AppNotificationManager.Default.RemoveByIdAsync(id).GetAwaiter().GetResult();
        }
        return true;
    }

    public bool ClearAll()
    {
        AppNotificationManager.Default.RemoveAllAsync().GetAwaiter().GetResult();
        return true;
    }

    public async Task<bool> Show(NotificationRequest request)
    {
        if (!OperatingSystem.IsWindows() || request is null)
        {
            return false;
        }

        var allowed = await AreNotificationsEnabled();
        if (!allowed)
        {
            OnNotificationsDisabled();
            return false;
        }

        var builder = new AppNotificationBuilder()
            .AddArgument("notificationId", request.NotificationId.ToString(CultureInfo.CurrentCulture))
            .AddText(request.Title ?? string.Empty);

        if (!string.IsNullOrWhiteSpace(request.Subtitle))
        {
            builder.AddText(request.Subtitle);
        }

        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            builder.AddText(request.Description);
        }

        if (!string.IsNullOrWhiteSpace(request.Group))
        {
            builder.SetGroup(request.Group);
        }

        var appNotif = builder.BuildNotification();

        AppNotificationManager.Default.Show(appNotif);
        return true;
    }

    public Task<IList<NotificationRequest>> GetPendingNotificationList()
    {
        // Not supported with AppNotification scheduling yet in minimal implementation.
        return Task.FromResult<IList<NotificationRequest>>([]);
    }

    public Task<IList<NotificationRequest>> GetDeliveredNotificationList()
    {
        // Not available directly.
        return Task.FromResult<IList<NotificationRequest>>([]);
    }

    public Task<bool> AreNotificationsEnabled(NotificationPermission? permission = null)
    {
        // AppNotificationManager requires registration and Windows notifications must be enabled in OS.
        // Use AppNotificationManager.IsSupported to check OS support.
        var supported = AppNotificationManager.IsSupported();
        return Task.FromResult(supported);
    }

    public Task<bool> RequestNotificationPermission(NotificationPermission? permission = null)
    {
        // Windows typically doesn't prompt; ensure registration for notifications.
        AppNotificationManager.Default.Register();
        return Task.FromResult(true);
    }

    public void RegisterCategoryList(HashSet<NotificationCategory> categoryList)
    {
        // Windows AppNotification does not have category registration equivalent. No-op.
    }
}
