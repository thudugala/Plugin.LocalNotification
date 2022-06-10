using Android.App;
using Android.OS;
using AndroidX.Core.App;
using Plugin.LocalNotification.AndroidOption;
using System;

namespace Plugin.LocalNotification.Platforms
{
    public static class PlatformExtensions
    {

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int ToNative(this AndroidVisibilityType type)
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
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToNative(this NotificationCategoryType type)
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
        public static AlarmType ToNative(this AndroidAlarmType type)
        {
            return type switch
            {
                AndroidAlarmType.Rtc => AlarmType.Rtc,
                AndroidAlarmType.RtcWakeup => AlarmType.RtcWakeup,
                AndroidAlarmType.ElapsedRealtime => AlarmType.ElapsedRealtime,
                AndroidAlarmType.ElapsedRealtimeWakeup => AlarmType.ElapsedRealtimeWakeup,
                _ => AlarmType.Rtc
            };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PendingIntentFlags ToNative(this AndroidPendingIntentFlags type)
        {
            return ((PendingIntentFlags)type).SetImmutableIfNeeded();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PendingIntentFlags SetImmutableIfNeeded(this PendingIntentFlags type)
        {
            if ((int)Build.VERSION.SdkInt >= 31 &&
                type.HasFlag(PendingIntentFlags.Immutable) == false)
            {
                type |= PendingIntentFlags.Immutable;
            }

            return type;
        }
    }
}
