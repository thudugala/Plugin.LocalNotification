using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Util;
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
        public ScheduledNotificationWorker(Context context, WorkerParameters workerParameters) : base(context,
            workerParameters)
        {
        }

        public override Result DoWork()
        {
            var serializedNotification = InputData.GetString(NotificationCenter.ExtraReturnNotification);

            if (string.IsNullOrWhiteSpace(serializedNotification))
            {
                return Result.InvokeFailure();
            }

            var serializedNotificationAndroid = InputData.GetString($"{NotificationCenter.ExtraReturnNotification}_Android");

            Task.Run(() =>
            {
                try
                {
                    Log.Info(Application.Context.PackageName, $"ScheduledNotificationWorker.DoWork: SerializedNotification [{serializedNotification}]");
                    var notification = ObjectSerializer.DeserializeObject<NotificationRequest>(serializedNotification);
                    if (string.IsNullOrWhiteSpace(serializedNotificationAndroid) == false)
                    {
                        var notificationAndroid =
                            ObjectSerializer.DeserializeObject<AndroidOptions>(serializedNotificationAndroid);
                        if (notificationAndroid != null)
                        {
                            notification.Android = notificationAndroid;
                        }
                    }

                    if (notification.NotifyTime.HasValue && notification.Repeats != NotificationRepeat.No)
                    {
                        switch (notification.Repeats)
                        {
                            case NotificationRepeat.Daily:
                                // To be consistent with iOS, Schedule notification next day same time.
                                notification.NotifyTime = notification.NotifyTime.Value.AddDays(1);
                                while (notification.NotifyTime <= DateTime.Now)
                                {
                                    notification.NotifyTime = notification.NotifyTime.Value.AddDays(1);
                                }

                                break;

                            case NotificationRepeat.Weekly:
                                // To be consistent with iOS, Schedule notification next week same day same time.
                                notification.NotifyTime = notification.NotifyTime.Value.AddDays(7);
                                while (notification.NotifyTime <= DateTime.Now)
                                {
                                    notification.NotifyTime = notification.NotifyTime.Value.AddDays(7);
                                }

                                break;
                        }

                        NotificationCenter.Current.Show(notification);
                    }

                    // To be consistent with iOS, Do not show notification if NotifyTime is earlier than DateTime.Now
                    if (notification.NotifyTime != null && notification.NotifyTime.Value <= DateTime.Now.AddMinutes(-1))
                    {
                        System.Diagnostics.Debug.WriteLine("NotifyTime is earlier than DateTime.Now, notification ignored");
                        return;
                    }

                    notification.NotifyTime = null;
                    NotificationCenter.Current.Show(notification);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            });

            return Result.InvokeSuccess();
        }
    }
}