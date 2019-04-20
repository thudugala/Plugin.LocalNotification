using Android.App;
using Android.App.Job;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using System;

namespace Plugin.LocalNotification.Platform.Droid
{
    /// <inheritdoc />
    [Android.Runtime.Preserve]
    public class NotificationServiceImpl : INotificationService
    {
        private readonly NotificationManager _notificationManager;
        private readonly JobScheduler _jobScheduler;

        /// <inheritdoc />
        public event NotificationTappedEventHandler NotificationTapped;

        /// <inheritdoc />
        public void OnNotificationTapped(NotificationTappedEventArgs e)
        {
            NotificationTapped?.Invoke(e);
        }

        /// <inheritdoc />
        public NotificationServiceImpl()
        {
            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                {
                    return;
                }

                _notificationManager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;
                _jobScheduler = Application.Context.GetSystemService(Context.JobSchedulerService) as JobScheduler;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        /// <inheritdoc />
        public void Cancel(int notificationId)
        {
            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                {
                    return;
                }

                _jobScheduler.Cancel(notificationId);
                _notificationManager.Cancel(notificationId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        /// <inheritdoc />
        public void CancelAll()
        {
            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                {
                    return;
                }

                _jobScheduler.CancelAll();
                _notificationManager.CancelAll();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        /// <inheritdoc />
        public void Show(NotificationRequest notificationRequest)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
            {
                return;
            }

            if (notificationRequest is null)
            {
                return;
            }

            if (notificationRequest.NotifyTime.HasValue)
            {
                ShowLater(notificationRequest);
            }
            else
            {
                ShowNow(notificationRequest);
            }
        }

        private void ShowLater(NotificationRequest notificationRequest)
        {
            if (notificationRequest.NotifyTime.HasValue == false)
            {
                return;
            }

            var triggerTime = NotifyTimeInMilliseconds(notificationRequest.NotifyTime.Value);

            notificationRequest.NotifyTime = null;

            var serializedNotification = ObjectSerializer<NotificationRequest>.SerializeObject(notificationRequest);

            var javaClass = Java.Lang.Class.FromType(typeof(ScheduledNotificationJobService));
            var component = new ComponentName(Application.Context, javaClass);

            // Bundle up parameters
            var extras = new PersistableBundle();
            extras.PutString(NotificationCenter.ExtraReturnNotification, serializedNotification);

            triggerTime -= NotifyTimeInMilliseconds(DateTime.Now);

            var builder = new JobInfo.Builder(notificationRequest.NotificationId, component);
            builder.SetMinimumLatency(triggerTime); // Fire at TriggerTime
            builder.SetOverrideDeadline(triggerTime + 5000); // Or at least 5 Seconds Later
            builder.SetExtras(extras);
            builder.SetPersisted(CheckBootPermission()); //Job will be recreated after Reboot if Permissions are granted

            var jobInfo = builder.Build();
            _jobScheduler.Schedule(jobInfo);
        }

        private static bool CheckBootPermission()
        {
            const string permission = "android.permission.RECEIVE_BOOT_COMPLETED";
            return Build.VERSION.SdkInt >= BuildVersionCodes.M
                ? Application.Context.CheckSelfPermission(permission)
                  == Permission.Granted
                : PermissionChecker.CheckSelfPermission(Application.Context, permission)
                  == PermissionChecker.PermissionGranted;
        }

        private static long NotifyTimeInMilliseconds(DateTime notifyTime)
        {
            var utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
            var epochDifference = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;

            var utcAlarmTimeInMillis = utcTime.AddSeconds(-epochDifference).Ticks / 10000;

            return utcAlarmTimeInMillis;
        }

        private void ShowNow(NotificationRequest notificationRequest)
        {
            try
            {
                var channelId = $"{Application.Context.PackageName}.general";

                var builder = new NotificationCompat.Builder(Application.Context, channelId);
                builder.SetContentTitle(notificationRequest.Title);
                builder.SetContentText(notificationRequest.Description);
                builder.SetStyle(new NotificationCompat.BigTextStyle().BigText(notificationRequest.Description));
                builder.SetNumber(notificationRequest.BadgeNumber);
                builder.SetAutoCancel(notificationRequest.Android.AutoCancel);
                builder.SetPriority((int)notificationRequest.Android.Priority);

                if( notificationRequest.Android.Color.HasValue)
                {
                    builder.SetColor(notificationRequest.Android.Color.Value);
                }

                if (notificationRequest.Android.TimeoutAfter.HasValue)
                {
                    builder.SetTimeoutAfter((long)notificationRequest.Android.TimeoutAfter.Value.TotalMilliseconds);
                }

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var importance = NotificationImportance.Default;
                    switch (notificationRequest.Android.Priority)
                    {
                        case NotificationPriority.Min:
                            importance = NotificationImportance.Min;
                            break;

                        case NotificationPriority.Low:
                            importance = NotificationImportance.Low;
                            break;

                        case NotificationPriority.High:
                            importance = NotificationImportance.High;
                            break;

                        case NotificationPriority.Max:
                            importance = NotificationImportance.Max;
                            break;
                    }
                    var channel = new NotificationChannel(channelId, "General", importance);

                    _notificationManager.CreateNotificationChannel(channel);

                    builder.SetChannelId(channelId);
                }

                var notificationIntent = Application.Context.PackageManager.GetLaunchIntentForPackage(Application.Context.PackageName);
                notificationIntent.SetFlags(ActivityFlags.SingleTop);

                notificationIntent.PutExtra(NotificationCenter.ExtraReturnDataAndroid, notificationRequest.ReturningData);

                var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
                builder.SetContentIntent(pendingIntent);

                if (NotificationCenter.NotificationIconId != 0)
                {
                    builder.SetSmallIcon(NotificationCenter.NotificationIconId);
                }
                else
                {
                    var iconId = Application.Context.ApplicationInfo.Icon;
                    if (iconId == 0)
                    {
                        Application.Context.Resources.GetIdentifier("Icon", "drawable",
                            Application.Context.PackageName);
                    }
                    builder.SetSmallIcon(iconId);
                }

                var notification = builder.Build();

                if( notificationRequest.Android.LedColor.HasValue )
                {
                    notification.LedARGB = notificationRequest.Android.LedColor.Value;
                }

                notification.Defaults = NotificationDefaults.All;

                _notificationManager.Notify(notificationRequest.NotificationId, notification);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}