using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;
using Plugin.LocalNotification.AndroidOption;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Plugin.LocalNotification.Platform.Droid
{
    /// <inheritdoc />
    public class NotificationServiceImpl : INotificationService
    {
        private readonly IList<NotificationCategory> _categoryList = new List<NotificationCategory>();

        /// <summary>
        ///
        /// </summary>
        protected readonly NotificationManager MyNotificationManager;
        
        /// <summary>
        ///
        /// </summary>
        protected readonly AlarmManager MyAlarmManager;

        /// <inheritdoc />
        public event NotificationTappedEventHandler NotificationTapped;

        /// <inheritdoc />
        public event NotificationReceivedEventHandler NotificationReceived;

        /// <inheritdoc />
        public event NotificationActionTappedEventHandler NotificationActionTapped;

        /// <inheritdoc />
        public void OnNotificationTapped(NotificationEventArgs e)
        {
            NotificationTapped?.Invoke(e);
        }

        /// <inheritdoc />
        public Task<IList<int>> PendingNotificationList()
        {
            IList<int> result = new List<int>();
            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public void OnNotificationReceived(NotificationEventArgs e)
        {
            NotificationReceived?.Invoke(e);
        }

        /// <inheritdoc />
        public Task<IList<int>> DeliveredNotificationList()
        {
            IList<int> result = new List<int>();
            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public void OnNotificationActionTapped(NotificationActionEventArgs e)
        {
            NotificationActionTapped?.Invoke(e);
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

                MyNotificationManager =
                    Application.Context.GetSystemService(Context.NotificationService) as NotificationManager ??
                    throw new ApplicationException(Properties.Resources.AndroidNotificationServiceNotFound);

                MyAlarmManager = Application.Context.GetSystemService(Context.AlarmService) as AlarmManager ??
                                 throw new ApplicationException(Properties.Resources.AndroidAlarmServiceNotFound);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        /// <inheritdoc />
        public bool Cancel(int notificationId)
        {
            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
                {
                    return false;
                }

                var intent = new Intent(Application.Context, typeof(ScheduledAlarmReceiver));
                var alarmIntent = PendingIntent.GetBroadcast(
                    Application.Context,
                    notificationId,
                    intent,
                    PendingIntentFlags.CancelCurrent
                );

                MyAlarmManager?.Cancel(alarmIntent);

                MyNotificationManager?.Cancel(notificationId);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return false;
            }
        }

        /// <inheritdoc />
        public bool CancelAll()
        {
            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
                {
                    return false;
                }

                var intent = new Intent(Application.Context, typeof(ScheduledAlarmReceiver));
                var alarmIntent = PendingIntent.GetBroadcast(
                    Application.Context,
                    0,
                    intent,
                    PendingIntentFlags.CancelCurrent
                );

                MyAlarmManager?.Cancel(alarmIntent);
                alarmIntent?.Cancel();

                MyNotificationManager?.CancelAll();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return false;
            }
        }

        /// <inheritdoc />
        public Task<bool> Show(Func<NotificationRequestBuilder, NotificationRequest> builder) => Show(builder.Invoke(new NotificationRequestBuilder()));

        /// <inheritdoc />
        public async Task<bool> Show(NotificationRequest notificationRequest)
        {
            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
                {
                    return false;
                }

                if (notificationRequest is null)
                {
                    return false;
                }

                if (notificationRequest.Schedule.NotifyTime.HasValue)
                {
                    return ShowLater(notificationRequest);
                }

                var result = await ShowNow(notificationRequest);
                return result;
            }
            catch (Exception ex)
            {
                Log(ex.Message);
                return false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        protected virtual bool ShowLater(NotificationRequest request)
        {
            if (request.Schedule.NotifyTime is null ||
                request.Schedule.NotifyTime <= DateTime.Now) // To be consistent with iOS, Do not Schedule notification if NotifyTime is earlier than DateTime.Now
            {
                return false;
            }

            var dictionaryRequest = NotificationCenter.GetRequestSerialize(request);

            var intent = new Intent(Application.Context, typeof(ScheduledAlarmReceiver));
            foreach (var item in dictionaryRequest)
            {
                intent.PutExtra(item.Key, item.Value);
            }
            var pendingIntent = PendingIntent.GetBroadcast(
                Application.Context,
                request.NotificationId,
                intent,
                PendingIntentFlags.UpdateCurrent
            );

            var utcAlarmTimeInMillis = (request.Schedule.NotifyTime.Value.ToUniversalTime() - DateTime.UtcNow).TotalMilliseconds;
            var triggerTime = (long)utcAlarmTimeInMillis;

            if (request.Schedule.RepeatType != NotificationRepeat.No)
            {
                TimeSpan? repeatInterval = null;
                switch (request.Schedule.RepeatType)
                {
                    case NotificationRepeat.Daily:
                        // To be consistent with iOS, Schedule notification next day same time.
                        repeatInterval = TimeSpan.FromDays(1);
                        break;

                    case NotificationRepeat.Weekly:
                        // To be consistent with iOS, Schedule notification next week same day same time.
                        repeatInterval = TimeSpan.FromDays(7);
                        break;

                    case NotificationRepeat.TimeInterval:
                        if (request.Schedule.NotifyRepeatInterval.HasValue)
                        {
                            repeatInterval = request.Schedule.NotifyRepeatInterval.Value;
                        }
                        break;
                }

                if (repeatInterval == null)
                {
                    return true;
                }
                var intervalTime = (long)repeatInterval.Value.TotalMilliseconds;

                MyAlarmManager.SetInexactRepeating(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + triggerTime, intervalTime, pendingIntent);
            }
            else
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                {
                    MyAlarmManager.SetExactAndAllowWhileIdle(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + triggerTime, pendingIntent);
                }
                else
                {
                    MyAlarmManager.SetExact(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + triggerTime, pendingIntent);
                }
            }
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        protected internal virtual async Task<bool> ShowNow(NotificationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Android.ChannelId))
            {
                request.Android.ChannelId = AndroidOptions.DefaultChannelId;
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = MyNotificationManager.GetNotificationChannel(request.Android.ChannelId);
                if (channel is null)
                {
                    NotificationCenter.CreateNotificationChannel(new NotificationChannelRequest
                    {
                        Id = request.Android.ChannelId
                    });
                }
            }

            using var builder = new NotificationCompat.Builder(Application.Context, request.Android.ChannelId);
            builder.SetContentTitle(request.Title);
            builder.SetSubText(request.Subtitle);
            builder.SetContentText(request.Description);
            if (request.Image != null && request.Image.HasValue)
            {
                var imageBitmap = await GetNativeImage(request.Image);
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

            if (string.IsNullOrWhiteSpace(request.Android.Group) == false)
            {
                builder.SetGroup(request.Android.Group);
                if (request.Android.IsGroupSummary)
                {
                    builder.SetGroupSummary(true);
                }
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                if (request.CategoryType != NotificationCategoryType.None)
                {
                    builder.SetCategory(ToNativeCategory(request.CategoryType));
                }

                builder.SetVisibility(ToNativeVisibilityType(request.Android.VisibilityType));
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
                request.Android.IsProgressBarIndeterminate.HasValue)
            {
                builder.SetProgress(request.Android.ProgressBarMax.Value,
                    request.Android.ProgressBarProgress.Value,
                    request.Android.IsProgressBarIndeterminate.Value);
            }

            if (request.Android.Color != null)
            {
                if (request.Android.Color.Argb.HasValue)
                {
                    builder.SetColor(request.Android.Color.Argb.Value);
                }
                else if (string.IsNullOrWhiteSpace(request.Android.Color.ResourceName) == false)
                {
                    var colorResourceId = Application.Context.Resources?.GetIdentifier(request.Android.Color.ResourceName, "color", Application.Context.PackageName) ?? 0;
                    var colorId = Application.Context.GetColor(colorResourceId);
                    builder.SetColor(colorId);
                }
            }

            builder.SetSmallIcon(GetIcon(request.Android.IconSmallName));
            if (request.Android.IconLargeName != null && string.IsNullOrWhiteSpace(request.Android.IconLargeName.ResourceName) == false)
            {
                var largeIcon = await BitmapFactory.DecodeResourceAsync(Application.Context.Resources, GetIcon(request.Android.IconLargeName));
                if (largeIcon != null)
                {
                    builder.SetLargeIcon(largeIcon);
                }
            }

            if (request.Android.TimeoutAfter.HasValue)
            {
                builder.SetTimeoutAfter((long)request.Android.TimeoutAfter.Value.TotalMilliseconds);
            }

            var notificationIntent = Application.Context.PackageManager?.GetLaunchIntentForPackage(Application.Context.PackageName ?? string.Empty);
            if (notificationIntent is null)
            {
                Log($"NotificationServiceImpl.ShowNow: notificationIntent is null");
                return false;
            }

            var serializedRequest = JsonSerializer.Serialize(request);
            notificationIntent.SetFlags(ActivityFlags.SingleTop);
            notificationIntent.PutExtra(NotificationCenter.ReturnRequest, serializedRequest);

            var pendingIntent = PendingIntent.GetActivity(Application.Context, request.NotificationId, notificationIntent,
                PendingIntentFlags.CancelCurrent);
            builder.SetContentIntent(pendingIntent);

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

            var notification = builder.Build();
            if (Build.VERSION.SdkInt < BuildVersionCodes.O &&
                request.Android.LedColor.HasValue)
            {
#pragma warning disable 618
                notification.LedARGB = request.Android.LedColor.Value;
#pragma warning restore 618
            }

            if (Build.VERSION.SdkInt < BuildVersionCodes.O &&
                string.IsNullOrWhiteSpace(request.Sound))
            {
#pragma warning disable 618
                notification.Defaults = NotificationDefaults.All;
#pragma warning restore 618
            }
            MyNotificationManager?.Notify(request.NotificationId, notification);

            var args = new NotificationEventArgs
            {
                Request = request
            };
            NotificationCenter.Current.OnNotificationReceived(args);

            return true;
        }

        private async Task<Bitmap> GetNativeImage(NotificationImage notificationImage)
        {
            if (notificationImage is null || notificationImage.HasValue == false)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(notificationImage.ResourceName) == false)
            {
                var imageId = Application.Context.Resources?.GetIdentifier(notificationImage.ResourceName, AndroidIcon.DefaultType, Application.Context.PackageName);
                if (imageId != null)
                {
                    return await BitmapFactory.DecodeResourceAsync(Application.Context.Resources, imageId.Value);
                }
            }
            if (string.IsNullOrWhiteSpace(notificationImage.FilePath) == false)
            {
                if (File.Exists(notificationImage.FilePath))
                {
                    return await BitmapFactory.DecodeFileAsync(notificationImage.FilePath);
                }
            }
            if (notificationImage.Binary != null && notificationImage.Binary.Length > 0)
            {
                return await BitmapFactory.DecodeByteArrayAsync(notificationImage.Binary, 0, notificationImage.Binary.Length);
            }
            return null;
        }

        private NotificationCompat.Action CreateAction(NotificationRequest request, string serializedRequest, NotificationAction action)
        {
            var pendingIntent = CreateActionIntent(serializedRequest, action);
            var nativeAction = new NotificationCompat.Action(GetIcon(request.Android.IconSmallName), new Java.Lang.String(action.Title), pendingIntent);

            return nativeAction;
        }

        private PendingIntent CreateActionIntent(string serializedRequest, NotificationAction action)
        {
            var intent = new Intent(Application.Context, typeof(NotificationActionReceiver));
            intent.SetAction(NotificationActionReceiver.EntryIntentAction)
                .PutExtra(NotificationActionReceiver.NotificationActionActionId, action.ActionId)
                .PutExtra(NotificationCenter.ReturnRequest, serializedRequest);

            var pendingIntent = PendingIntent.GetBroadcast(
                Application.Context,
                action.ActionId,
                intent,
                PendingIntentFlags.CancelCurrent
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
        /// <param name="type"></param>
        /// <returns></returns>
        protected static int ToNativeVisibilityType(AndroidVisibilityType type)
        {
            return type switch
            {
                AndroidVisibilityType.Private => (int)NotificationVisibility.Private,
                AndroidVisibilityType.Public => (int)NotificationVisibility.Public,
                AndroidVisibilityType.Secret => (int)NotificationVisibility.Secret,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

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

                iconId = Application.Context.Resources?.GetIdentifier(icon.ResourceName, icon.Type, Application.Context.PackageName) ?? 0;
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

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        protected static void Log(string message)
        {
            Android.Util.Log.Info(Application.Context.PackageName, message);
        }

        /// <inheritdoc />
        public void RegisterCategoryList(HashSet<NotificationCategory> categoryList)
        {
            if (categoryList is null || categoryList.Any() == false)
            {
                return;
            }

            foreach (var category in categoryList)
            {
                if (category.CategoryType == NotificationCategoryType.None)
                {
                    continue;
                }

                _categoryList.Add(category);
            }
        }

        private string ToNativeCategory(NotificationCategoryType type)
        {
            return type switch
            {
                NotificationCategoryType.Alarm => NotificationCompat.CategoryAlarm,
                NotificationCategoryType.Status => NotificationCompat.CategoryStatus,
                NotificationCategoryType.Reminder => NotificationCompat.CategoryReminder,
                NotificationCategoryType.Event => NotificationCompat.CategoryEvent,
                NotificationCategoryType.Error => NotificationCompat.CategoryError,
                NotificationCategoryType.Progress => NotificationCompat.CategoryProgress,
                NotificationCategoryType.Promo => NotificationCompat.CategoryPromo,
                NotificationCategoryType.Recommendation => NotificationCompat.CategoryRecommendation,
                NotificationCategoryType.Service => NotificationCompat.CategoryService,
                _ => NotificationCompat.CategoryStatus
            };
        }
    }
}