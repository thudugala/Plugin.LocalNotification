using Android.Content;
using System;

namespace Plugin.LocalNotification.Platform.Droid
{
    [BroadcastReceiver(
        Name = ReceiverName,
        Enabled = true,
        Exported = false
    )]
    internal class ScheduledAlarmReceiver : BroadcastReceiver
    {
        /// <summary>
        ///
        /// </summary>
        public const string ReceiverName = "Plugin.LocalNotification." + nameof(ScheduledAlarmReceiver);

        public override async void OnReceive(Context context, Intent intent)
        {
            try
            {
                var requestSerialize = intent.GetStringExtra(NotificationCenter.ReturnRequest);

                if (string.IsNullOrWhiteSpace(requestSerialize))
                {
                    System.Diagnostics.Debug.WriteLine("Request Json Not Found");
                    return;
                }

                var notification = NotificationCenter.GetRequest(requestSerialize);
                if (notification is null)
                {
                    System.Diagnostics.Debug.WriteLine("Request Not Found");
                    return;
                }

                if (notification.Schedule.NotifyTime.HasValue &&
                    notification.Schedule.RepeatType != NotificationRepeat.No)
                {
                    var notificationService = TryGetDefaultDroidNotificationService();

                    if (notification.Schedule.NotifyAutoCancelTime.HasValue &&
                        notification.Schedule.NotifyAutoCancelTime <= DateTime.Now)
                    {
                        notificationService.Cancel(notification.NotificationId);
                        System.Diagnostics.Debug.WriteLine("Notification Auto Canceled");
                        return;
                    }
                }
                
                notification.Schedule.RepeatType = NotificationRepeat.No;
                // To be consistent with iOS, Do not show notification if NotifyTime is earlier than DateTime.Now
                //if (notification.Schedule.NotifyTime != null &&
                //    notification.Schedule.NotifyTime.Value <= DateTime.Now.AddMinutes(-1))
                //{
                //    System.Diagnostics.Debug.WriteLine("NotifyTime is earlier than DateTime.Now, notification ignored");
                //    return;
                //}

                notification.Schedule.NotifyTime = null;
                await NotificationCenter.Current.Show(notification);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
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