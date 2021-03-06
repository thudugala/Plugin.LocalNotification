using Android.App;
using Android.Content;
using Android.OS;
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
        /// <summary>
        /// 
        /// </summary>
        protected readonly NotificationManager NotificationManager;
        
        /// <summary>
        /// 
        /// </summary>
        protected readonly WorkManager WorkManager;

        /// <inheritdoc />
        public event NotificationTappedEventHandler NotificationTapped;

        /// <inheritdoc />
        public event NotificationReceivedEventHandler NotificationReceived;

        /// <inheritdoc />
        public void OnNotificationTapped(NotificationTappedEventArgs e)
        {
            NotificationTapped?.Invoke(e);
        }

        /// <inheritdoc />
        public void OnNotificationReceived(NotificationReceivedEventArgs e)
        {
            NotificationReceived?.Invoke(e);
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

                NotificationManager =
                    Application.Context.GetSystemService(Context.NotificationService) as NotificationManager ??
                    throw new ApplicationException(Properties.Resources.AndroidNotificationServiceNotFound);
                WorkManager = WorkManager.GetInstance(Application.Context);
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

                WorkManager?.CancelAllWorkByTag(notificationId.ToString(CultureInfo.CurrentCulture));
                NotificationManager?.Cancel(notificationId);
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

                WorkManager?.CancelAllWork();
                NotificationManager?.CancelAll();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        /// <inheritdoc />
        public void Show(Func<NotificationRequestBuilder, NotificationRequest> builder) => Show(builder.Invoke(new NotificationRequestBuilder()));

        /// <inheritdoc />
        public void Show(NotificationRequest notificationRequest)
        {            
            Show((builder) => builder
                .NotifyAt(DateTime.Now.AddSeconds(10))
                .WithDescription("SampleDescription")
                .WithNotificationId(10)
                .WithTitle("SampleTitle")
                .WithAndroidOptions((android) => android
                    .WithAutoCancel(true)
                    .WithChannelId("General")
                    .WithOngoingStatus(true)
                    .WithPriority(NotificationPriority.High)
                    .Build())
                .WithiOSOptions((ios) => ios
                    .WithForegroundAlertStatus(false)
                    .WithForegroundSoundStatus(true)
                    .Build())
                .Create());
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
                    ShowNow(notificationRequest, true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notificationRequest"></param>
        protected virtual void ShowLater(NotificationRequest notificationRequest)
        {
            if (notificationRequest.NotifyTime is null ||
                notificationRequest.NotifyTime.Value <= DateTime.Now) // To be consistent with iOS, Do not Schedule notification if NotifyTime is earlier than DateTime.Now
            {
                return;
            }

            Cancel(notificationRequest.NotificationId);

            EnqueueWorker(notificationRequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancelBeforeShow"></param>
        protected internal virtual void ShowNow(NotificationRequest request, bool cancelBeforeShow)
        {
            if (cancelBeforeShow)
            {
                Cancel(request.NotificationId);
            }

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

            var notificationIntent = Application.Context.PackageManager?.GetLaunchIntentForPackage(Application.Context.PackageName ?? string.Empty);
            if (notificationIntent == null)
            {
                Log($"NotificationServiceImpl.ShowNow: notificationIntent is null");
                return;
            }

            notificationIntent.SetFlags(ActivityFlags.SingleTop);
            notificationIntent.PutExtra(NotificationCenter.ExtraReturnDataAndroid, request.ReturningData);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, request.NotificationId, notificationIntent,
                PendingIntentFlags.CancelCurrent);
            builder.SetContentIntent(pendingIntent);

            var notification = builder.Build();
            if (Build.VERSION.SdkInt < BuildVersionCodes.O &&
                request.Android.LedColor.HasValue)
            {
                notification.LedARGB = request.Android.LedColor.Value;
            }

            if (Build.VERSION.SdkInt < BuildVersionCodes.O &&
                string.IsNullOrWhiteSpace(request.Sound))
            {
                notification.Defaults = NotificationDefaults.All;
            }
            NotificationManager?.Notify(request.NotificationId, notification);

            var args = new NotificationReceivedEventArgs
            {
                Title = request.Title,
                Description = request.Description,
                Data = request.ReturningData
            };
            NotificationCenter.Current.OnNotificationReceived(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notificationRequest"></param>
        protected internal virtual void EnqueueWorker(NotificationRequest notificationRequest)
        {
            if (!notificationRequest.NotifyTime.HasValue)
            {
                Log($"{nameof(notificationRequest.NotifyTime)} value doesn't set!");
                return;
            }

            var notifyTime = notificationRequest.NotifyTime.Value;
            var serializedNotification = ObjectSerializer.SerializeObject(notificationRequest);

            // Why serialized options separately ?
            // System.Xml.Serialization.XmlSerializer Deserialize and Serialize methods ignore
            // object property "Android" when linking option set to "SDK Assemblies Only"
            var serializedNotificationAndroid = ObjectSerializer.SerializeObject(notificationRequest.Android);
            Log($"NotificationServiceImpl.ShowLater: SerializedNotification [{serializedNotification}]");

            using var dataBuilder = new Data.Builder()
                .PutString(NotificationCenter.ExtraReturnNotification, serializedNotification)
                .PutString($"{NotificationCenter.ExtraReturnNotification}_Android", serializedNotificationAndroid);
            var data = dataBuilder.Build();
            var tag = notificationRequest.NotificationId.ToString(CultureInfo.CurrentCulture);
            var diff = (long)(notifyTime - DateTime.Now).TotalMilliseconds;

            var workRequest = OneTimeWorkRequest.Builder.From<ScheduledNotificationWorker>()
                .AddTag(tag)
                .SetInputData(data)
                .SetInitialDelay(diff, TimeUnit.Milliseconds)
                .Build();

            WorkManager?.Enqueue(workRequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iconName"></param>
        /// <returns></returns>
        protected static int GetIcon(string iconName)
        {
            var iconId = 0;
            if (string.IsNullOrWhiteSpace(iconName) == false)
            {
                iconId = Application.Context.Resources?.GetIdentifier(iconName, "drawable", Application.Context.PackageName) ?? 0;
            }

            if (iconId != 0)
            {
                return iconId;
            }

            iconId = Application.Context.ApplicationInfo?.Icon ?? 0;
            if (iconId == 0)
            {
                iconId = Application.Context.Resources?.GetIdentifier("icon", "drawable",
                    Application.Context.PackageName) ?? 0;
            }

            return iconId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected static void Log(string message)
        {
            Android.Util.Log.Info(Application.Context.PackageName, message);
        }
    }
}
