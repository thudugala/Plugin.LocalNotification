using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using AndroidX.Work;
using Java.Util.Concurrent;
using System;
using System.Globalization;
using AndroidX.Core.App;

namespace Plugin.LocalNotification.Platform.Droid
{
    /// <inheritdoc />
    public class NotificationServiceImpl : INotificationService
    {
        private readonly NotificationManager _notificationManager;
        private readonly WorkManager _workManager;

        /// <inheritdoc />
        public event NotificationTappedEventHandler NotificationTapped;

        /// <inheritdoc />
        public void OnNotificationTapped(NotificationTappedEventArgs e)
        {
            NotificationTapped?.Invoke(e);
        }

        /// <summary>
        ///
        /// </summary>
        public NotificationServiceImpl()
        {
            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
                {
                    return;
                }

                _notificationManager =
                    Application.Context.GetSystemService(Context.NotificationService) as NotificationManager ??
                    throw new ApplicationException(Properties.Resources.AndroidNotificationServiceNotFound);
                _workManager = WorkManager.GetInstance(Application.Context);
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

                _workManager?.CancelAllWorkByTag(notificationId.ToString(CultureInfo.CurrentCulture));
                _notificationManager?.Cancel(notificationId);
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

                _workManager?.CancelAllWork();
                _notificationManager?.CancelAll();
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
            if (notificationRequest.NotifyTime is null ||
                notificationRequest.NotifyTime.Value <= DateTime.Now) // To be consistent with iOS, Do not Schedule notification if NotifyTime is earlier than DateTime.Now
            {
                return;
            }

            Cancel(notificationRequest.NotificationId);

            var notifyTime = notificationRequest.NotifyTime.Value;
            var serializedNotification = ObjectSerializer.SerializeObject(notificationRequest);
            // Why serialized options separately ?
            // System.Xml.Serialization.XmlSerializer Deserialize and Serialize methods ignore object property "Android" when linking option set to "SDK Assemblies Only"
            var serializedNotificationAndroid = ObjectSerializer.SerializeObject(notificationRequest.Android);
            Log.Info(Application.Context.PackageName, $"NotificationServiceImpl.ShowLater: SerializedNotification [{serializedNotification}]");
            using var dataBuilder = new Data.Builder();
            dataBuilder.PutString(NotificationCenter.ExtraReturnNotification, serializedNotification);
            dataBuilder.PutString($"{NotificationCenter.ExtraReturnNotification}_Android", serializedNotificationAndroid);

            var requestBuilder = OneTimeWorkRequest.Builder.From<ScheduledNotificationWorker>();
            requestBuilder.AddTag(notificationRequest.NotificationId.ToString(CultureInfo.CurrentCulture));
            requestBuilder.SetInputData(dataBuilder.Build());
            var diff = (long)(notifyTime - DateTime.Now).TotalMilliseconds;
            requestBuilder.SetInitialDelay(diff, TimeUnit.Milliseconds);

            var workRequest = requestBuilder.Build();
            _workManager?.Enqueue(workRequest);
        }

        private void ShowNow(NotificationRequest request)
        {
            Cancel(request.NotificationId);

            if (string.IsNullOrWhiteSpace(request.Android.ChannelId))
            {
                request.Android.ChannelId = NotificationCenter.DefaultChannelId;
            }

            using var builder = new NotificationCompat.Builder(Application.Context, request.Android.ChannelId);
            builder.SetContentTitle(request.Title);
            builder.SetContentText(request.Description);
            using (var bigTextStyle = new NotificationCompat.BigTextStyle())
            {
                var bigText = bigTextStyle.BigText(request.Description);
                builder.SetStyle(bigText);
            }
            builder.SetNumber(request.BadgeNumber);
            builder.SetAutoCancel(request.Android.AutoCancel);
            builder.SetOngoing(request.Android.Ongoing);

            if (string.IsNullOrWhiteSpace(request.Android.Group) == false)
            {
                builder.SetGroup(request.Android.Group);
                if (request.Android.IsGroupSummary)
                {
                    builder.SetGroupSummary(true);
                }
            }

            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                builder.SetPriority((int)request.Android.Priority);

                var soundUri = NotificationCenter.GetSoundUri(request.Sound);
                if (soundUri != null)
                {
                    builder.SetSound(soundUri);
                }
            }

            if (request.Android.VibrationPattern != null)
            {
                builder.SetVibrate(request.Android.VibrationPattern);
            }

            if (request.Android.ProgressBarMax.HasValue &&
                request.Android.ProgressBarProgress.HasValue &&
                request.Android.ProgressBarIndeterminate.HasValue)
            {
                builder.SetProgress(request.Android.ProgressBarMax.Value,
                    request.Android.ProgressBarProgress.Value,
                    request.Android.ProgressBarIndeterminate.Value);
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
            var pendingIntent = PendingIntent.GetActivity(Application.Context, request.NotificationId, notificationIntent,
                PendingIntentFlags.CancelCurrent);
            builder.SetContentIntent(pendingIntent);

            var notification = builder.Build();
            if (request.Android.LedColor.HasValue)
            {
                notification.LedARGB = request.Android.LedColor.Value;
            }

            if (Build.VERSION.SdkInt < BuildVersionCodes.O &&
                string.IsNullOrWhiteSpace(request.Sound))
            {
                notification.Defaults = NotificationDefaults.All;
            }
            _notificationManager?.Notify(request.NotificationId, notification);
        }

        private static int GetIcon(string iconName)
        {
            var iconId = 0;
            if (string.IsNullOrWhiteSpace(iconName) == false)
            {
                iconId = Application.Context.Resources.GetIdentifier(iconName, "drawable", Application.Context.PackageName);
            }

            if (iconId != 0)
            {
                return iconId;
            }

            iconId = Application.Context.ApplicationInfo.Icon;
            if (iconId == 0)
            {
                iconId = Application.Context.Resources.GetIdentifier("icon", "drawable",
                    Application.Context.PackageName);
            }

            return iconId;
        }
    }
}