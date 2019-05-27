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

        private void ShowLater(NotificationRequest notificationRequest)
        {
            if (notificationRequest.NotifyTime.HasValue == false)
            {
                return;
            }

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

        private void ShowNow(NotificationRequest notificationRequest)
        {
            try
            {
                var channelId = $"{Application.Context.PackageName}.{notificationRequest.Android.ChannelName}";

                var builder = new NotificationCompat.Builder(Application.Context, channelId);
                builder.SetContentTitle(notificationRequest.Title);
                builder.SetContentText(notificationRequest.Description);
                builder.SetStyle(new NotificationCompat.BigTextStyle().BigText(notificationRequest.Description));
                builder.SetNumber(notificationRequest.BadgeNumber);

                if (string.IsNullOrWhiteSpace(notificationRequest.Sound) == false)
                {
                    if (notificationRequest.Sound.Contains("://") == false)
                    {
                        notificationRequest.Sound = $"{ContentResolver.SchemeAndroidResource}://{Application.Context.PackageName}/raw/{notificationRequest.Sound}";
                    }
                    var uri = Android.Net.Uri.Parse(notificationRequest.Sound);
                    builder.SetSound(uri);
                }

                builder.SetAutoCancel(notificationRequest.Android.AutoCancel);
                builder.SetPriority((int)notificationRequest.Android.Priority);
                if (notificationRequest.Android.Color.HasValue)
                {
                    builder.SetColor(notificationRequest.Android.Color.Value);
                }
                if (notificationRequest.Android.TimeoutAfter.HasValue)
                {
                    builder.SetTimeoutAfter((long)notificationRequest.Android.TimeoutAfter.Value.TotalMilliseconds);
                }

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var channel = _notificationManager.GetNotificationChannel(channelId);
                    if (channel is null)
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

                        // you can't change the importance or other notification behaviors after this.
                        // once you create the channel, you cannot change these settings and
                        // the user has final control of whether these behaviors are active.
                        channel = new NotificationChannel(channelId, notificationRequest.Android.ChannelName, importance)
                        {
                            Description = notificationRequest.Android.ChannelDescription
                        };
                        _notificationManager.CreateNotificationChannel(channel);
                    }

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
                        iconId = Application.Context.Resources.GetIdentifier("icon", "drawable", Application.Context.PackageName);
                    }
                    builder.SetSmallIcon(iconId);
                }

                var notification = builder.Build();
                if (notificationRequest.Android.LedColor.HasValue)
                {
                    notification.LedARGB = notificationRequest.Android.LedColor.Value;
                }
                if (string.IsNullOrWhiteSpace(notificationRequest.Sound))
                {
                    notification.Defaults = NotificationDefaults.All;
                }
                _notificationManager.Notify(notificationRequest.NotificationId, notification);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}