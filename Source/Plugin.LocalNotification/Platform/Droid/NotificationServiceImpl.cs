using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Work;
using Java.Util.Concurrent;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public event NotificationActionTappedEventHandler NotificationActionTapped;

        /// <inheritdoc />
        public void OnNotificationTapped(NotificationEventArgs e)
        {
            NotificationTapped?.Invoke(e);
        }

        /// <inheritdoc />
        public void OnNotificationReceived(NotificationEventArgs e)
        {
            NotificationReceived?.Invoke(e);
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
        public bool Cancel(int notificationId)
        {
            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
                {
                    return false;
                }

                WorkManager?.CancelAllWorkByTag(notificationId.ToString(CultureInfo.CurrentCulture));
                NotificationManager?.Cancel(notificationId);
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

                WorkManager?.CancelAllWork();
                NotificationManager?.CancelAll();
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
        public Task<bool> Show(NotificationRequest notificationRequest)
        {
            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
                {
                    return Task.FromResult(false);
                }

                if (notificationRequest is null)
                {
                    return Task.FromResult(false);
                }

                var result = notificationRequest.Schedule.NotifyTime.HasValue ? ShowLater(notificationRequest) : ShowNow(notificationRequest);
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return Task.FromResult(false);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="notificationRequest"></param>
        protected virtual bool ShowLater(NotificationRequest notificationRequest)
        {
            if (notificationRequest.Schedule.NotifyTime is null ||
                notificationRequest.Schedule.NotifyTime.Value <= DateTime.Now) // To be consistent with iOS, Do not Schedule notification if NotifyTime is earlier than DateTime.Now
            {
                return false;
            }

            Cancel(notificationRequest.NotificationId);

            return EnqueueWorker(notificationRequest);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        protected internal virtual bool ShowNow(NotificationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Android.ChannelId))
            {
                request.Android.ChannelId = AndroidOptions.DefaultChannelId;
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = NotificationManager.GetNotificationChannel(request.Android.ChannelId);
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

            if (string.IsNullOrWhiteSpace(request.Android.Color) == false)
            {
                var colorId = Application.Context.Resources?.GetIdentifier(request.Android.Color, "color", Application.Context.PackageName) ?? 0;
                builder.SetColor(colorId);
            }

            builder.SetSmallIcon(GetIcon(request.Android.IconSmallName));
            if (request.Android.IconLargeName != null && string.IsNullOrWhiteSpace(request.Android.IconLargeName.Name) == false)
            {
                builder.SetLargeIcon(BitmapFactory.DecodeResource(Application.Context.Resources, GetIcon(request.Android.IconLargeName)));
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
            NotificationManager?.Notify(request.NotificationId, notification);

            var args = new NotificationEventArgs
            {
                Request = request
            };
            NotificationCenter.Current.OnNotificationReceived(args);

            return true;
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
        /// <param name="request"></param>
        protected internal virtual bool EnqueueWorker(NotificationRequest request)
        {
            if (!request.Schedule.NotifyTime.HasValue)
            {
                Log($"{nameof(request.Schedule.NotifyTime)} value doesn't set!");
                return false;
            }

            var notifyTime = request.Schedule.NotifyTime.Value;

            using var dataBuilder = new Data.Builder();
            var dictionary = NotificationCenter.GetRequestSerialize(request);
            foreach (var item in dictionary)
            {
                dataBuilder.PutString(item.Key, item.Value);
            }
            var data = dataBuilder.Build();
            var tag = request.NotificationId.ToString(CultureInfo.CurrentCulture);
            var diff = (long)(notifyTime - DateTime.Now).TotalMilliseconds;

            var workRequest = OneTimeWorkRequest.Builder.From<ScheduledNotificationWorker>()
                .AddTag(tag)
                .SetInputData(data)
                .SetInitialDelay(diff, TimeUnit.Milliseconds)
                .Build();

            WorkManager?.Enqueue(workRequest);
            return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        protected static int GetIcon(AndroidNotificationIcon icon)
        {
            var iconId = 0;
            if (icon != null && string.IsNullOrWhiteSpace(icon.Name) == false)
            {
                if (string.IsNullOrWhiteSpace(icon.Type))
                {
                    icon.Type = AndroidNotificationIcon.DefaultType;
                }

                iconId = Application.Context.Resources?.GetIdentifier(icon.Name, icon.Type, Application.Context.PackageName) ?? 0;
            }

            if (iconId != 0)
            {
                return iconId;
            }

            iconId = Application.Context.ApplicationInfo?.Icon ?? 0;
            if (iconId == 0)
            {
                iconId = Application.Context.Resources?.GetIdentifier("icon", AndroidNotificationIcon.DefaultType,
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
        public void RegisterCategoryList(IList<NotificationCategory> categoryList)
        {
            if (categoryList is null || categoryList.Any() == false)
            {
                return;
            }

            var categorySet = new HashSet<NotificationCategoryType>();
            foreach (var category in categoryList)
            {
                if (category.CategoryType == NotificationCategoryType.None)
                {
                    continue;
                }

                if (categorySet.Contains(category.CategoryType))
                {
                    continue;
                }

                categorySet.Add(category.CategoryType);

                var duplicateActionList = new List<NotificationAction>();
                var actionIdSet = new HashSet<int>();
                foreach (var notificationAction in category.ActionList)
                {
                    if (actionIdSet.Contains(notificationAction.ActionId))
                    {
                        duplicateActionList.Add(notificationAction);
                    }
                    else
                    {
                        actionIdSet.Add(notificationAction.ActionId);
                    }
                }

                if (duplicateActionList.Any())
                {
                    foreach (var duplicateAction in duplicateActionList)
                    {
                        category.ActionList.Remove(duplicateAction);
                    }
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
                NotificationCategoryType.StopWatch => NotificationCompat.CategoryStopwatch,
                NotificationCategoryType.Progress => NotificationCompat.CategoryProgress,
                NotificationCategoryType.Promo => NotificationCompat.CategoryPromo,
                NotificationCategoryType.Recommendation => NotificationCompat.CategoryRecommendation,
                NotificationCategoryType.Service => NotificationCompat.CategoryService,
                _ => NotificationCompat.CategoryStatus
            };
        }
    }
}