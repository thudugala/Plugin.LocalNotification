using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Work;
using Java.Util.Concurrent;
using Plugin.LocalNotification.Platform.Droid;
using System;
using System.Globalization;

namespace Plugin.LocalNotification
{
    public static partial class NotificationCenter
    {
        /// <summary>
        /// Return Data Key.
        /// </summary>
        public static string ExtraReturnDataAndroid => "Plugin.LocalNotification.RETURN_DATA";

        /// <summary>
        /// Return Notification Key.
        /// </summary>
        public static string ExtraReturnNotification => "Plugin.LocalNotification.RETURN_NOTIFICATION";

        /// <summary>
        /// Default Channel Id.
        /// </summary>
        public static string DefaultChannelId => "Plugin.LocalNotification.GENERAL";

        /// <summary>
        /// Notify Local Notification Tapped.
        /// </summary>
        /// <param name="intent"></param>
        public static void NotifyNotificationTapped(Intent intent)
        {
            try
            {
                if (intent is null)
                {
                    return;
                }

                if (intent.HasExtra(ExtraReturnDataAndroid) == false)
                {
                    return;
                }

                var subscribeItem = new NotificationTappedEventArgs
                {
                    Data = intent.GetStringExtra(ExtraReturnDataAndroid)
                };

                OnNotificationTapped(subscribeItem);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Create Notification Channel Group with builder when API >= 26.
        /// If you'd like to further organize the appearance of your channels in the settings UI, you can create channel groups.
        /// This is a good idea when your app supports multiple user accounts (such as for work profiles),
        /// so you can create a notification channel group for each account.
        /// This way, users can easily identify and control multiple notification channels that have identical names.
        /// </summary>
        /// <param name="builder"></param>
        public static void CreateNotificationChannelGroup(Func<NotificationChannelGroupRequestBuilder, NotificationChannelGroupRequest> builder)
        {
            CreateNotificationChannelGroup(builder.Invoke(new NotificationChannelGroupRequestBuilder()));
        }

        /// <summary>
        /// Create Notification Channel Group when API >= 26.
        /// If you'd like to further organize the appearance of your channels in the settings UI, you can create channel groups.
        /// This is a good idea when your app supports multiple user accounts (such as for work profiles),
        /// so you can create a notification channel group for each account.
        /// This way, users can easily identify and control multiple notification channels that have identical names.
        /// </summary>
        /// <param name="request"></param>
        public static void CreateNotificationChannelGroup(NotificationChannelGroupRequest request)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }

            if (!(Application.Context.GetSystemService(Context.NotificationService) is NotificationManager
                notificationManager))
            {
                return;
            }

            if (request is null ||
                string.IsNullOrWhiteSpace(request.Group) ||
                string.IsNullOrWhiteSpace(request.Name))
            {
                return;
            }

            using (var channelGroup = new NotificationChannelGroup(request.Group, request.Name))
            {
                notificationManager.CreateNotificationChannelGroup(channelGroup);
            }
        }

        /// <summary>
        /// Create Notification Channel with builder when API >= 26.
        /// </summary>
        /// <param name="builder"></param>
        public static void CreateNotificationChannel(Func<NotificationChannelRequestBuilder, NotificationChannelRequest> builder)
        {
            CreateNotificationChannel(builder.Invoke(new NotificationChannelRequestBuilder()));
        }

        /// <summary>
        /// Create Notification Channel when API >= 26.
        /// </summary>
        /// <param name="request"></param>
        public static void CreateNotificationChannel(NotificationChannelRequest request = null)
        {
            Initialize();

            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }

            if (Application.Context.GetSystemService(Context.NotificationService) is not NotificationManager
                notificationManager)
            {
                return;
            }

            if (request is null)
            {
                request = new NotificationChannelRequest();
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                request.Name = "General";
            }

            if (string.IsNullOrWhiteSpace(request.Id))
            {
                request.Id = DefaultChannelId;
            }

            // you can't change the importance or other notification behaviors after this.
            // once you create the channel, you cannot change these settings and
            // the user has final control of whether these behaviors are active.
            using var channel = new NotificationChannel(request.Id, request.Name, request.Importance)
            {
                Description = request.Description,
                Group = request.Group,
                LightColor = request.LightColor,
                LockscreenVisibility = request.LockscreenVisibility,
            };
            var soundUri = GetSoundUri(request.Sound);
            if (soundUri != null)
            {
                using var audioAttributesBuilder = new AudioAttributes.Builder();
                var audioAttributes = audioAttributesBuilder.SetUsage(AudioUsageKind.Notification)
                    .SetContentType(AudioContentType.Music)
                    .Build();

                channel.SetSound(soundUri, audioAttributes);
            }

            if (request.VibrationPattern != null)
            {
                channel.SetVibrationPattern(request.VibrationPattern);
            }

            channel.SetShowBadge(request.ShowBadge);
            channel.EnableLights(request.EnableLights);
            channel.EnableVibration(request.EnableVibration);
            channel.SetBypassDnd(request.CanBypassDnd);

            notificationManager.CreateNotificationChannel(channel);
        }

        internal static Android.Net.Uri GetSoundUri(string soundFileName)
        {
            if (string.IsNullOrWhiteSpace(soundFileName))
            {
                return null;
            }

            if (soundFileName.Contains("://", StringComparison.InvariantCulture) == false)
            {
                soundFileName = $"{ContentResolver.SchemeAndroidResource}://{Application.Context.PackageName}/raw/{soundFileName}";
            }

            return Android.Net.Uri.Parse(soundFileName);
        }

        #region Impletmentation

        /// <summary>
        /// 
        /// </summary>
        private static NotificationManager _notificationManager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

        /// <summary>
        /// 
        /// </summary>
        private static WorkManager _workManager = WorkManager.GetInstance(Application.Context);

        /// <inheritdoc />
        private static void OnPlatformNotificationTapped(NotificationTappedEventArgs e)
        {
            NotificationTapped?.Invoke(e);
        }

        /// <inheritdoc />
        private static void OnPlatformNotificationReceived(NotificationReceivedEventArgs e)
        {
            NotificationReceived?.Invoke(e);
        }

        /// <summary>
        ///
        /// </summary>
        private static void Initialize()
        {
            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
                {
                    return;
                }

                _notificationManager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager ??
                                      throw new ApplicationException(Properties.Resources.AndroidNotificationServiceNotFound);
                _workManager = WorkManager.GetInstance(Application.Context);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        /// <inheritdoc />
        private static void PlatformCancel(int notificationId)
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
        private static void PlatformCancelAll()
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
        private static void PlatformShow(Func<NotificationRequestBuilder, NotificationRequest> builder)
        {
            Show(builder.Invoke(new NotificationRequestBuilder()));
        }

        /// <inheritdoc />
        private static void PlatformShow(NotificationRequest notificationRequest)
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
        private static void ShowLater(NotificationRequest notificationRequest)
        {
            if (notificationRequest.NotifyTime is null || notificationRequest.NotifyTime.Value <= DateTime.Now)
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
        internal static void ShowNow(NotificationRequest request, bool cancelBeforeShow)
        {
            if (cancelBeforeShow)
            {
                Cancel(request.NotificationId);
            }

            if (string.IsNullOrWhiteSpace(request.Android.ChannelId))
            {
                request.Android.ChannelId = DefaultChannelId;
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

                var soundUri = GetSoundUri(request.Sound);
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
                Log("NotificationServiceImpl.ShowNow: notificationIntent is null");
                return;
            }

            notificationIntent.SetFlags(ActivityFlags.SingleTop);
            notificationIntent.PutExtra(ExtraReturnDataAndroid, request.ReturningData);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, request.NotificationId, notificationIntent,
                PendingIntentFlags.CancelCurrent);
            builder.SetContentIntent(pendingIntent);

            var notification = builder.Build();
            if (Build.VERSION.SdkInt < BuildVersionCodes.O && request.Android.LedColor.HasValue)
            {
                notification.LedARGB = request.Android.LedColor.Value;
            }

            if (Build.VERSION.SdkInt < BuildVersionCodes.O && string.IsNullOrWhiteSpace(request.Sound))
            {
                notification.Defaults = NotificationDefaults.All;
            }

            //var notificationManager = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);

            //if(notificationManager is null)
            //    return;

            //notificationManager?.Notify(request.NotificationId, notification);
            _notificationManager?.Notify(request.NotificationId, notification);

            var args = new NotificationReceivedEventArgs
            {
                Title = request.Title,
                Description = request.Description,
                Data = request.ReturningData
            };

            OnNotificationReceived(args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notificationRequest"></param>
        internal static void EnqueueWorker(NotificationRequest notificationRequest)
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
                .PutString(ExtraReturnNotification, serializedNotification)
                .PutString($"{ExtraReturnNotification}_Android", serializedNotificationAndroid);
            var data = dataBuilder.Build();
            var tag = notificationRequest.NotificationId.ToString(CultureInfo.CurrentCulture);
            var diff = (long)(notifyTime - DateTime.Now).TotalMilliseconds;

            var workRequest = OneTimeWorkRequest.Builder.From<ScheduledNotificationWorker>()
                .AddTag(tag)
                .SetInputData(data)
                .SetInitialDelay(diff, TimeUnit.Milliseconds)
                .Build();

            _workManager?.Enqueue(workRequest);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iconName"></param>
        /// <returns></returns>
        private static int GetIcon(string iconName)
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
        private static void Log(string message)
        {
            Android.Util.Log.Info(Application.Context.PackageName, message);
        }

        #endregion
    }
}