using Android.App;
using Android.Content;
using System;

namespace Plugin.LocalNotification.Platform.Droid
{
    [BroadcastReceiver(
        Name = ReceiverName,
        Enabled = true,
        Exported = false,
        Label = "Plugin LocalNotification Scheduled Alarm Receiver"
    )]
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
                var requestSerialize = intent.GetStringExtra(NotificationCenter.ReturnRequest);

                if (string.IsNullOrWhiteSpace(requestSerialize))
                {
                    NotificationCenter.Log("ScheduledAlarmReceiver.OnReceive: Request Json Not Found");
                    return;
                }

                var request = NotificationCenter.GetRequest(requestSerialize);
                if (request is null)
                {
                    NotificationCenter.Log("ScheduledAlarmReceiver.OnReceive: Request Not Found");
                    return;
                }

                var notificationService = TryGetDefaultDroidNotificationService();

                if (request.Schedule.NotifyTime.HasValue &&
                    request.Schedule.RepeatType != NotificationRepeat.No)
                {
                    if (request.Schedule.NotifyAutoCancelTime.HasValue &&
                        request.Schedule.NotifyAutoCancelTime <= DateTime.Now)
                    {
                        notificationService.Cancel(request.NotificationId);
                        NotificationCenter.Log("ScheduledAlarmReceiver.OnReceive: Notification Auto Canceled");
                        return;
                    }

                    request.Schedule.NotifyTime = request.GetNextNotifyTime();
                    if (request.Schedule.NotifyTime.HasValue)
                    {
                        // schedule again.
                        notificationService.ShowLater(request);
                    }
                }

                // To be consistent with iOS, Do not show notification if NotifyTime is earlier than DateTime.Now
                if (request.Schedule.AndroidIsValidNotifyTime == false)
                {
                    NotificationCenter.Log("ScheduledAlarmReceiver.OnReceive: NotifyTime is earlier than DateTime.Now and Allowed Delay, notification ignored");
                    return;
                }
                                
                await notificationService.ShowNow(request);
            }
            catch (Exception ex)
            {
                NotificationCenter.Log(ex);
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