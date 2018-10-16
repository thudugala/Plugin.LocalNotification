using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Plugin.LocalNotification.Platform.Droid;
using System;

[assembly: Xamarin.Forms.Dependency(typeof(LocalNotificationService))]
namespace Plugin.LocalNotification.Platform.Droid
{
    /// <inheritdoc />
    [Android.Runtime.Preserve]
    public class LocalNotificationService : ILocalNotificationService
    {
        /// <summary>
        /// Return Data Key.
        /// </summary>
        internal static string ExtraReturnData = "Plugin.LocalNotification.Platform.Android.RETURN_DATA";

        /// <summary>
        /// Return Notification Key.
        /// </summary>
        internal static string ExtraReturnNotification = "Plugin.LocalNotification.Platform.Android.RETURN_NOTIFICATION";

        /// <summary>
        /// Get or Set Resource Icon to display.
        /// </summary>
        public static int NotificationIconId { get; set; }

        private readonly NotificationManager _notificationManager;
        private readonly AlarmManager _alarmManager;
        private readonly JobScheduler _jobScheduler;

        /// <inheritdoc />
        public LocalNotificationService()
        {
            try
            {
                _notificationManager = Application.Context.GetSystemService(Android.Content.Context.NotificationService) as NotificationManager;
                _alarmManager = Application.Context.GetSystemService(Android.Content.Context.AlarmService) as AlarmManager;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    _jobScheduler = Application.Context.GetSystemService(Android.Content.Context.JobSchedulerService) as JobScheduler;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Notify Local Notification Tapped.
        /// </summary>
        /// <param name="intent"></param>
        public static void NotifyNotificationTapped(Intent intent)
        {
            try
            {
                if (intent.HasExtra(ExtraReturnData) == false)
                {
                    return;
                }

                var subscribeItem = new LocalNotificationTappedEvent
                {
                    Data = intent.GetStringExtra(ExtraReturnData)
                };
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    Xamarin.Forms.MessagingCenter.Instance.Send(subscribeItem, typeof(LocalNotificationTappedEvent).FullName);
                });
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
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    _jobScheduler.Cancel(notificationId);
                }
                else
                {
                    var notificationIntent = new Intent(Application.Context, typeof(ScheduledNotificationReceiver));
                    var pendingIntent = PendingIntent.GetBroadcast(Application.Context, 0, notificationIntent, PendingIntentFlags.CancelCurrent);

                    _alarmManager.Cancel(pendingIntent);
                }
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
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    _jobScheduler.CancelAll();
                }
                else
                {
                    var notificationIntent = new Intent(Application.Context, typeof(ScheduledNotificationReceiver));
                    var pendingIntent = PendingIntent.GetBroadcast(Application.Context, 0, notificationIntent, PendingIntentFlags.CancelCurrent);

                    _alarmManager.Cancel(pendingIntent);
                }
                _notificationManager.CancelAll();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        /// <inheritdoc />
        public void Show(LocalNotification localNotification)
        {
            if (localNotification is null)
            {
                return;
            }

            if (localNotification.NotifyTime.HasValue)
            {
                ShowLater(localNotification);
            }
            else
            {
                ShowNow(localNotification);
            }
        }

        private void ShowLater(LocalNotification localNotification)
        {
            if (localNotification.NotifyTime.HasValue == false)
            {
                return;
            }

            var triggerTime = NotifyTimeInMilliseconds(localNotification.NotifyTime.Value);

            localNotification.NotifyTime = null;

            var serializedNotification = ObjectSerializer<LocalNotification>.SerializeObject(localNotification);

            var scheduled = false;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                var javaClass = Java.Lang.Class.FromType(typeof(ScheduledNotificationJobService));
                var component = new ComponentName(Application.Context, javaClass);

                // Bundle up parameters
                var extras = new PersistableBundle();
                extras.PutString(ExtraReturnNotification, serializedNotification);

                triggerTime -= NotifyTimeInMilliseconds(DateTime.Now);

                var builder = new JobInfo.Builder(localNotification.NotificationId, component);
                builder.SetMinimumLatency(triggerTime); // Fire at TriggerTime
                builder.SetOverrideDeadline(triggerTime + 5000); // Or at least 5 Seconds Later
                builder.SetExtras(extras);
                builder.SetPersisted(CheckBootPermission()); //Job will be recreated after Reboot if Permissions are granted

                var jobInfo = builder.Build();
                var result = _jobScheduler.Schedule(jobInfo);
                scheduled = result == JobScheduler.ResultSuccess;
            }

            if (scheduled)
            {
                return;
            }
            // The job was not scheduled. So use the old implementation
            var notificationIntent = new Intent(Application.Context, typeof(ScheduledNotificationReceiver));
            notificationIntent.SetAction("LocalNotifierIntent" + localNotification.NotificationId);
            notificationIntent.PutExtra(ExtraReturnNotification, serializedNotification);

            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, 0, notificationIntent,
                PendingIntentFlags.CancelCurrent);

            _alarmManager.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
        }

        private bool CheckBootPermission()
        {
            return Application.Context.CheckSelfPermission("android.permission.RECEIVE_BOOT_COMPLETED") == Android.Content.PM.Permission.Granted;
        }

        private long NotifyTimeInMilliseconds(DateTime notifyTime)
        {
            var utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
            var epochDifference = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;

            var utcAlarmTimeInMillis = utcTime.AddSeconds(-epochDifference).Ticks / 10000;

            return utcAlarmTimeInMillis;
        }

        private void ShowNow(LocalNotification localNotification)
        {
            try
            {
                var builder = new NotificationCompat.Builder(Application.Context);
                builder.SetContentTitle(localNotification.Title);
                builder.SetContentText(localNotification.Description);
                builder.SetStyle(new NotificationCompat.BigTextStyle().BigText(localNotification.Description));
                builder.SetNumber(localNotification.BadgeNumber);
                builder.SetAutoCancel(true);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var channelId = $"{Application.Context.PackageName}.general";

                    var channel = new NotificationChannel(channelId, "General", NotificationImportance.Default);

                    _notificationManager.CreateNotificationChannel(channel);

                    builder.SetChannelId(channelId);
                }

                var notificationIntent = Application.Context.PackageManager.GetLaunchIntentForPackage(Application.Context.PackageName);
                notificationIntent.SetFlags(ActivityFlags.SingleTop);

                notificationIntent.PutExtra(ExtraReturnData, localNotification.ReturningData);

                var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
                builder.SetContentIntent(pendingIntent);

                if (NotificationIconId != 0)
                {
                    builder.SetSmallIcon(NotificationIconId);
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
                notification.Defaults = NotificationDefaults.All;

                _notificationManager.Notify(localNotification.NotificationId, notification);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}