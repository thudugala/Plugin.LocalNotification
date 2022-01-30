using System;
using Android.App;
using Android.Content;
using System.Linq;

namespace Plugin.LocalNotification.Platform.Droid
{
    [BroadcastReceiver(
        Name = ReceiverName,
        Enabled = true,
        Exported = false,
        Label = "Plugin LocalNotification Reboot complete Receiver")]
    [IntentFilter(
        new[] { Intent.ActionBootCompleted },
        Categories = new[] { Intent.CategoryHome })]
    internal class BootReceiver : BroadcastReceiver
    {
        /// <summary>
        ///
        /// </summary>
        public const string ReceiverName = "plugin.LocalNotification." + nameof(BootReceiver);

        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                if (intent.Action != Intent.ActionBootCompleted)
                {
                    NotificationCenter.Log("Not ActionBootCompleted");
                    return;
                }

                var requestList = NotificationRepository.Current.GetPendingList();
                if (requestList.Any() == false)
                {
                    NotificationCenter.Log("No Pending Notification Request");
                    return;
                }

                var activeForReScheduleRequestList = requestList.Where(r => r.IsStillActiveForReSchedule()).ToList();
                if (activeForReScheduleRequestList.Any() == false)
                {
                    NotificationCenter.Log("No Pending Notification Request that is Still Active For ReSchedule");
                    return;
                }

                var notificationService = TryGetDefaultDroidNotificationService();
                foreach (var request in activeForReScheduleRequestList)
                {
                    request.Schedule.NotifyTime = request.GetNextNotifyTime();

                    // re schedule again.
                    NotificationCenter.Log($"ReScheduled Notification Request {request.NotificationId}");
                    notificationService.ShowLater(request);
                }

                NotificationCenter.Log($"{nameof(BootReceiver)}-{nameof(OnReceive)}");
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