using Android.Content;
using Android.Runtime;
using AndroidX.Work;
using System;
using System.Threading.Tasks;

namespace Plugin.LocalNotification.Platform.Droid
{
    [Preserve(AllMembers = true)]
    internal class ScheduledNotificationWorker : Worker
    {
        public ScheduledNotificationWorker(Context context, WorkerParameters workerParameters) : base(context,
            workerParameters)
        {
        }

        public override Result DoWork()
        {
            var requestSerialize = InputData.GetString(NotificationCenter.ReturnRequest);

            if (string.IsNullOrWhiteSpace(requestSerialize))
            {
                return Result.InvokeFailure();
            }

            var notification = NotificationCenter.GetRequest(requestSerialize);
            if (notification is null)
            {
                return Result.InvokeFailure();
            }

            Task.Run(() =>
            {
                try
                {
                    if (notification.Schedule.NotifyTime.HasValue && notification.Schedule.RepeatType != NotificationRepeat.No)
                    {
                        switch (notification.Schedule.RepeatType)
                        {
                            case NotificationRepeat.Daily:
                                // To be consistent with iOS, Schedule notification next day same time.
                                notification.Schedule.NotifyTime = notification.Schedule.NotifyTime.Value.AddDays(1);
                                break;

                            case NotificationRepeat.Weekly:
                                // To be consistent with iOS, Schedule notification next week same day same time.
                                notification.Schedule.NotifyTime = notification.Schedule.NotifyTime.Value.AddDays(7);
                                break;

                            case NotificationRepeat.TimeInterval:
                                if (notification.Schedule.NotifyRepeatInterval.HasValue)
                                {
                                    TimeSpan interval = notification.Schedule.NotifyRepeatInterval.Value;
                                    notification.Schedule.NotifyTime = notification.Schedule.NotifyTime.Value.Add(interval);
                                }
                                break;
                        }

                        var notificationService = TryGetDefaultDroidNotificationService();

                        if (notification.Schedule.NotifyAutoCancelTime.HasValue && notification.Schedule.NotifyAutoCancelTime <= DateTime.Now)
                        {
                            notificationService.Cancel(notification.NotificationId);
                            System.Diagnostics.Debug.WriteLine("Notification Auto Canceled");
                            return;
                        }

                        notificationService.ShowNow(notification);
                        notificationService.EnqueueWorker(notification);
                        return;
                    }

                    // To be consistent with iOS, Do not show notification if NotifyTime is earlier than DateTime.Now
                    if (notification.Schedule.NotifyTime != null && notification.Schedule.NotifyTime.Value <= DateTime.Now.AddMinutes(-1))
                    {
                        System.Diagnostics.Debug.WriteLine("NotifyTime is earlier than DateTime.Now, notification ignored");
                        return;
                    }

                    notification.Schedule.NotifyTime = null;
                    NotificationCenter.Current.Show(notification);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                }
            });

            return Result.InvokeSuccess();
        }

        private static NotificationServiceImpl TryGetDefaultDroidNotificationService()
        {
            return NotificationCenter.Current is NotificationServiceImpl notificationService
                ? notificationService
                : new NotificationServiceImpl();
        }
    }
}