using Android.Content;
using Android.Runtime;
using AndroidX.Work;
using System;
using System.Threading.Tasks;

namespace Plugin.LocalNotification.Platform.Droid
{
#pragma warning disable CA1812
    [Preserve(AllMembers = true)]
    internal class ScheduledNotificationWorker : Worker
#pragma warning restore CA1812
    {
        public ScheduledNotificationWorker(Context context, WorkerParameters workerParameters) : base(context, workerParameters)
        {
        }

        public override Result DoWork()
        {
            var serializedNotification = InputData.GetString(NotificationCenter.ExtraReturnNotification);

            if (string.IsNullOrWhiteSpace(serializedNotification))
            {
                return Result.InvokeFailure();
            }

            Task.Run(() =>
            {
                var serializer = new ObjectSerializer<NotificationRequest>();
                var notification = serializer.DeserializeObject(serializedNotification);

                if (notification.NotifyTime.HasValue && notification.Repeats)
                {
                    // To be consistent with iOS, Schedule notification next daty same time.
                    notification.NotifyTime = notification.NotifyTime.Value.AddDays(1);
                    NotificationCenter.Current.Show(notification);
                }

                // To be consistent with iOS, Do not show notification if NotifyTime is earlier than DateTime.Now
                if (notification.NotifyTime.Value <= DateTime.Now) 
                {
                    return;
                }

                notification.NotifyTime = null;
                NotificationCenter.Current.Show(notification);
            });

            return Result.InvokeSuccess();
        }
    }
}