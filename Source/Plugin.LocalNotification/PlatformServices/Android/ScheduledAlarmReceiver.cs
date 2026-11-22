using Android.App;
using Android.Content;
using Plugin.LocalNotification.Core;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.Core.Platforms.Android;

namespace Plugin.LocalNotification.Platforms;

[BroadcastReceiver(
    Name = ReceiverName,
    Enabled = true,
    Exported = false,
    Label = "Plugin LocalNotification Scheduled Alarm Receiver")]
[IntentFilter(
    [
        Intent.ActionBootCompleted,
        Intent.ActionMyPackageReplaced,
        "android.intent.action.QUICKBOOT_POWERON",
        "com.htc.intent.action.QUICKBOOT_POWERON"
    ],
    Categories = new[]
    {
        Intent.CategoryHome
    })]
internal class ScheduledAlarmReceiver : BroadcastReceiver
{
    /// <summary>
    ///
    /// </summary>
    public const string ReceiverName = "plugin.LocalNotification." + nameof(ScheduledAlarmReceiver);

    public override async void OnReceive(Context? context, Intent? intent)
    {
        try
        {
            var notificationService = TryGetDefaultDroidNotificationService();

            if (intent is null)
            {
                LocalNotificationLogger.Log("No intent");
                return;
            }

            if (intent.Action is Intent.ActionBootCompleted or
                Intent.ActionMyPackageReplaced or
                "android.intent.action.QUICKBOOT_POWERON" or
                "com.htc.intent.action.QUICKBOOT_POWERON")
            {
                LocalNotificationLogger.Log("ActionBootCompleted");

                var requestList = NotificationRepository.GetPendingList();
                if (requestList.Count <= 0)
                {
                    LocalNotificationLogger.Log("No Pending Notification Request");
                    return;
                }

                foreach (var request in requestList)
                {
                    await SendNotification(notificationService, request);
                }
            }
            else
            {
                var requestSerialize = intent.GetStringExtra(RequestConstants.ReturnRequest);
                if (string.IsNullOrWhiteSpace(requestSerialize))
                {
                    LocalNotificationLogger.Log("Request Json Not Found");
                    return;
                }
                var request = LocalNotificationCenter.GetRequest(requestSerialize);

                await SendNotification(notificationService, request);
            }
        }
        catch (Exception ex)
        {
            LocalNotificationLogger.Log(ex);
        }
    }

    private static async Task SendNotification(NotificationServiceImpl notificationService, NotificationRequest request)
    {
        if (request is null)
        {
            LocalNotificationLogger.Log("Request Not Found");
            return;
        }

        if (request.Schedule.NotifyTime is null)
        {
            LocalNotificationLogger.Log($"Notification {request.NotificationId} has no NotifyTime");
            return;
        }

        if (request.Schedule.NotifyAutoCancelTime <= DateTimeOffset.Now)
        {
            _ = notificationService.Cancel(request.NotificationId);
            LocalNotificationLogger.Log($"Notification {request.NotificationId} Auto Canceled");
            return;
        }

        var timeNow = DateTimeOffset.Now;
        var wasReScheduled = false;
        if (request.Schedule.Android.IsValidNotifyTime(timeNow, request.Schedule.NotifyTime))
        {
            if (request.Schedule.Android.IsValidShowNowTime(timeNow, request.Schedule.NotifyTime))
            {
                _ = await notificationService.ShowNow(request);

                if (request.Schedule.RepeatType == NotificationRepeat.No)
                {
                    NotificationRepository.RemoveByPendingIdList(request.NotificationId);
                }
            }
            else if (request.Schedule.Android.IsValidShowLaterTime(timeNow, request.Schedule.NotifyTime))
            {
                // schedule again.
                wasReScheduled = true;
                _ = notificationService.ShowLater(request);
            }
        }
        else
        {
            LocalNotificationLogger.Log(
                "NotifyTime is earlier than (DateTime.Now - Allowed Delay), notification ignored");
        }

        // even if the request is too old to show, if it is a Repeating notification, then reschedule again
        if (wasReScheduled == false && request.Schedule.RepeatType != NotificationRepeat.No)
        {
            request.Schedule.NotifyTime = request.Schedule.Android.GetNextNotifyTimeForRepeat(
                request.Schedule.NotifyTime,
                request.Schedule.RepeatType,
                request.Schedule.NotifyRepeatInterval);

            if (request.Schedule.NotifyTime.HasValue)
            {
                // reschedule again.
                _ = notificationService.ShowLater(request);
            }
            else
            {
                LocalNotificationLogger.Log(
                    $"Notification {request.NotificationId} New NotifyTime is null, no reschedule");
            }
        }
    }

    private static NotificationServiceImpl TryGetDefaultDroidNotificationService() => LocalNotificationCenter.Current is NotificationServiceImpl notificationService
            ? notificationService
            : new NotificationServiceImpl();
}