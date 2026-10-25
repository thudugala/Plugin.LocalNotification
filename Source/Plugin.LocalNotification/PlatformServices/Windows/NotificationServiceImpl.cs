using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Plugin.LocalNotification.Core;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.EventArgs;
using System.Collections.Concurrent;
using System.Globalization;

namespace Plugin.LocalNotification.Platforms;

/// <inheritdoc />
internal class NotificationServiceImpl : INotificationService
{
    private readonly List<NotificationCategory> _categoryList = [];
    private readonly ConcurrentDictionary<int, Timer> _scheduledTimers = new();

    /// <inheritdoc />
    public Func<NotificationRequest, Task<NotificationEventReceivingArgs>>? NotificationReceiving { get; set; }

    /// <inheritdoc />
    public bool IsSupported => OperatingSystem.IsWindows();

    /// <inheritdoc />
    public event NotificationReceivedEventHandler? NotificationReceived;

    /// <inheritdoc />
    public event NotificationActionTappedEventHandler? NotificationActionTapped;

    /// <inheritdoc />
    public event NotificationDisabledEventHandler? NotificationsDisabled;

    /// <inheritdoc />
    public void OnNotificationReceived(NotificationEventArgs e) => NotificationReceived?.Invoke(e);

    /// <inheritdoc />
    public void OnNotificationActionTapped(NotificationActionEventArgs e) => NotificationActionTapped?.Invoke(e);

    /// <inheritdoc />
    public void OnNotificationsDisabled() => NotificationsDisabled?.Invoke();

    /// <summary>
    ///
    /// </summary>
    public NotificationServiceImpl()
    {
        try
        {
            AppNotificationManager.Default.NotificationInvoked += OnNotificationInvoked;
            AppNotificationManager.Default.Register();
            ReschedulePendingNotifications();
        }
        catch (Exception ex)
        {
            LocalNotificationLogger.Log(ex);
        }
    }

    private void OnNotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
    {
        try
        {
            var notificationId = 0;
            var actionId = NotificationActionEventArgs.TapActionId;

            if (args.Arguments.TryGetValue("notificationId", out var idStr) &&
                int.TryParse(idStr, CultureInfo.InvariantCulture, out var parsedId))
            {
                notificationId = parsedId;
            }

            if (args.Arguments.TryGetValue("actionId", out var actionIdStr) &&
                int.TryParse(actionIdStr, CultureInfo.InvariantCulture, out var parsedActionId))
            {
                actionId = parsedActionId;
            }

            var deliveredList = NotificationRepository.GetDeliveredList();
            var request = deliveredList.FirstOrDefault(r => r.NotificationId == notificationId)
                          ?? new NotificationRequest { NotificationId = notificationId };

            if (args.Arguments.TryGetValue("returningData", out var returningData))
            {
                request.ReturningData = returningData;
            }

            var eventArgs = new NotificationActionEventArgs
            {
                Request = request,
                ActionId = actionId
            };

            OnNotificationActionTapped(eventArgs);
        }
        catch (Exception ex)
        {
            LocalNotificationLogger.Log(ex);
        }
    }

    internal void ReschedulePendingNotifications()
    {
        try
        {
            var pendingList = NotificationRepository.GetPendingList();
            var expiredIds = new List<int>();

            foreach (var request in pendingList)
            {
                if (request.Schedule.NotifyTime.HasValue)
                {
                    var delay = request.Schedule.NotifyTime.Value - DateTimeOffset.Now;
                    if (delay > TimeSpan.Zero)
                    {
                        ScheduleTimer(request, delay);
                    }
                    else
                    {
                        expiredIds.Add(request.NotificationId);
                    }
                }
            }

            if (expiredIds.Count > 0)
            {
                NotificationRepository.RemoveByPendingIdList([.. expiredIds]);
            }
        }
        catch (Exception ex)
        {
            LocalNotificationLogger.Log(ex);
        }
    }

    /// <inheritdoc />
    public bool Cancel(params int[] notificationIdList)
    {
        foreach (var id in notificationIdList)
        {
            CancelScheduledTimer(id);

            try
            {
                var tag = id.ToString(CultureInfo.InvariantCulture);
                AppNotificationManager.Default.RemoveByTagAsync(tag).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                LocalNotificationLogger.Log(ex);
            }
        }

        NotificationRepository.RemoveByPendingIdList(notificationIdList);
        NotificationRepository.RemoveByDeliveredIdList(notificationIdList);
        return true;
    }

    /// <inheritdoc />
    public bool Cancel(int notificationId, string? tag) => Cancel(notificationId);

    /// <inheritdoc />
    public bool CancelAll()
    {
        CancelAllScheduledTimers();

        try
        {
            AppNotificationManager.Default.RemoveAllAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            LocalNotificationLogger.Log(ex);
        }

        NotificationRepository.RemovePendingList();
        NotificationRepository.RemoveDeliveredList();
        return true;
    }

    /// <inheritdoc />
    public bool Clear(params int[] notificationIdList)
    {
        foreach (var id in notificationIdList)
        {
            try
            {
                var tag = id.ToString(CultureInfo.InvariantCulture);
                AppNotificationManager.Default.RemoveByTagAsync(tag).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                LocalNotificationLogger.Log(ex);
            }
        }

        NotificationRepository.RemoveByDeliveredIdList(notificationIdList);
        return true;
    }

    /// <inheritdoc />
    public bool ClearAll()
    {
        try
        {
            AppNotificationManager.Default.RemoveAllAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            LocalNotificationLogger.Log(ex);
        }

        NotificationRepository.RemoveDeliveredList();
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> Show(NotificationRequest request)
    {
        if (!OperatingSystem.IsWindows() || request is null)
        {
            return false;
        }

        var allowed = await AreNotificationsEnabled().ConfigureAwait(false);
        if (!allowed)
        {
            OnNotificationsDisabled();
            return false;
        }

        return request.Schedule.NotifyTime.HasValue 
            ? ShowLater(request) 
            : await ShowNow(request).ConfigureAwait(false);
    }

    private bool ShowLater(NotificationRequest request)
    {
        var notifyTime = request.Schedule.NotifyTime!.Value;
        var delay = notifyTime - DateTimeOffset.Now;

        if (delay <= TimeSpan.Zero)
        {
            LocalNotificationLogger.Log("NotifyTime is in the past, notification ignored");
            return false;
        }

        CancelScheduledTimer(request.NotificationId);
        ScheduleTimer(request, delay);
        NotificationRepository.AddPendingRequest(request);
        return true;
    }

    private void ScheduleTimer(NotificationRequest request, TimeSpan delay)
    {
        var timer = new Timer(async _ =>
        {
            if (_scheduledTimers.TryRemove(request.NotificationId, out var t))
            {
                t.Dispose();
            }

            NotificationRepository.RemoveByPendingIdList(request.NotificationId);
            await ShowNow(request).ConfigureAwait(false);
            ScheduleRepeat(request);
        }, null, delay, Timeout.InfiniteTimeSpan);

        _scheduledTimers[request.NotificationId] = timer;
    }

    private void ScheduleRepeat(NotificationRequest request)
    {
        if (request.Schedule.RepeatType == NotificationRepeat.No)
        {
            return;
        }

        var nextNotifyTime = request.Schedule.RepeatType switch
        {
            NotificationRepeat.Daily => request.Schedule.NotifyTime?.AddDays(1),
            NotificationRepeat.Weekly => request.Schedule.NotifyTime?.AddDays(7),
            NotificationRepeat.TimeInterval when request.Schedule.NotifyRepeatInterval.HasValue =>
                DateTimeOffset.Now.Add(request.Schedule.NotifyRepeatInterval.Value),
            _ => null
        };

        if (nextNotifyTime.HasValue)
        {
            if (request.Schedule.NotifyAutoCancelTime.HasValue &&
                nextNotifyTime.Value > request.Schedule.NotifyAutoCancelTime.Value)
            {
                return;
            }

            request.Schedule.NotifyTime = nextNotifyTime;
            var delay = nextNotifyTime.Value - DateTimeOffset.Now;
            if (delay > TimeSpan.Zero)
            {
                ScheduleTimer(request, delay);
                NotificationRepository.AddPendingRequest(request);
            }
        }
    }

    private void CancelScheduledTimer(int notificationId)
    {
        if (_scheduledTimers.TryRemove(notificationId, out var timer))
        {
            timer.Dispose();
        }
    }

    private void CancelAllScheduledTimers()
    {
        foreach (var kvp in _scheduledTimers)
        {
            if (_scheduledTimers.TryRemove(kvp.Key, out var timer))
            {
                timer.Dispose();
            }
        }
    }

    private async Task<bool> ShowNow(NotificationRequest request)
    {
        var requestHandled = false;
        if (NotificationReceiving != null)
        {
            var requestArg = await NotificationReceiving(request).ConfigureAwait(false);
            if (requestArg is null || requestArg.Handled)
            {
                requestHandled = true;
            }

            if (requestArg?.Request != null)
            {
                request = requestArg.Request;
            }
        }

        if (!requestHandled)
        {
            var builder = new AppNotificationBuilder()
                .AddArgument("notificationId", request.NotificationId.ToString(CultureInfo.InvariantCulture));

            if (!string.IsNullOrWhiteSpace(request.ReturningData))
            {
                builder.AddArgument("returningData", request.ReturningData);
            }

            builder.AddText(request.Title ?? string.Empty);

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

            // Image support
            if (request.Image is { HasValue: true })
            {
                var imageUri = GetImageUri(request.Image);
                if (imageUri != null)
                {
                    builder.SetInlineImage(imageUri);
                }
            }

            // Category action buttons
            if (request.CategoryType != NotificationCategoryType.None)
            {
                var category = _categoryList.FirstOrDefault(c => c.CategoryType == request.CategoryType);
                if (category != null)
                {
                    foreach (var action in category.ActionList)
                    {
                        var button = new AppNotificationButton(action.Title)
                            .AddArgument("notificationId", request.NotificationId.ToString(CultureInfo.InvariantCulture))
                            .AddArgument("actionId", action.ActionId.ToString(CultureInfo.InvariantCulture));

                        if (!string.IsNullOrWhiteSpace(request.ReturningData))
                        {
                            button.AddArgument("returningData", request.ReturningData);
                        }

                        builder.AddButton(button);
                    }
                }
            }

            if (request.Silent)
            {
                builder.MuteAudio();
            }

            var appNotif = builder.BuildNotification();

            // Set tag for programmatic management (cancel/clear by notification ID)
            appNotif.Tag = request.NotificationId.ToString(CultureInfo.InvariantCulture);

            if (!string.IsNullOrWhiteSpace(request.Group))
            {
                appNotif.Group = request.Group;
            }

            if (request.Schedule.NotifyAutoCancelTime.HasValue)
            {
                appNotif.Expiration = request.Schedule.NotifyAutoCancelTime.Value;
            }

            AppNotificationManager.Default.Show(appNotif);
        }
        else
        {
            LocalNotificationLogger.Log("NotificationServiceImpl.ShowNow: notification is Handled");
        }

        var args = new NotificationEventArgs
        {
            Request = request
        };
        OnNotificationReceived(args);
        NotificationRepository.AddDeliveredRequest(request);

        return true;
    }

    private static Uri? GetImageUri(NotificationImage? image)
    {
        if (image is null || !image.HasValue)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(image.FilePath) && File.Exists(image.FilePath))
        {
            return new Uri(image.FilePath);
        }

        if (image.Binary is { Length: > 0 })
        {
            try
            {
                var tempDir = Path.Combine(Path.GetTempPath(), "Plugin.LocalNotification");
                Directory.CreateDirectory(tempDir);
                var tempPath = Path.Combine(tempDir, $"notif_img_{Guid.NewGuid()}.png");
                File.WriteAllBytes(tempPath, image.Binary);
                return new Uri(tempPath);
            }
            catch (Exception ex)
            {
                LocalNotificationLogger.Log(ex);
            }
        }

        return null;
    }

    /// <inheritdoc />
    public Task<IList<NotificationRequest>> GetPendingNotificationList()
    {
        IList<NotificationRequest> itemList = NotificationRepository.GetPendingList();
        return Task.FromResult(itemList);
    }

    /// <inheritdoc />
    public Task<IList<NotificationRequest>> GetDeliveredNotificationList()
    {
        IList<NotificationRequest> itemList = NotificationRepository.GetDeliveredList();
        return Task.FromResult(itemList);
    }

    /// <inheritdoc />
    public Task<IList<ActiveNotification>> GetActiveNotifications()
    {
        // Windows does not expose an API for querying currently displayed notifications.
        return Task.FromResult<IList<ActiveNotification>>([]);
    }

    /// <inheritdoc />
    public Task<bool> AreNotificationsEnabled(NotificationPermission? permission = null)
    {
        try
        {
            var setting = AppNotificationManager.Default.Setting;
            return Task.FromResult(setting == AppNotificationSetting.Enabled);
        }
        catch
        {
            var supported = AppNotificationManager.IsSupported();
            return Task.FromResult(supported);
        }
    }

    /// <inheritdoc />
    public Task<NotificationPermissionStatus> GetNotificationPermissionStatus()
    {
        bool isEnabled;
        try
        {
            isEnabled = AppNotificationManager.Default.Setting == AppNotificationSetting.Enabled;
        }
        catch
        {
            isEnabled = AppNotificationManager.IsSupported();
        }

        return Task.FromResult(new NotificationPermissionStatus
        {
            IsEnabled = isEnabled,
            IsAlertEnabled = isEnabled,
            IsSoundEnabled = isEnabled,
            IsBadgeEnabled = isEnabled,
            CanScheduleExactAlarms = true
        });
    }

    /// <inheritdoc />
    public Task<bool> RequestNotificationPermission(NotificationPermission? permission = null)
    {
        try
        {
            AppNotificationManager.Default.Register();
        }
        catch (Exception ex)
        {
            LocalNotificationLogger.Log(ex);
            return Task.FromResult(false);
        }
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public void RegisterCategoryList(HashSet<NotificationCategory> categoryList)
    {
        if (categoryList is null || categoryList.Count == 0)
        {
            return;
        }

        foreach (var category in categoryList.Where(category =>
                     category.CategoryType != NotificationCategoryType.None))
        {
            _categoryList.Add(category);
        }
    }    
}
