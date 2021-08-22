using Android.App;
using Android.Content;
using System.Linq;

namespace Plugin.LocalNotification.Platform.Droid
{
    [BroadcastReceiver(Enabled = true, Label = "Plugin LocalNotification Reboot complete Receiver")]
    [IntentFilter(
        new[] { Intent.ActionBootCompleted },
        Categories = new[] { Intent.CategoryHome })]
    internal class BootReceiver : BroadcastReceiver
    {
        public override async void OnReceive(Context context, Intent intent)
        {
            if (intent.Action != Intent.ActionBootCompleted)
            {
                return;
            }

            var requestList = NotificationRepository.Current.GetPendingList();
            if(requestList.Any() == false)
            {
                return;
            }
            var activeForReScheduleRequestList = requestList.Where(r => r.IsStillActiveForReSchedule()).ToList();
            if (activeForReScheduleRequestList.Any() == false)
            {
                return;
            }

            var notificationService = TryGetDefaultDroidNotificationService();
            foreach (var request in activeForReScheduleRequestList)
            {
                request.Schedule.NotifyTime = request.GetNextNotifyTime();

                // re schedule again.
                await notificationService.Show(request);
            }

            Android.Util.Log.Info(
                Application.Context.PackageName,
                $"{nameof(BootReceiver)}-{nameof(OnReceive)}");
        }

        private static NotificationServiceImpl TryGetDefaultDroidNotificationService()
        {
            return NotificationCenter.Current is NotificationServiceImpl notificationService
                ? notificationService
                : new NotificationServiceImpl();
        }
    }
}