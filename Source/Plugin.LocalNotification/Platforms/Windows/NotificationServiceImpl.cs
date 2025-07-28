using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Plugin.LocalNotification.EventArgs;

namespace Plugin.LocalNotification.Platforms;

internal class NotificationServiceImpl : INotificationService
{
    private readonly List<NotificationCategory> _categoryList = [];
    private readonly AppNotificationManager _notificationManager;

    public Func<NotificationRequest, Task<NotificationEventReceivingArgs>>? NotificationReceiving { get; set; }

    public bool IsSupported => true;

    public event NotificationActionTappedEventHandler? NotificationActionTapped;
    public event NotificationReceivedEventHandler? NotificationReceived;
    public event NotificationDisabledEventHandler? NotificationsDisabled;

    public NotificationServiceImpl()
    {
        _notificationManager = AppNotificationManager.Default;
        _notificationManager.NotificationInvoked += (sender, args) =>
        {
            var arguments = string.Join(";", args.Arguments.Select(kv => $"{kv.Key}={kv.Value}"));
            LocalNotificationCenter.NotifyNotificationTapped(arguments);
        };
    }

    public bool Cancel(params int[] notificationIdList)
    {
        foreach (var id in notificationIdList)
        {
            _notificationManager.RemoveByIdAsync((uint)id).GetAwaiter().GetResult();
        }
        return true;
    }

    public bool CancelAll()
    {
        _notificationManager.RemoveAllAsync().GetAwaiter().GetResult();
        return true;
    }

    public bool Clear(params int[] notificationIdList)
    {
        foreach (var id in notificationIdList)
        {
            _notificationManager.RemoveByIdAsync((uint)id).GetAwaiter().GetResult();
        }
        return true;
    }

    public bool ClearAll()
    {
        _notificationManager.RemoveAllAsync().GetAwaiter().GetResult();
        return true;
    }

    public async Task<IList<NotificationRequest>> GetDeliveredNotificationList()
    {
        var deliveredNotifications = new List<NotificationRequest>();
        var notifications = await _notificationManager.GetAllAsync();

        if (notifications == null || !notifications.Any())
        {
            return deliveredNotifications;
        }

        foreach (var notification in notifications)
        {
            var (_, request) = LocalNotificationCenter.GetRequestFromArguments(notification.Tag);
            if (request is not null)
            {
                deliveredNotifications.Add(request);
            }
        }

        return deliveredNotifications;
    }

    public Task<IList<NotificationRequest>> GetPendingNotificationList()
    {
        // Windows App SDK doesn't support retrieving scheduled notifications
        return Task.FromResult<IList<NotificationRequest>>(new List<NotificationRequest>());
    }

    public void OnNotificationActionTapped(NotificationActionEventArgs e)
    {
        NotificationActionTapped?.Invoke(e);
    }

    public void OnNotificationReceived(NotificationEventArgs e)
    {
        NotificationReceived?.Invoke(e);
    }

    public void OnNotificationsDisabled()
    {
        NotificationsDisabled?.Invoke();
    }

    public void RegisterCategoryList(HashSet<NotificationCategory> categoryList)
    {
        if (categoryList is null || !categoryList.Any())
        {
            return;
        }

        foreach (var category in categoryList.Where(category =>
                     category.CategoryType != NotificationCategoryType.None))
        {
            _categoryList.Add(category);
        }
    }

    public Task<bool> AreNotificationsEnabled(NotificationPermission? permission = null)
    {
        return Task.FromResult(_notificationManager.Setting == AppNotificationSetting.Enabled);
    }

    public Task<bool> RequestNotificationPermission(NotificationPermission? permission = null)
    {
        // For Windows, we just need to check if notifications are enabled
        // as there's no explicit permission system like iOS or Android           
        return AreNotificationsEnabled(permission);
    }

    public async Task<bool> Show(NotificationRequest request)
    {
        if (NotificationReceiving != null)
        {
            var args = await NotificationReceiving.Invoke(request);
            if (args.Handled)
            {
                return false;
            }
        }

        var builder = new AppNotificationBuilder()
            .AddArgument(LocalNotificationCenter.ReturnRequest, request.NotificationId.ToString())
            .AddArgument(LocalNotificationCenter.ReturnRequestActionId, NotificationActionEventArgs.TapActionId.ToString())
            .AddText(request.Title);

        if (!string.IsNullOrEmpty(request.Subtitle))
        {
            builder.AddText(request.Subtitle);
        }

        if (!string.IsNullOrEmpty(request.Description))
        {
            builder.AddText(request.Description);
        }

        // Set duration based on whether it's scheduled
        if (request.Schedule?.NotifyTime != null)
        {
            builder.SetDuration(AppNotificationDuration.Long);
        }
        else
        {
            builder.SetDuration(AppNotificationDuration.Default);
        }

        // Add actions if we have categories
        if (_categoryList.Any())
        {
            var categoryByType = _categoryList.FirstOrDefault(c => c.CategoryType == request.CategoryType);
            if (categoryByType != null)
            {
                foreach (var action in categoryByType.ActionList)
                {
                    builder.AddButton(new AppNotificationButton(action.Title)
                        .AddArgument(LocalNotificationCenter.ReturnRequest, request.NotificationId.ToString())
                        .AddArgument(LocalNotificationCenter.ReturnRequestActionId, action.ActionId.ToString()));
                }
            }
        }

        // Build and configure the notification
        var notification = builder.BuildNotification();
        notification.Tag = request.NotificationId.ToString();
        notification.Group = request.Group;

        // Show the notification
        _notificationManager.Show(notification);
        return true;
    }
}