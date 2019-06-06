using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using AndroidX.Work;
using Java.Util.Concurrent;
using System;

namespace Plugin.LocalNotification.Platform.Droid
{
    /// <inheritdoc />
    [Android.Runtime.Preserve]
    public class NotificationServiceImpl : INotificationService
    {
        private readonly NotificationManager _notificationManager;

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
                if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
                {
                    return;
                }

                _notificationManager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;
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
                if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
                {
                    return;
                }

                WorkManager.Instance.CancelAllWorkByTag(notificationId.ToString());
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
                if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
                {
                    return;
                }

                WorkManager.Instance.CancelAllWork();
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
            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private void ShowLater(NotificationRequest notificationRequest)
        {
            if (notificationRequest.NotifyTime.HasValue == false)
            {
                return;
            }

            Cancel(notificationRequest.NotificationId);

            var triggerTime = NotifyTimeInMilliseconds(notificationRequest.NotifyTime.Value);
            notificationRequest.NotifyTime = null;
            triggerTime -= NotifyTimeInMilliseconds(DateTime.Now);

            var serializedNotification = ObjectSerializer<NotificationRequest>.SerializeObject(notificationRequest);

            var dataBuilder = new Data.Builder();
            dataBuilder.PutString(NotificationCenter.ExtraReturnNotification, serializedNotification);

            var reqbuilder = OneTimeWorkRequest.Builder.From<ScheduledNotificationWorker>();
            reqbuilder.AddTag(notificationRequest.NotificationId.ToString());
            reqbuilder.SetInputData(dataBuilder.Build());
            reqbuilder.SetInitialDelay(triggerTime, TimeUnit.Milliseconds);

            var workRequest = reqbuilder.Build();
            WorkManager.Instance.Enqueue(workRequest);
        }

        private static long NotifyTimeInMilliseconds(DateTime notifyTime)
        {
            var utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
            var epochDifference = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;

            var utcAlarmTimeInMillis = utcTime.AddSeconds(-epochDifference).Ticks / 10000;

            return utcAlarmTimeInMillis;
        }

        private void ShowNow(NotificationRequest request)
        {
            Cancel(request.NotificationId);

            if (string.IsNullOrWhiteSpace(request.Android.ChannelId))
            {
                request.Android.ChannelId = NotificationCenter.DefaultChannelId; 
            }

            var builder = new NotificationCompat.Builder(Application.Context, request.Android.ChannelId);
            builder.SetContentTitle(request.Title);
            builder.SetContentText(request.Description);
            builder.SetStyle(new NotificationCompat.BigTextStyle().BigText(request.Description));
            builder.SetNumber(request.BadgeNumber);
            builder.SetAutoCancel(request.Android.AutoCancel);

            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                builder.SetPriority((int)request.Android.Priority);

                var soundUri = NotificationCenter.GetSoundUri(request.Sound);
                if (soundUri != null)
                {
                    builder.SetSound(soundUri);
                }
            }

            if (request.Android.Color.HasValue)
            {
                builder.SetColor(request.Android.Color.Value);
            }
            builder.SetSmallIcon(GetIcon(request.Android.IconName));
            if (request.Android.TimeoutAfter.HasValue)
            {
                builder.SetTimeoutAfter((long)request.Android.TimeoutAfter.Value.TotalMilliseconds);
            }

            var notificationIntent = Application.Context.PackageManager.GetLaunchIntentForPackage(Application.Context.PackageName);
            notificationIntent.SetFlags(ActivityFlags.SingleTop);
            notificationIntent.PutExtra(NotificationCenter.ExtraReturnDataAndroid, request.ReturningData);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            builder.SetContentIntent(pendingIntent);

            var notification = builder.Build();
            if (request.Android.LedColor.HasValue)
            {
                notification.LedARGB = request.Android.LedColor.Value;
            }
            if (Build.VERSION.SdkInt < BuildVersionCodes.O &&
                string.IsNullOrWhiteSpace(request.Sound) == false)
            {
                notification.Defaults = NotificationDefaults.All;
            }
            _notificationManager.Notify(request.NotificationId, notification);
        }

        private int GetIcon(string iconName)
        {
            var iconId = 0;
            if (string.IsNullOrWhiteSpace(iconName) == false)
            {
                iconId = Application.Context.Resources.GetIdentifier(iconName, "drawable", Application.Context.PackageName);
            }
            if (iconId == 0)
            {
                iconId = Application.Context.ApplicationInfo.Icon;
                if (iconId == 0)
                {
                    iconId = Application.Context.Resources.GetIdentifier("icon", "drawable",
                        Application.Context.PackageName);
                }
            }

            return iconId;
        }
    }
}