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
        public Task<IList<NotificationRequest>> GetPendingNotificationList()
        {
            IList<NotificationRequest> itemList = NotificationRepository.Current.GetPendingList();
            return Task.FromResult(itemList);
        }

        /// <inheritdoc />
        public void OnNotificationReceived(NotificationEventArgs e)
        {
            NotificationReceived?.Invoke(e);
        }

        /// <inheritdoc />
        public Task<IList<NotificationRequest>> GetDeliveredNotificationList()
        {
            IList<NotificationRequest> itemList = NotificationRepository.Current.GetDeliveredList();
            return Task.FromResult(itemList);
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
                NotificationCenter.Log(ex);
            }
        }

        /// <inheritdoc />
        public bool Cancel(params int[] notificationIdList)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
            {
                return false;
            }

            foreach (var notificationId in notificationIdList)
            {
                var intent = new Intent(Application.Context, typeof(ScheduledAlarmReceiver));
                var alarmIntent = PendingIntent.GetBroadcast(
                    Application.Context,
                    notificationId,
                    intent,
                    PendingIntentFlags.CancelCurrent
                );

                MyAlarmManager?.Cancel(alarmIntent);
                MyNotificationManager?.Cancel(notificationId);
            }

            NotificationRepository.Current.RemoveByPendingIdList(notificationIdList);
            NotificationRepository.Current.RemoveByDeliveredIdList(notificationIdList);
            return true;
        }

        /// <inheritdoc />
        public bool CancelAll()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
            {
                return false;
            }

            var idList = NotificationRepository.Current.GetPendingList().Select(r => r.NotificationId).ToList();
            foreach (var id in idList)
            {
                var intent = new Intent(Application.Context, typeof(ScheduledAlarmReceiver));
                var alarmIntent = PendingIntent.GetBroadcast(
                    Application.Context,
                    id,
                    intent,
                    PendingIntentFlags.CancelCurrent
                );

                MyAlarmManager?.Cancel(alarmIntent);
                alarmIntent?.Cancel();
            }

            MyNotificationManager?.CancelAll();
            NotificationRepository.Current.RemovePendingList();
            NotificationRepository.Current.RemoveDeliveredList();
            return true;
        }

        /// <inheritdoc />
        public Task<bool> Show(Func<NotificationRequestBuilder, NotificationRequest> builder) =>
            Show(builder.Invoke(new NotificationRequestBuilder()));

        /// <inheritdoc />
        public async Task<bool> Show(NotificationRequest notificationRequest)
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

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        internal virtual bool ShowLater(NotificationRequest request)
        {
            // To be consistent with iOS, Do not Schedule notification if NotifyTime is earlier than DateTime.Now
            if (request.Schedule.AndroidIsValidNotifyTime == false)
            {
                NotificationCenter.Log("NotificationServiceImpl.ShowLater: NotifyTime is earlier than DateTime.Now and Allowed Delay, notification ignored");
                return false;
            }

            var dictionaryRequest = NotificationCenter.GetRequestSerializeDictionary(request);

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

            var utcAlarmTimeInMillis =
                (request.Schedule.NotifyTime.GetValueOrDefault().ToUniversalTime() - DateTime.UtcNow)
                .TotalMilliseconds;
            var triggerTime = (long) utcAlarmTimeInMillis;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                MyAlarmManager.SetExactAndAllowWhileIdle(AlarmType.ElapsedRealtime,
                    SystemClock.ElapsedRealtime() + triggerTime, pendingIntent);
            }
            else
            {
                MyAlarmManager.SetExact(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + triggerTime,
                    pendingIntent);
            }

            NotificationRepository.Current.AddPendingRequest(request);

            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        internal virtual async Task<bool> ShowNow(NotificationRequest request)
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
                builder.SetPriority((int) request.Android.Priority);

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
                    var colorResourceId =
                        Application.Context.Resources?.GetIdentifier(request.Android.Color.ResourceName, "color",
                            Application.Context.PackageName) ?? 0;
                    var colorId = Application.Context.GetColor(colorResourceId);
                    builder.SetColor(colorId);
                }
            }

            builder.SetSmallIcon(GetIcon(request.Android.IconSmallName));
            if (request.Android.IconLargeName != null &&
                string.IsNullOrWhiteSpace(request.Android.IconLargeName.ResourceName) == false)
            {
                var largeIcon = await BitmapFactory.DecodeResourceAsync(Application.Context.Resources,
                    GetIcon(request.Android.IconLargeName));
                if (largeIcon != null)
                {
                    builder.SetLargeIcon(largeIcon);
                }
            }

            if (request.Android.TimeoutAfter.HasValue)
            {
                builder.SetTimeoutAfter((long) request.Android.TimeoutAfter.Value.TotalMilliseconds);
            }

            var notificationIntent =
                Application.Context.PackageManager?.GetLaunchIntentForPackage(Application.Context.PackageName ??
                                                                              string.Empty);
            if (notificationIntent is null)
            {
                NotificationCenter.Log("NotificationServiceImpl.ShowNow: notificationIntent is null");
                return false;
            }

            var serializedRequest = NotificationCenter.GetRequestSerialize(request);
            notificationIntent.SetFlags(ActivityFlags.SingleTop);
            notificationIntent.PutExtra(NotificationCenter.ReturnRequest, serializedRequest);
                        
            var pendingIntent = PendingIntent.GetActivity(Application.Context, request.NotificationId,
                notificationIntent,
                ToPendingIntentFlags(request.Android.PendingIntentFlags));
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

            if (request.Silent)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                builder.SetNotificationSilent();
#pragma warning restore CS0618 // Type or member is obsolete
            }

            MyNotificationManager?.Notify(request.NotificationId, notification);

            var args = new NotificationEventArgs
            {
                Request = request
            };
            NotificationCenter.Current.OnNotificationReceived(args);

            NotificationRepository.Current.AddDeliveredRequest(request);

            return true;
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
                return await BitmapFactory.DecodeByteArrayAsync(notificationImage.Binary, 0,
                    notificationImage.Binary.Length);
            }

            return null;
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
            var pendingIntent = CreateActionIntent(serializedRequest, action);
            if (string.IsNullOrWhiteSpace(action.AndroidIconName.ResourceName))
            {
                action.AndroidIconName = request.Android.IconSmallName;
            }

            var nativeAction = new NotificationCompat.Action(GetIcon(action.AndroidIconName),
                new Java.Lang.String(action.Title), pendingIntent);

            return nativeAction;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="serializedRequest"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        protected virtual PendingIntent CreateActionIntent(string serializedRequest, NotificationAction action)
        {
            var intent = new Intent(Application.Context, typeof(NotificationActionReceiver));
            intent.SetAction(NotificationActionReceiver.EntryIntentAction)
                .PutExtra(NotificationActionReceiver.NotificationActionActionId, action.ActionId)
                .PutExtra(NotificationCenter.ReturnRequest, serializedRequest);

            var pendingIntent = PendingIntent.GetBroadcast(
                Application.Context,
                action.ActionId,
                intent,
                ToPendingIntentFlags(action.PendingIntentFlags)
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
                AndroidVisibilityType.Private => (int) NotificationVisibility.Private,
                AndroidVisibilityType.Public => (int) NotificationVisibility.Public,
                AndroidVisibilityType.Secret => (int) NotificationVisibility.Secret,
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

            foreach (var category in categoryList)
            {
                if (category.CategoryType == NotificationCategoryType.None)
                {
                    continue;
                }

                _categoryList.Add(category);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual string ToNativeCategory(NotificationCategoryType type)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected virtual PendingIntentFlags ToPendingIntentFlags(AndroidPendingIntentFlags type)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.S &&
                type.HasFlag(AndroidPendingIntentFlags.Immutable) == false)
            {
                type = type | AndroidPendingIntentFlags.Immutable;
            }
            return (PendingIntentFlags)type;
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
    }
}