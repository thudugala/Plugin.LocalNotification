﻿using Android.Content;
using AndroidX.Work;
using System.Threading.Tasks;

namespace Plugin.LocalNotification.Platform.Droid
{
#pragma warning disable CA1812
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

                NotificationCenter.Current.Show(notification);
            });

            return Result.InvokeSuccess();
        }
    }
}