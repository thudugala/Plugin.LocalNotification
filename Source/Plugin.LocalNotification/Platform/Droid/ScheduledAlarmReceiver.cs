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
        new[] { Intent.ActionBootCompleted },
        Categories = new[] { Intent.CategoryHome })]
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

                if (intent.Action == Intent.ActionBootCompleted)
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

            if (request.Schedule.NotifyAutoCancelTime.HasValue &&
                request.Schedule.NotifyAutoCancelTime <= DateTime.Now)
            {
                notificationService.Cancel(request.NotificationId);
                NotificationCenter.Log($"Notification {request.NotificationId} Auto Canceled");
                return;
            }

            if (request.Schedule.NotifyTime.HasValue &&
                request.Schedule.RepeatType != NotificationRepeat.No)
            {
                request.Schedule.NotifyTime = request.GetNextNotifyTime();
                if (request.Schedule.NotifyTime.HasValue)
                {
                    // schedule again.
                    notificationService.ShowLater(request);
                }
            }

            // To be consistent with iOS, Do not show notification if NotifyTime is earlier than (DateTime.Now + AllowedDelay)
            if (request.Schedule.AndroidIsValidNotifyTime)
            {
                await notificationService.ShowNow(request);
            }
            else
            {
                NotificationCenter.Log("NotifyTime is earlier than DateTime.Now and Allowed Delay, notification ignored");
            }

            if (request.Schedule.NotifyTime.HasValue &&
                request.Schedule.RepeatType == NotificationRepeat.No)
            {
                NotificationRepository.Current.RemoveByPendingIdList(request.NotificationId);
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