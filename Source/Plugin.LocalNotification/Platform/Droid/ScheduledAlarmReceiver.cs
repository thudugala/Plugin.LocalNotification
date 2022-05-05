using Android.App;
using Android.Content;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.LocalNotification.Platform.Droid
{
    [BroadcastReceiver(
        Name = ReceiverName,
        Enabled = true,
        Exported = false,
        Label = "Plugin LocalNotification Scheduled Alarm Receiver")]
    [IntentFilter(
        new[] 
        { 
            Intent.ActionBootCompleted, 
            Intent.ActionMyPackageReplaced, 
            "android.intent.action.QUICKBOOT_POWERON", 
            "com.htc.intent.action.QUICKBOOT_POWERON" 
        },
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

        public override async void OnReceive(Context context, Intent intent)
        {
            try
            {
                var notificationService = TryGetDefaultDroidNotificationService();

                if (intent.Action == Intent.ActionBootCompleted ||
                    intent.Action == Intent.ActionMyPackageReplaced ||
                    intent.Action == "android.intent.action.QUICKBOOT_POWERON" ||
                    intent.Action == "com.htc.intent.action.QUICKBOOT_POWERON")
                {
                    NotificationCenter.Log("ActionBootCompleted");

                    var requestList = NotificationRepository.Current.GetPendingList();
                    if (requestList.Any() == false)
                    {
                        NotificationCenter.Log("No Pending Notification Request");
                        return;
                    }

                    foreach (var request in requestList)
                    {
                        await SendNotification(notificationService, request);
                    }
                }
                else
                {
                    var requestSerialize = intent.GetStringExtra(NotificationCenter.ReturnRequest);
                    if (string.IsNullOrWhiteSpace(requestSerialize))
                    {
                        NotificationCenter.Log("Request Json Not Found");
                        return;
                    }
                    var request = NotificationCenter.GetRequest(requestSerialize);

                    await SendNotification(notificationService, request);
                }
            }
            catch (Exception ex)
            {
                NotificationCenter.Log(ex);
            }
        }

        private async Task SendNotification(NotificationServiceImpl notificationService, NotificationRequest request)
        {
            if (request is null)
            {
                NotificationCenter.Log("Request Not Found");
                return;
            }

            if (request.Schedule.NotifyTime is null)
            {
                NotificationCenter.Log($"Notification {request.NotificationId} has no NotifyTime");
                return;
            }

            if (request.Schedule.NotifyAutoCancelTime <= DateTime.Now)
            {
                notificationService.Cancel(request.NotificationId);
                NotificationCenter.Log($"Notification {request.NotificationId} Auto Canceled");
                return;
            }

            var timeNow = DateTime.Now;
            var wasReScheduled = false;
            if (request.Schedule.Android.IsValidNotifyTime(timeNow, request.Schedule.NotifyTime))
            {
                if (request.Schedule.Android.IsValidShowNowTime(timeNow, request.Schedule.NotifyTime))
                {
                    await notificationService.ShowNow(request);

                    if (request.Schedule.RepeatType == NotificationRepeat.No)
                    {
                        NotificationRepository.Current.RemoveByPendingIdList(request.NotificationId);
                    }
                }
                else if (request.Schedule.Android.IsValidShowLaterTime(timeNow, request.Schedule.NotifyTime))
                {
                    // schedule again.
                    wasReScheduled = true;
                    notificationService.ShowLater(request);
                }
            }
            else
            {
                NotificationCenter.Log(
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
                    notificationService.ShowLater(request);
                }
                else
                {
                    NotificationCenter.Log(
                        $"Notification {request.NotificationId} New NotifyTime is null, no reschedule");
                }
            }
        }

        private static NotificationServiceImpl TryGetDefaultDroidNotificationService()
        {
            return NotificationCenter.Current is NotificationServiceImpl notificationService
                ? notificationService
                : new NotificationServiceImpl();
        }
    }
}