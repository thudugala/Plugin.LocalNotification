using Android.Content;
using AndroidX.Work;
using System.Threading.Tasks;

namespace Plugin.LocalNotification.Platform.Droid
{
    internal class ScheduledNotificationWorker : Worker
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
                var notification = ObjectSerializer<NotificationRequest>.DeserializeObject(serializedNotification);

                NotificationCenter.Current.Show(notification);
            });

            return Result.InvokeSuccess();
        }
    }
}