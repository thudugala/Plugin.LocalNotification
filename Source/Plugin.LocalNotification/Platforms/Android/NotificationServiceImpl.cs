using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;
using Java.Lang;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.EventArgs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Exception = System.Exception;

namespace Plugin.LocalNotification.Platforms
{
    /// <inheritdoc />
    public class NotificationServiceImpl : INotificationService
    {
        private readonly IList<NotificationCategory> _categoryList = new List<NotificationCategory>();

        /// <inheritdoc />
        public Func<NotificationRequest, Task<NotificationEventReceivingArgs>> NotificationReceiving { get; set; }

        /// <summary>
        ///
        /// </summary>
        protected readonly NotificationManager MyNotificationManager;

        /// <summary>
        ///
        /// </summary>
        protected readonly AlarmManager MyAlarmManager;

        /// <summary>
        ///
        /// </summary>
        protected readonly GeofencingClient MyGeofencingClient;

        private readonly Random _random;

        /// <inheritdoc />
        public event NotificationReceivedEventHandler NotificationReceived;

        /// <inheritdoc />
        public event NotificationActionTappedEventHandler NotificationActionTapped;

        /// <inheritdoc />
        public event NotificationDisabledEventHandler NotificationsDisabled;

        /// <inheritdoc />
        public void OnNotificationActionTapped(NotificationActionEventArgs e)
        {
            NotificationActionTapped?.Invoke(e);
        }

        /// <inheritdoc />
        public void OnNotificationReceived(NotificationEventArgs e)
        {
            NotificationReceived?.Invoke(e);
        }

        /// <summary>
        ///
        /// </summary>
        public void OnNotificationsDisabled()
        {
            NotificationsDisabled?.Invoke();
        }

        /// <inheritdoc />
        public Task<IList<NotificationRequest>> GetPendingNotificationList()
        {
            IList<NotificationRequest> itemList = NotificationRepository.Current.GetPendingList();
            return Task.FromResult(itemList);
        }

        /// <inheritdoc />
        public Task<IList<NotificationRequest>> GetDeliveredNotificationList()
        {
            IList<NotificationRequest> itemList = NotificationRepository.Current.GetDeliveredList();
            return Task.FromResult(itemList);
        }

        /// <summary>
        ///
        /// </summary>
        public NotificationServiceImpl()
        {
            try
            {
#if MONOANDROID
                if (Build.VERSION.SdkInt < BuildVersionCodes.Kitkat)
                {
                    return;
                }
#elif ANDROID
                if (!OperatingSystem.IsAndroidVersionAtLeast(21))
                {
                    return;
                }
#endif

                MyNotificationManager = NotificationManager.FromContext(Application.Context);
                MyAlarmManager = AlarmManager.FromContext(Application.Context);
                MyGeofencingClient = LocationServices.GetGeofencingClient(Application.Context);

                _random = new Random();
            }
            catch (Exception ex)
            {
                LocalNotificationCenter.Log(ex);
            }
        }

        /// <inheritdoc />
        public bool Cancel(params int[] notificationIdList)
        {
#if MONOANDROID
            if (Build.VERSION.SdkInt < BuildVersionCodes.Kitkat)
            {
                return false;
            }
#elif ANDROID
            if (!OperatingSystem.IsAndroidVersionAtLeast(21))
            {
                return false;
            }
#endif

            foreach (var notificationId in notificationIdList)
            {
                var alarmPendingIntent = CreateAlarmIntent(notificationId, null);
                MyAlarmManager?.Cancel(alarmPendingIntent);
                alarmPendingIntent?.Cancel();

                MyNotificationManager?.Cancel(notificationId);

                var geoPendingIntent = CreateGeofenceIntent(notificationId, null);
                MyGeofencingClient?.RemoveGeofences(geoPendingIntent);
            }

            NotificationRepository.Current.RemoveByPendingIdList(notificationIdList);
            NotificationRepository.Current.RemoveByDeliveredIdList(notificationIdList);
            return true;
        }

        /// <inheritdoc />
        public bool CancelAll()
        {
#if MONOANDROID
            if (Build.VERSION.SdkInt < BuildVersionCodes.Kitkat)
            {
                return false;
            }
#elif ANDROID
            if (!OperatingSystem.IsAndroidVersionAtLeast(21))
            {
                return false;
            }
#endif

            var notificationIdList = NotificationRepository.Current.GetPendingList().Select(r => r.NotificationId).ToList();
            foreach (var notificationId in notificationIdList)
            {
                var alarmPendingIntent = CreateAlarmIntent(notificationId, null);
                MyAlarmManager?.Cancel(alarmPendingIntent);
                alarmPendingIntent?.Cancel();

                var geoPendingIntent = CreateGeofenceIntent(notificationId, null);
                MyGeofencingClient?.RemoveGeofences(geoPendingIntent);
            }

            MyNotificationManager?.CancelAll();
            NotificationRepository.Current.RemovePendingList();
            NotificationRepository.Current.RemoveDeliveredList();
            return true;
        }

        /// <inheritdoc />
        public bool Clear(params int[] notificationIdList)
        {
            foreach (var notificationId in notificationIdList)
            {
                MyNotificationManager.Cancel(notificationId);
            }

            NotificationRepository.Current.RemoveByDeliveredIdList(notificationIdList);
            return true;
        }

        /// <inheritdoc />
        public bool ClearAll()
        {
            MyNotificationManager.CancelAll();
            NotificationRepository.Current.RemoveDeliveredList();
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> Show(NotificationRequest request)
        {
#if MONOANDROID
            if (Build.VERSION.SdkInt < BuildVersionCodes.Kitkat)
            {
                return false;
            }
#elif ANDROID
            if (!OperatingSystem.IsAndroidVersionAtLeast(21))
            {
                return false;
            }
#endif

            var allowed = await AreNotificationsEnabled().ConfigureAwait(false);
            if (allowed == false)
            {
                OnNotificationsDisabled();
                LocalNotificationCenter.Log("Notifications are disabled");
                return false;
            }

            if (request is null)
            {
                return false;
            }

            if (request.Geofence.IsGeofence)
            {
                var geoResult = await ShowGeofence(request);
                return geoResult;
            }

            if (request.Schedule.NotifyTime.HasValue)
            {
                return ShowLater(request);
            }

            var result = await ShowNow(request);
            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal virtual async Task<bool> ShowGeofence(NotificationRequest request)
        {
            var geofenceBuilder = new GeofenceBuilder()
            .SetRequestId(request.NotificationId.ToString())
            .SetExpirationDuration(request.Geofence.Android.ExpirationDurationInMilliseconds)
            .SetNotificationResponsiveness(request.Geofence.Android.ResponsivenessMilliseconds)
            .SetCircularRegion(
                request.Geofence.Center.Latitude,
                request.Geofence.Center.Longitude,
                Convert.ToSingle(request.Geofence.RadiusInMeters)
            );

            var transitionType = 0;
            if ((request.Geofence.NotifyOn & NotificationRequestGeofence.GeofenceNotifyOn.OnEntry) == NotificationRequestGeofence.GeofenceNotifyOn.OnEntry)
            {
                transitionType |= Geofence.GeofenceTransitionEnter;
            }
            if ((request.Geofence.NotifyOn & NotificationRequestGeofence.GeofenceNotifyOn.OnExit) == NotificationRequestGeofence.GeofenceNotifyOn.OnExit)
            {
                transitionType |= Geofence.GeofenceTransitionEnter;
            }

            if (request.Geofence.Android.LoiteringDelayMilliseconds > 0)
            {
                transitionType = Geofence.GeofenceTransitionDwell;
                geofenceBuilder.SetLoiteringDelay(request.Geofence.Android.LoiteringDelayMilliseconds);
            }
            geofenceBuilder.SetTransitionTypes(transitionType);

            var geofence = geofenceBuilder.Build();

            var geoRequest = new GeofencingRequest.Builder()
                .SetInitialTrigger(0)
                .AddGeofence(geofence)
                .Build();

            var serializedRequest = LocalNotificationCenter.GetRequestSerialize(request);
            var pendingIntent = CreateGeofenceIntent(request.NotificationId, serializedRequest);

            await MyGeofencingClient
                .AddGeofencesAsync(
                    geoRequest,
                    pendingIntent
                )
                .ConfigureAwait(false);

            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="notificationId"></param>
        /// <param name="serializedRequest"></param>
        /// <returns></returns>
        protected virtual PendingIntent CreateGeofenceIntent(int notificationId, string serializedRequest)
        {
            var pendingIntent = CreateActionIntent(notificationId, serializedRequest, new NotificationAction(0)
            {
                Android =
                {
                    LaunchAppWhenTapped = false,
                    PendingIntentFlags = AndroidPendingIntentFlags.UpdateCurrent
                }
            }, typeof(GeofenceTransitionsIntentReceiver));
            return pendingIntent;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        internal virtual bool ShowLater(NotificationRequest request)
        {
            if (request.Schedule.Android.IsValidNotifyTime(DateTime.Now, request.Schedule.NotifyTime) == false)
            {
                LocalNotificationCenter.Log(
                    "NotifyTime is earlier than (DateTime.Now - Allowed Delay), notification ignored");
                return false;
            }

            var serializedRequest = LocalNotificationCenter.GetRequestSerialize(request);
            var alarmIntent = CreateAlarmIntent(request.NotificationId, serializedRequest);

            var utcAlarmTimeInMillis =
                (request.Schedule.NotifyTime.GetValueOrDefault().ToUniversalTime() - DateTime.UtcNow)
                .TotalMilliseconds;
            var triggerTime = (long)utcAlarmTimeInMillis;

            var alarmType = request.Schedule.Android.AlarmType.ToNative();
            var triggerAtTime = GetBaseCurrentTime(alarmType) + triggerTime;

            var canScheduleExactAlarms = true;
            if (
#if MONOANDROID
                Build.VERSION.SdkInt >= BuildVersionCodes.S
#elif ANDROID
                OperatingSystem.IsAndroidVersionAtLeast(31)
#endif
            )
            {
                canScheduleExactAlarms = MyAlarmManager.CanScheduleExactAlarms();
            }

            if (
#if MONOANDROID
                Build.VERSION.SdkInt >= BuildVersionCodes.M
#elif ANDROID
                OperatingSystem.IsAndroidVersionAtLeast(23)
#endif
            )
            {
                if (canScheduleExactAlarms)
                {
                    MyAlarmManager.SetExactAndAllowWhileIdle(alarmType, triggerAtTime, alarmIntent);
                }
                else
                {
                    MyAlarmManager.SetAndAllowWhileIdle(alarmType, triggerAtTime, alarmIntent);
                }
            }
            else
            {
                if (canScheduleExactAlarms)
                {
                    MyAlarmManager.SetExact(alarmType, triggerAtTime, alarmIntent);
                }
                else
                {
                    MyAlarmManager.Set(alarmType, triggerAtTime, alarmIntent);
                }
            }

            NotificationRepository.Current.AddPendingRequest(request);

            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="notificationId"></param>
        /// <param name="serializedRequest"></param>
        /// <returns></returns>
        protected virtual PendingIntent CreateAlarmIntent(int notificationId, string serializedRequest)
        {
            var pendingIntent = CreateActionIntent(notificationId, serializedRequest, new NotificationAction(0)
            {
                Android =
                {
                    LaunchAppWhenTapped = false,
                    PendingIntentFlags = AndroidPendingIntentFlags.UpdateCurrent
                }
            }, typeof(ScheduledAlarmReceiver));
            return pendingIntent;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual long GetBaseCurrentTime(AlarmType type)
        {
            return type switch
            {
                AlarmType.Rtc => JavaSystem.CurrentTimeMillis(),
                AlarmType.RtcWakeup => JavaSystem.CurrentTimeMillis(),
                AlarmType.ElapsedRealtime => SystemClock.ElapsedRealtime(),
                AlarmType.ElapsedRealtimeWakeup => SystemClock.ElapsedRealtime(),
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        internal virtual async Task<bool> ShowNow(NotificationRequest request)
        {
            var requestHandled = false;
            if (NotificationReceiving != null)
            {
                var requestArg = await NotificationReceiving(request).ConfigureAwait(false);
                if (requestArg is null || requestArg.Handled)
                {
                    requestHandled = true;
                }

                if (requestArg?.Request != null)
                {
                    request = requestArg.Request;
                }
            }

            if (string.IsNullOrWhiteSpace(request.Android.ChannelId))
            {
                request.Android.ChannelId = AndroidOptions.DefaultChannelId;
            }

            if (
#if MONOANDROID
                Build.VERSION.SdkInt >= BuildVersionCodes.O
#elif ANDROID
                OperatingSystem.IsAndroidVersionAtLeast(26)
#endif
                )
            {
                var channel = MyNotificationManager.GetNotificationChannel(request.Android.ChannelId);
                if (channel is null)
                {
                    LocalNotificationCenter.CreateNotificationChannel(new NotificationChannelRequest
                    {
                        Id = request.Android.ChannelId
                    });
                }
            }

            using var builder = new NotificationCompat.Builder(Application.Context, request.Android.ChannelId);

            builder.SetContentTitle(request.Title);
            builder.SetSubText(request.Subtitle);
            builder.SetContentText(request.Description);
            if (request.Image is { HasValue: true })
            {
                var imageBitmap = await GetNativeImage(request.Image).ConfigureAwait(false);
                if (imageBitmap != null)
                {
                    using var picStyle = new NotificationCompat.BigPictureStyle();
                    picStyle.BigPicture(imageBitmap);
                    picStyle.SetSummaryText(request.Subtitle);
                    builder.SetStyle(picStyle);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(request.Description) == false)
                {
                    using var bigTextStyle = new NotificationCompat.BigTextStyle();
                    bigTextStyle.BigText(request.Description);
                    bigTextStyle.SetSummaryText(request.Subtitle);
                    builder.SetStyle(bigTextStyle);
                }
            }

            builder.SetNumber(request.BadgeNumber);
            builder.SetAutoCancel(request.Android.AutoCancel);
            builder.SetOngoing(request.Android.Ongoing);

            if (string.IsNullOrWhiteSpace(request.Group) == false)
            {
                builder.SetGroup(request.Group);
                if (request.Android.IsGroupSummary)
                {
                    builder.SetGroupSummary(true);
                }
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                if (request.CategoryType != NotificationCategoryType.None)
                {
                    builder.SetCategory(request.CategoryType.ToNative());
                }

                builder.SetVisibility((int)request.Android.VisibilityType.ToNative());
            }

            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                builder.SetPriority((int)request.Android.Priority);

                var soundUri = LocalNotificationCenter.GetSoundUri(request.Sound);
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
                request.Android.IsProgressBarIndeterminate.HasValue)
            {
                builder.SetProgress(request.Android.ProgressBarMax.Value,
                    request.Android.ProgressBarProgress.Value,
                    request.Android.IsProgressBarIndeterminate.Value);
            }

            if (request.Android.Color != null)
            {
                builder.SetColor(request.Android.Color.ToNative());
            }

            builder.SetSmallIcon(GetIcon(request.Android.IconSmallName));
            if (request.Android.IconLargeName != null &&
                string.IsNullOrWhiteSpace(request.Android.IconLargeName.ResourceName) == false)
            {
                var largeIcon = await BitmapFactory.DecodeResourceAsync(Application.Context.Resources,
                    GetIcon(request.Android.IconLargeName)).ConfigureAwait(false);
                if (largeIcon != null)
                {
                    builder.SetLargeIcon(largeIcon);
                }
            }

            if (request.Android.TimeoutAfter.HasValue)
            {
                builder.SetTimeoutAfter((long)request.Android.TimeoutAfter.Value.TotalMilliseconds);
            }

            var serializedRequest = LocalNotificationCenter.GetRequestSerialize(request);

            var contentIntent = CreateActionIntent(request.NotificationId, serializedRequest, new NotificationAction(NotificationActionEventArgs.TapActionId)
            {
                Android =
                {
                    LaunchAppWhenTapped = request.Android.LaunchAppWhenTapped,
                    PendingIntentFlags = request.Android.PendingIntentFlags
                }
            }, typeof(NotificationActionReceiver));

            var deleteIntent = CreateActionIntent(request.NotificationId, serializedRequest, new NotificationAction(NotificationActionEventArgs.DismissedActionId)
            {
                Android =
                {
                    LaunchAppWhenTapped = false,
                    PendingIntentFlags = AndroidPendingIntentFlags.CancelCurrent
                }
            }, typeof(NotificationActionReceiver));

            builder.SetContentIntent(contentIntent);
            builder.SetDeleteIntent(deleteIntent);

            if (_categoryList.Any())
            {
                var categoryByType = _categoryList.FirstOrDefault(c => c.CategoryType == request.CategoryType);
                if (categoryByType != null)
                {
                    foreach (var notificationAction in categoryByType.ActionList)
                    {
                        var nativeAction = CreateAction(request, serializedRequest, notificationAction);
                        if (nativeAction is null)
                        {
                            continue;
                        }

                        builder.AddAction(nativeAction);
                    }
                }
            }

            if (request.Silent)
            {
                builder.SetSilent(request.Silent);
            }

            var notification = builder.Build();
            if (Build.VERSION.SdkInt < BuildVersionCodes.O &&
                request.Android.LedColor.HasValue)
            {
#pragma warning disable 618
#pragma warning disable CA1422
#pragma warning disable CA1416
                notification.LedARGB = request.Android.LedColor.Value;
#pragma warning restore CA1416
#pragma warning restore CA1422
#pragma warning restore 618
            }

            if (Build.VERSION.SdkInt < BuildVersionCodes.O &&
                string.IsNullOrWhiteSpace(request.Sound))
            {
#pragma warning disable 618
#pragma warning disable CA1422
#pragma warning disable CA1416
                notification.Defaults = NotificationDefaults.All;
#pragma warning restore CA1416
#pragma warning restore CA1422
#pragma warning restore 618
            }
            if (requestHandled == false)
            {
                MyNotificationManager?.Notify(request.NotificationId, notification);
            }
            else
            {
                LocalNotificationCenter.Log("NotificationServiceImpl.ShowNow: notification is Handled");
            }

            var args = new NotificationEventArgs
            {
                Request = request
            };
            LocalNotificationCenter.Current.OnNotificationReceived(args);
            NotificationRepository.Current.AddDeliveredRequest(request);

            return true;
        }

        /// <inheritdoc />
        public Task<bool> AreNotificationsEnabled()
        {
#if MONOANDROID
            if (Build.VERSION.SdkInt < BuildVersionCodes.N)
            {
                return Task.FromResult(true);
            }
#elif ANDROID
            if (!OperatingSystem.IsAndroidVersionAtLeast(24))
            {
                return Task.FromResult(true);
            }
#endif
            return Task.FromResult(MyNotificationManager.AreNotificationsEnabled());
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="notificationImage"></param>
        /// <returns></returns>
        protected virtual async Task<Bitmap> GetNativeImage(NotificationImage notificationImage)
        {
            if (notificationImage is null || notificationImage.HasValue == false)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(notificationImage.ResourceName) == false)
            {
                var imageId = Application.Context.Resources?.GetIdentifier(notificationImage.ResourceName,
                    AndroidIcon.DefaultType, Application.Context.PackageName);
                if (imageId != null)
                {
                    return await BitmapFactory.DecodeResourceAsync(Application.Context.Resources, imageId.Value)
                        .ConfigureAwait(false);
                }
            }

            if (string.IsNullOrWhiteSpace(notificationImage.FilePath) == false)
            {
                if (File.Exists(notificationImage.FilePath))
                {
                    return await BitmapFactory.DecodeFileAsync(notificationImage.FilePath).ConfigureAwait(false);
                }
            }

            return notificationImage.Binary is { Length: > 0 }
                ? await BitmapFactory.DecodeByteArrayAsync(notificationImage.Binary, 0,
                    notificationImage.Binary.Length).ConfigureAwait(false)
                : null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <param name="serializedRequest"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        protected virtual NotificationCompat.Action CreateAction(NotificationRequest request, string serializedRequest,
            NotificationAction action)
        {
            var pendingIntent = CreateActionIntent(request.NotificationId, serializedRequest, action, typeof(NotificationActionReceiver));
            if (string.IsNullOrWhiteSpace(action.Android.IconName.ResourceName))
            {
                action.Android.IconName = request.Android.IconSmallName;
            }

            var nativeAction = new NotificationCompat.Action(GetIcon(action.Android.IconName),
                new Java.Lang.String(action.Title), pendingIntent);

            return nativeAction;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="notificationId"></param>
        /// <param name="serializedRequest"></param>
        /// <param name="action"></param>
        /// <param name="broadcastReceiverType"></param>
        /// <returns></returns>
        protected virtual PendingIntent CreateActionIntent(int notificationId, string serializedRequest, NotificationAction action, Type broadcastReceiverType)
        {
            var notificationIntent = action.Android.LaunchAppWhenTapped
                ? (Application.Context.PackageManager?.GetLaunchIntentForPackage(Application.Context.PackageName ??
                                                                              string.Empty))
                : new Intent(Application.Context, broadcastReceiverType);

            notificationIntent.AddFlags(ActivityFlags.SingleTop)
                .AddFlags(ActivityFlags.IncludeStoppedPackages)
                .PutExtra(LocalNotificationCenter.ReturnRequestActionId, action.ActionId)
                .PutExtra(LocalNotificationCenter.ReturnRequest, serializedRequest);

            //var requestCode = _random.Next();
            // Cannot be random, then you cannot cancel it.
            var requestCode = notificationId + action.ActionId;

            var pendingIntent = action.Android.LaunchAppWhenTapped
                ? PendingIntent.GetActivity(
                    Application.Context,
                    requestCode,
                    notificationIntent,
                    action.Android.PendingIntentFlags.ToNative())
                : PendingIntent.GetBroadcast(
                    Application.Context,
                    requestCode,
                    notificationIntent,
                    action.Android.PendingIntentFlags.ToNative()
                );
            return pendingIntent;
        }

        //private NotificationCompat.Action CreateTextReply(NotificationRequest request, string serializedRequest, NotificationAction action)
        //{
        //    var pendingIntent = CreateActionIntent(request, serializedRequest, action);

        //    var input = new AndroidX.Core.App.RemoteInput.Builder(AndroidNotificationProcessor.RemoteInputResultKey)
        //        .SetLabel(action.Title)
        //        .Build();

        //    var iconId = GetIcon(request.Android.IconSmallName);
        //    var nativeAction = new NotificationCompat.Action.Builder(iconId, action.Title, pendingIntent)
        //        .SetAllowGeneratedReplies(true)
        //        .AddRemoteInput(input)
        //        .Build();

        //    return nativeAction;
        //}

        /// <summary>
        ///
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        protected static int GetIcon(AndroidIcon icon)
        {
            var iconId = 0;
            if (icon != null && string.IsNullOrWhiteSpace(icon.ResourceName) == false)
            {
                if (string.IsNullOrWhiteSpace(icon.Type))
                {
                    icon.Type = AndroidIcon.DefaultType;
                }

                iconId = Application.Context.Resources?.GetIdentifier(icon.ResourceName, icon.Type,
                    Application.Context.PackageName) ?? 0;
            }

            if (iconId != 0)
            {
                return iconId;
            }

            iconId = Application.Context.ApplicationInfo?.Icon ?? 0;
            if (iconId == 0)
            {
                iconId = Application.Context.Resources?.GetIdentifier("icon", AndroidIcon.DefaultType,
                    Application.Context.PackageName) ?? 0;
            }

            return iconId;
        }

        /// <inheritdoc />
        public void RegisterCategoryList(HashSet<NotificationCategory> categoryList)
        {
            if (categoryList is null || categoryList.Any() == false)
            {
                return;
            }

            foreach (var category in categoryList.Where(category =>
                         category.CategoryType != NotificationCategoryType.None))
            {
                _categoryList.Add(category);
            }
        }

        /// <inheritdoc />
        public Task<bool> RequestNotificationPermission(NotificationPermission permission = null)
        {
            return LocalNotificationCenter.RequestNotificationPermissionAsync(permission);
        }
    }
}
