using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;
using Java.Lang;
using Plugin.LocalNotification.Core;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.Core.Models.AndroidOption;
using Plugin.LocalNotification.Core.Platforms.Android;
using Plugin.LocalNotification.EventArgs;
using Application = Android.App.Application;
using Exception = System.Exception;

namespace Plugin.LocalNotification.Platforms;

/// <inheritdoc />
internal class NotificationServiceImpl : IAndroidNotificationService
{
    private readonly List<NotificationCategory> _categoryList = [];

    /// <inheritdoc />
    public Func<NotificationRequest, Task<NotificationEventReceivingArgs>>? NotificationReceiving { get; set; }

    public bool IsSupported => OperatingSystem.IsAndroid();

    /// <summary>
    ///
    /// </summary>
    protected readonly NotificationManager? MyNotificationManager;

    /// <summary>
    ///
    /// </summary>
    protected readonly AlarmManager? MyAlarmManager;

    /// <inheritdoc />
    public event NotificationReceivedEventHandler? NotificationReceived;

    /// <inheritdoc />
    public event NotificationActionTappedEventHandler? NotificationActionTapped;

    /// <inheritdoc />
    public event NotificationDisabledEventHandler? NotificationsDisabled;

    /// <inheritdoc />
    public void OnNotificationActionTapped(NotificationActionEventArgs e)
    {
        NotificationActionTapped?.Invoke(e);
    }

    /// <inheritdoc />
    public void OnNotificationReceived(NotificationEventArgs e)
    {
        NotificationReceived?.Invoke(e);
    }

    /// <inheritdoc />
    public void OnNotificationsDisabled()
    {
        NotificationsDisabled?.Invoke();
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

    /// <summary>
    ///
    /// </summary>
    public NotificationServiceImpl()
    {
        try
        {
            MyNotificationManager = NotificationManager.FromContext(Application.Context);
            MyAlarmManager = AlarmManager.FromContext(Application.Context);

            GeofenceHandlerRegistry.ShowNotificationFromSerializedRequest = async (serializedRequest) =>
            {
                var request = LocalNotificationCenter.GetRequest(serializedRequest);
                return request is not null && LocalNotificationCenter.Current is NotificationServiceImpl androidService && await androidService.ShowNow(request);
            };
        }
        catch (Exception ex)
        {
            LocalNotificationLogger.Log(ex);
        }
    }

    /// <inheritdoc />
    public bool Cancel(params int[] notificationIdList)
    {
        foreach (var notificationId in notificationIdList)
        {
            var alarmPendingIntent = CreateAlarmIntent(notificationId, null);
            if (alarmPendingIntent != null)
            {
                MyAlarmManager?.Cancel(alarmPendingIntent);
                alarmPendingIntent.Cancel();
            }

            MyNotificationManager?.Cancel(notificationId);

            if (GeofenceHandlerRegistry.Handler is not null)
            {
                var geoPendingIntent = GeofenceHandlerRegistry.Handler.CreateGeofenceIntent(notificationId, null);
                GeofenceHandlerRegistry.Handler.RemoveGeofences(geoPendingIntent);
            }
        }

        NotificationRepository.RemoveByPendingIdList(notificationIdList);
        NotificationRepository.RemoveByDeliveredIdList(notificationIdList);
        return true;
    }

    /// <inheritdoc />
    public bool Cancel(int notificationId, string? tag)
    {
        var alarmPendingIntent = CreateAlarmIntent(notificationId, null);
        if (alarmPendingIntent != null)
        {
            MyAlarmManager?.Cancel(alarmPendingIntent);
            alarmPendingIntent.Cancel();
        }

        MyNotificationManager?.Cancel(tag, notificationId);

        if (GeofenceHandlerRegistry.Handler is not null)
        {
            var geoPendingIntent = GeofenceHandlerRegistry.Handler.CreateGeofenceIntent(notificationId, null);
            GeofenceHandlerRegistry.Handler.RemoveGeofences(geoPendingIntent);
        }

        NotificationRepository.RemoveByPendingIdList([notificationId]);
        NotificationRepository.RemoveByDeliveredIdList([notificationId]);
        return true;
    }

    /// <inheritdoc />
    public bool CancelAll()
    {
        var notificationIdList = NotificationRepository.GetPendingList().Select(r => r.NotificationId).ToList();
        foreach (var notificationId in notificationIdList)
        {
            var alarmPendingIntent = CreateAlarmIntent(notificationId, null);
            if (alarmPendingIntent != null)
            {
                MyAlarmManager?.Cancel(alarmPendingIntent);
                alarmPendingIntent.Cancel();
            }

            if (GeofenceHandlerRegistry.Handler is not null)
            {
                var geoPendingIntent = GeofenceHandlerRegistry.Handler.CreateGeofenceIntent(notificationId, null);
                GeofenceHandlerRegistry.Handler.RemoveGeofences(geoPendingIntent);
            }            
        }

        MyNotificationManager?.CancelAll();
        NotificationRepository.RemovePendingList();
        NotificationRepository.RemoveDeliveredList();
        return true;
    }

    /// <inheritdoc />
    public bool Clear(params int[] notificationIdList)
    {
        foreach (var notificationId in notificationIdList)
        {
            MyNotificationManager?.Cancel(notificationId);
        }

        NotificationRepository.RemoveByDeliveredIdList(notificationIdList);
        return true;
    }

    /// <inheritdoc />
    public bool ClearAll()
    {
        MyNotificationManager?.CancelAll();
        NotificationRepository.RemoveDeliveredList();
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> Show(NotificationRequest request)
    {
        var allowed = await AreNotificationsEnabled().ConfigureAwait(false);
        if (allowed == false)
        {
            OnNotificationsDisabled();
            LocalNotificationLogger.Log("Notifications are disabled");
            return false;
        }

        if (request is null)
        {
            return false;
        }

        if (request.Geofence.IsGeofence)
        {
            if (GeofenceHandlerRegistry.Handler is null)
            {
                LocalNotificationLogger.Log(Properties.Resources.GeofencePackageMissing);
                return false;
            }

            var serializedRequest = LocalNotificationCenter.GetRequestSerialize(request);
            return await GeofenceHandlerRegistry.Handler.ShowGeofence(request, serializedRequest);            
        }

        if (request.Schedule.NotifyTime.HasValue)
        {
            return ShowLater(request);
        }

        var result = await ShowNow(request);
        return result;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    internal virtual bool ShowLater(NotificationRequest request)
    {
        if (request.Schedule.Android.IsValidNotifyTime(DateTimeOffset.Now, request.Schedule.NotifyTime) == false)
        {
            LocalNotificationLogger.Log(
                "NotifyTime is earlier than (DateTimeOffset.Now - Allowed Delay), notification ignored");
            return false;
        }

        var serializedRequest = LocalNotificationCenter.GetRequestSerialize(request);
        var alarmIntent = CreateAlarmIntent(request.NotificationId, serializedRequest);
        if (alarmIntent is null)
        {
            LocalNotificationLogger.Log("Alarm Intent Not generated");
            return false;
        }

        var utcAlarmTimeInMillis =
            (request.Schedule.NotifyTime.GetValueOrDefault().ToUniversalTime() - DateTimeOffset.UtcNow)
            .TotalMilliseconds;
        var triggerTime = (long)utcAlarmTimeInMillis;

        var alarmType = request.Schedule.Android.AlarmType.ToNative();
        var triggerAtTime = GetBaseCurrentTime(alarmType) + triggerTime;

        ScheduleAlarm(request.Schedule.Android.ScheduleMode, alarmType, triggerAtTime, alarmIntent);

        NotificationRepository.AddPendingRequest(request);

        return true;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="notificationId"></param>
    /// <param name="serializedRequest"></param>
    /// <returns></returns>
    protected virtual PendingIntent? CreateAlarmIntent(int notificationId, string? serializedRequest)
    {
        var pendingIntent = AndroidUtils.CreateActionIntent(notificationId, serializedRequest, new NotificationAction(0)
        {
            Android =
            {
                LaunchAppWhenTapped = false,
                PendingIntentFlags = AndroidPendingIntentFlags.UpdateCurrent
            }
        }, typeof(ScheduledAlarmReceiver));
        return pendingIntent;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    protected virtual long GetBaseCurrentTime(AlarmType type)
    {
        return type switch
        {
            AlarmType.Rtc => JavaSystem.CurrentTimeMillis(),
            AlarmType.RtcWakeup => JavaSystem.CurrentTimeMillis(),
            AlarmType.ElapsedRealtime => SystemClock.ElapsedRealtime(),
            AlarmType.ElapsedRealtimeWakeup => SystemClock.ElapsedRealtime(),
            _ => throw new NotImplementedException(),
        };
    }

    /// <summary>
    /// Schedules an alarm using the <c>AlarmManager</c> API chosen by <paramref name="scheduleMode"/>.
    /// </summary>
    protected virtual void ScheduleAlarm(AndroidScheduleMode scheduleMode, AlarmType alarmType, long triggerAtTime, PendingIntent alarmIntent)
    {
        switch (scheduleMode)
        {
            case AndroidScheduleMode.Inexact:
                MyAlarmManager?.Set(alarmType, triggerAtTime, alarmIntent);
                break;

            case AndroidScheduleMode.InexactAllowWhileIdle:
                if (OperatingSystem.IsAndroidVersionAtLeast(23))
                {
                    MyAlarmManager?.SetAndAllowWhileIdle(alarmType, triggerAtTime, alarmIntent);
                }
                else
                {
                    MyAlarmManager?.Set(alarmType, triggerAtTime, alarmIntent);
                }

                break;

            case AndroidScheduleMode.Exact:
                if (OperatingSystem.IsAndroidVersionAtLeast(23))
                {
                    MyAlarmManager?.SetExactAndAllowWhileIdle(alarmType, triggerAtTime, alarmIntent);
                }
                else
                {
                    MyAlarmManager?.SetExact(alarmType, triggerAtTime, alarmIntent);
                }

                break;

            case AndroidScheduleMode.ExactAllowWhileIdle:
                if (OperatingSystem.IsAndroidVersionAtLeast(23))
                {
                    MyAlarmManager?.SetExactAndAllowWhileIdle(alarmType, triggerAtTime, alarmIntent);
                }
                else
                {
                    MyAlarmManager?.SetExact(alarmType, triggerAtTime, alarmIntent);
                }

                break;

            case AndroidScheduleMode.AlarmClock:
                var alarmClockInfo = new AlarmManager.AlarmClockInfo(triggerAtTime, alarmIntent);
                MyAlarmManager?.SetAlarmClock(alarmClockInfo, alarmIntent);
                break;

            default: // AndroidScheduleMode.Default
                var canScheduleExact = true;
                if (OperatingSystem.IsAndroidVersionAtLeast(31))
                {
                    canScheduleExact = MyAlarmManager?.CanScheduleExactAlarms() ?? false;
                }

                if (OperatingSystem.IsAndroidVersionAtLeast(23))
                {
                    if (canScheduleExact)
                    {
                        MyAlarmManager?.SetExactAndAllowWhileIdle(alarmType, triggerAtTime, alarmIntent);
                    }
                    else
                    {
                        MyAlarmManager?.SetAndAllowWhileIdle(alarmType, triggerAtTime, alarmIntent);
                    }
                }
                else
                {
                    if (canScheduleExact)
                    {
                        MyAlarmManager?.SetExact(alarmType, triggerAtTime, alarmIntent);
                    }
                    else
                    {
                        MyAlarmManager?.Set(alarmType, triggerAtTime, alarmIntent);
                    }
                }
                break;
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    internal virtual async Task<bool> ShowNow(NotificationRequest request)
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

        if (string.IsNullOrWhiteSpace(request.Android.ChannelId))
        {
            request.Android.ChannelId = AndroidOptions.DefaultChannelId;
        }

        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            var channel = MyNotificationManager?.GetNotificationChannel(request.Android.ChannelId);
            if (channel is null)
            {
                LocalNotificationCenter.CreateNotificationChannels(
                [
                    new() 
                    {
                        Id = request.Android.ChannelId
                    }
                ]);
            }
        }

        using var builder = new NotificationCompat.Builder(Application.Context, request.Android.ChannelId);

        builder.SetContentTitle(request.Title);
        builder.SetSubText(request.Subtitle);
        builder.SetContentText(request.Description);

        if(request.Android.When is not null)
        {
            builder.SetShowWhen(true);
            builder.SetWhen(request.Android.When.Value.ToUnixTimeMilliseconds());
        }

        if (request.Android.UsesChronometer)
        {
            builder.SetUsesChronometer(true);
            if (OperatingSystem.IsAndroidVersionAtLeast(24))
            {
                builder.SetChronometerCountDown(request.Android.ChronometerCountDown);
            }
        }

        if (request.Android.Colorized)
        {
            builder.SetColorized(true);
        }

        await ApplyStyle(builder, request).ConfigureAwait(false);

        builder.SetNumber(request.BadgeNumber);
        builder.SetAutoCancel(request.Android.AutoCancel);
        builder.SetOngoing(request.Android.Ongoing);

        if (string.IsNullOrWhiteSpace(request.Group) == false)
        {
            builder.SetGroup(request.Group);
            if (request.Android.IsGroupSummary)
            {
                builder.SetGroupSummary(true);
            }
        }

        if (request.CategoryType != NotificationCategoryType.None)
        {
            builder.SetCategory(request.CategoryType.ToNative());
        }

        builder.SetVisibility((int)request.Android.VisibilityType.ToNative());

        if (!OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            builder.SetPriority((int)request.Android.Priority);

            var soundUri = LocalNotificationCenter.GetSoundUri(request.Sound);
            if (soundUri is not null)
            {
                builder.SetSound(soundUri, request.Android.AudioAttributeUsage.ToStreamType());
            }
        }

        if (request.Android.VibrationPattern is not null)
        {
            if (!OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                builder.SetVibrate(request.Android.VibrationPattern);
            }
        }

        if (request.Android.ProgressBar is not null)
        {
            builder.SetProgress(request.Android.ProgressBar.Max,
                request.Android.ProgressBar.Progress,
                request.Android.ProgressBar.IsIndeterminate);
        }

        if (request.Android.Color is not null)
        {
            builder.SetColor(request.Android.Color.ToNative());
        }

        builder.SetSmallIcon(GetIcon(request.Android.IconSmallName));
        if (request.Android.IconLargeName is not null &&
            string.IsNullOrWhiteSpace(request.Android.IconLargeName.ResourceName) == false)
        {
            var largeIcon = await BitmapFactory.DecodeResourceAsync(Application.Context.Resources,
                GetIcon(request.Android.IconLargeName)).ConfigureAwait(false);
            if (largeIcon is not null)
            {
                builder.SetLargeIcon(largeIcon);
            }
        }

        if (request.Android.TimeoutAfter.HasValue)
        {
            builder.SetTimeoutAfter((long)request.Android.TimeoutAfter.Value.TotalMilliseconds);
        }

        var serializedRequest = LocalNotificationCenter.GetRequestSerialize(request);

        var contentIntent = AndroidUtils.CreateActionIntent(request.NotificationId, serializedRequest, new NotificationAction(NotificationActionEventArgs.TapActionId)
        {
            Android =
            {
                LaunchAppWhenTapped = (request.Android.LaunchApp is not null) || request.Android.LaunchAppWhenTapped,
                PendingIntentFlags = request.Android.PendingIntentFlags
            }
        }, typeof(NotificationActionReceiver));

        var deleteIntent = AndroidUtils.CreateActionIntent(request.NotificationId, serializedRequest, new NotificationAction(NotificationActionEventArgs.DismissedActionId)
        {
            Android =
            {
                LaunchAppWhenTapped = false,
                PendingIntentFlags = AndroidPendingIntentFlags.CancelCurrent
            }
        }, typeof(NotificationActionReceiver));

        if (request.Android.LaunchApp is not null)
        {
            builder.SetFullScreenIntent(contentIntent, request.Android.LaunchApp.InHighPriority);
        }
        else
        {
            builder.SetContentIntent(contentIntent);
        }
        builder.SetDeleteIntent(deleteIntent);

        if (_categoryList.Count != 0)
        {
            var categoryByType = _categoryList.FirstOrDefault(c => c.CategoryType == request.CategoryType);
            if (categoryByType != null)
            {
                foreach (var notificationAction in categoryByType.ActionList)
                {
                    var nativeAction = CreateAction(request, serializedRequest, notificationAction);
                    if (nativeAction is null)
                    {
                        continue;
                    }

                    builder.AddAction(nativeAction);
                }
            }
        }

        if (request.Silent)
        {
            builder.SetSilent(request.Silent);
        }

        var notification = builder.Build();
        if (notification is not null && !OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            if (request.Android.LedColor is not null)
            {
                var ledOn = request.Android.LedOnMs ?? 300;
                var ledOff = request.Android.LedOffMs ?? 1000;
                notification.LedARGB = request.Android.LedColor.Value;
                notification.LedOnMS = ledOn;
                notification.LedOffMS = ledOff;
                notification.Flags |= NotificationFlags.ShowLights;
            }
            if (string.IsNullOrWhiteSpace(request.Sound))
            {
                notification.Defaults = NotificationDefaults.All;
            }
        }
        if (!requestHandled)
        {
            MyNotificationManager?.Notify(request.NotificationId, notification);
        }
        else
        {
            LocalNotificationLogger.Log("NotificationServiceImpl.ShowNow: notification is Handled");
        }

        var args = new NotificationEventArgs
        {
            Request = request
        };
        LocalNotificationCenter.Current.OnNotificationReceived(args);
        NotificationRepository.AddDeliveredRequest(request);

        return true;
    }
            
    /// <summary>
    ///
    /// </summary>
    /// <param name="notificationImage"></param>
    /// <returns></returns>
    protected virtual async Task<Bitmap?> GetNativeImage(NotificationImage? notificationImage)
    {
        if (notificationImage is null || notificationImage.HasValue == false)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(notificationImage.ResourceName) == false)
        {
            var imageId = Application.Context.Resources?.GetIdentifier(notificationImage.ResourceName,
                AndroidIcon.DefaultType, Application.Context.PackageName);
            if (imageId != null)
            {
                return await BitmapFactory.DecodeResourceAsync(Application.Context.Resources, imageId.Value)
                    .ConfigureAwait(false);
            }
        }

        if (string.IsNullOrWhiteSpace(notificationImage.FilePath) == false)
        {
            if (File.Exists(notificationImage.FilePath))
            {
                return await BitmapFactory.DecodeFileAsync(notificationImage.FilePath).ConfigureAwait(false);
            }
        }

        return notificationImage.Binary is { Length: > 0 }
            ? await BitmapFactory.DecodeByteArrayAsync(notificationImage.Binary, 0,
                notificationImage.Binary.Length).ConfigureAwait(false)
            : null;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private async Task ApplyStyle(NotificationCompat.Builder builder, NotificationRequest request)
    {
        switch (request.Android.Style)
        {
            case AndroidInboxStyle inboxStyle:
            {
                using var nativeStyle = new NotificationCompat.InboxStyle();
                if (!string.IsNullOrEmpty(inboxStyle.ContentTitle))
                {
                    nativeStyle.SetBigContentTitle(inboxStyle.ContentTitle);
                }
                if (!string.IsNullOrEmpty(inboxStyle.SummaryText))
                {
                    nativeStyle.SetSummaryText(inboxStyle.SummaryText);
                }
                foreach (var line in inboxStyle.Lines)
                {
                    nativeStyle.AddLine(line);
                }
                builder.SetStyle(nativeStyle);
                return;
            }

            case AndroidMessagingStyle messagingStyle:
            {
                using var nativeStyle = new NotificationCompat.MessagingStyle(messagingStyle.Person.Name);
                if (!string.IsNullOrEmpty(messagingStyle.ContentTitle))
                {
                    nativeStyle.SetConversationTitle(new Java.Lang.String(messagingStyle.ContentTitle));
                }
                nativeStyle.SetGroupConversation(messagingStyle.IsGroupConversation);
                foreach (var message in messagingStyle.Messages)
                {
                    AndroidX.Core.App.Person? sender = null;
                    if (message.Person?.Name is { Length: > 0 } senderName)
                    {                            
                        sender = new AndroidX.Core.App.Person.Builder()?
                            .SetName(senderName)?
                            .SetBot(message.Person.IsBot)?
                            .SetImportant(message.Person.IsImportant)?
                            .Build();
                    }
                    nativeStyle.AddMessage(
                        new NotificationCompat.MessagingStyle.Message(
                            new Java.Lang.String(message.Text),
                            message.Timestamp.ToUnixTimeMilliseconds(),
                            sender));
                }
                builder.SetStyle(nativeStyle);
                return;
            }
        }

        // Default: BigPicture when an image is present, BigText when description is present.
        if (request.Image is { HasValue: true })
        {
            var imageBitmap = await GetNativeImage(request.Image).ConfigureAwait(false);
            if (imageBitmap != null)
            {
                using var picStyle = new NotificationCompat.BigPictureStyle();
                picStyle.BigPicture(imageBitmap);
                picStyle.SetSummaryText(request.Subtitle);
                builder.SetStyle(picStyle);
            }
        }
        else if (!string.IsNullOrWhiteSpace(request.Description))
        {
            using var bigTextStyle = new NotificationCompat.BigTextStyle();
            bigTextStyle.BigText(request.Description);
            bigTextStyle.SetSummaryText(request.Subtitle);
            builder.SetStyle(bigTextStyle);
        }
    }

    protected virtual NotificationCompat.Action? CreateAction(NotificationRequest request, string serializedRequest,
        NotificationAction action)
    {
        var pendingIntent = AndroidUtils.CreateActionIntent(request.NotificationId, serializedRequest, action, typeof(NotificationActionReceiver));
        if (string.IsNullOrWhiteSpace(action.Android.IconName.ResourceName))
        {
            action.Android.IconName = request.Android.IconSmallName;
        }

        var actionBuilder = new NotificationCompat.Action.Builder(
            GetIcon(action.Android.IconName),
            new Java.Lang.String(action.Title),
            pendingIntent);

        foreach (var input in action.Android.Inputs)
        {
            var remoteInputBuilder = new AndroidX.Core.App.RemoteInput.Builder(RequestConstants.RemoteInputKey);

            if (remoteInputBuilder is null)
            {
                continue;
            }
            remoteInputBuilder
                .SetLabel(input.Label)?
                .SetAllowFreeFormInput(input.AllowFreeFormInput);

            if (input.Choices is { Length: > 0 })
            {
                remoteInputBuilder.SetChoices(
                    input.Choices.Select(c => (Java.Lang.ICharSequence)new Java.Lang.String(c)).ToArray());
            }

            actionBuilder.AddRemoteInput(remoteInputBuilder.Build());
        }

        return actionBuilder.Build();
    }
        
    /// <summary>
    ///
    /// </summary>
    /// <param name="icon"></param>
    /// <returns></returns>
    protected static int GetIcon(AndroidIcon icon)
    {
        var iconId = 0;
        if (icon != null && string.IsNullOrWhiteSpace(icon.ResourceName) == false)
        {
            if (string.IsNullOrWhiteSpace(icon.Type))
            {
                icon.Type = AndroidIcon.DefaultType;
            }

            iconId = Application.Context.Resources?.GetIdentifier(icon.ResourceName, icon.Type,
                Application.Context.PackageName) ?? 0;
        }

        if (iconId != 0)
        {
            return iconId;
        }

        iconId = Application.Context.ApplicationInfo?.Icon ?? 0;
        if (iconId == 0)
        {
            iconId = Application.Context.Resources?.GetIdentifier("icon", AndroidIcon.DefaultType,
                Application.Context.PackageName) ?? 0;
        }

        return iconId;
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

    private NotificationPermission _notificationPermission = new();

    /// <inheritdoc />
    public Task<bool> AreNotificationsEnabled(NotificationPermission? permission = null)
    {
        _notificationPermission = permission is not null ? permission : new NotificationPermission();

        if (MyNotificationManager is null)
        {
            return Task.FromResult(false);
        }

        if(!OperatingSystem.IsAndroidVersionAtLeast(24))
        {
            return Task.FromResult(true);
        }

        if(!MyNotificationManager.AreNotificationsEnabled())
        {
            return Task.FromResult(false);
        }

        if (!OperatingSystem.IsAndroidVersionAtLeast(33))
        {
            return Task.FromResult(true);
        }

        if (_notificationPermission.Android.RequestPermissionToScheduleExactAlarm)
        {
            var canScheduleExactAlarms = MyAlarmManager?.CanScheduleExactAlarms() ?? false;

            return Task.FromResult(canScheduleExactAlarms);
        }

        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public async Task<bool> RequestNotificationPermission(NotificationPermission? permission = null)
    {
        _notificationPermission = permission is not null ? permission : new NotificationPermission();

        var allowed = await AreNotificationsEnabled(_notificationPermission);
        if (allowed)
        {
            return true;
        }

        if (!OperatingSystem.IsAndroidVersionAtLeast(33))
        {
            return true;
        }

        if (!_notificationPermission.AskPermission)
        {
            return false;
        }
                    
        var status = await Permissions.RequestAsync<NotificationPerms>();
        if (status != PermissionStatus.Granted)
        {
            return false;
        }

        LocalNotificationLogger.Log($"Request Permission To Schedule Exact Alarm: {_notificationPermission.Android.RequestPermissionToScheduleExactAlarm}");

        if (!_notificationPermission.Android.RequestPermissionToScheduleExactAlarm)
        {
            return true;
        }
                   
        var canScheduleExactAlarms = MyAlarmManager?.CanScheduleExactAlarms() ?? false;

        LocalNotificationLogger.Log($"Can Schedule Exact Alarms: {canScheduleExactAlarms}");

        if (!canScheduleExactAlarms)
        {
            var uri = Android.Net.Uri.Parse($"package:{Application.Context.PackageName}");
            var intent = new Intent(Android.Provider.Settings.ActionRequestScheduleExactAlarm, uri);
            intent.AddFlags(ActivityFlags.NewTask);
            Application.Context.StartActivity(intent);
        }
                   
        return canScheduleExactAlarms;
    }
        
    /// <inheritdoc />
    public Task<bool> CanScheduleExactNotifications()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(31))
        {
            return Task.FromResult(true);
        }

        var canSchedule = MyAlarmManager?.CanScheduleExactAlarms() ?? false;
        return Task.FromResult(canSchedule);
    }

    /// <inheritdoc />
    public async Task<bool> RequestExactAlarmsPermission()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(31))
        {
            return true;
        }

        if (MyAlarmManager?.CanScheduleExactAlarms() ?? false)
        {
            return true;
        }

        var uri = Android.Net.Uri.Parse($"package:{Application.Context.PackageName}");
        var intent = new Intent(Android.Provider.Settings.ActionRequestScheduleExactAlarm, uri);
        intent.AddFlags(ActivityFlags.NewTask);
        Application.Context.StartActivity(intent);

        // Return current state after surfacing the settings screen; 
        // the caller is responsible for re-checking after the user returns to the app.
        await Task.CompletedTask;
        return MyAlarmManager?.CanScheduleExactAlarms() ?? false;
    }

    /// <inheritdoc />
    public async Task<bool> RequestFullScreenIntentPermission()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(34))
        {
            return true;
        }

        if (MyNotificationManager?.CanUseFullScreenIntent() ?? false)
        {
            return true;
        }

        var intent = new Intent(Android.Provider.Settings.ActionManageAppUseFullScreenIntent);
        intent.SetData(Android.Net.Uri.Parse($"package:{Application.Context.PackageName}"));
        intent.AddFlags(ActivityFlags.NewTask);
        Application.Context.StartActivity(intent);

        await Task.CompletedTask;
        return MyNotificationManager?.CanUseFullScreenIntent() ?? false;
    }

    /// <inheritdoc />
    public Task DeleteNotificationChannel(string channelId)
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            MyNotificationManager?.DeleteNotificationChannel(channelId);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<IList<AndroidNotificationChannelRequest>> GetNotificationChannels()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            return Task.FromResult<IList<AndroidNotificationChannelRequest>>([]);
        }

        var native = MyNotificationManager?.NotificationChannels ?? [];
        IList<AndroidNotificationChannelRequest> result = [.. native
            .Select(c =>
            {
                return !OperatingSystem.IsAndroidVersionAtLeast(26)
                    ? null
                    : new AndroidNotificationChannelRequest
                {
                    Id = c.Id ?? string.Empty,
                    Name = c.Name?.ToString() ?? string.Empty,
                    Description = c.Description ?? string.Empty,
                    Importance = c.Importance.ToLocalNotificationImportance(),
                    Group = c.Group ?? string.Empty,
                };
            })
            .Where(c => c is not null)
            .Select(c => c!)];

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task DeleteNotificationChannelGroup(string groupId)
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            MyNotificationManager?.DeleteNotificationChannelGroup(groupId);
        }

        return Task.CompletedTask;
    }
}