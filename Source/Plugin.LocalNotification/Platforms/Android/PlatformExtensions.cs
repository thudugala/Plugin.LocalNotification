using Android.App;
using Android.Content;
using AndroidX.Core.App;
using Plugin.LocalNotification.AndroidOption;
using Application = Android.App.Application;

namespace Plugin.LocalNotification.Platforms
{
    /// <summary>
    ///
    /// </summary>
    public static class PlatformExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static int ToNative(this AndroidColor color)
        {
            if (color is null)
            {
                return 0;
            }

            if (color.Argb.HasValue)
            {
                return color.Argb.Value;
            }

            if (string.IsNullOrWhiteSpace(color.ResourceName) == false)
            {
                if (OperatingSystem.IsAndroidVersionAtLeast(23))
                {
                    var colorResourceId =
                    Application.Context.Resources?.GetIdentifier(color.ResourceName, "color",
                        Application.Context.PackageName) ?? 0;

                    var colorId = Application.Context.GetColor(colorResourceId);

                    return colorId;
                }
            }
            return 0;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static NotificationImportance ToNative(this AndroidImportance type) => !OperatingSystem.IsAndroidVersionAtLeast(26)
                ? default
                : type switch
                {
                    AndroidImportance.Unspecified => NotificationImportance.Unspecified,
                    AndroidImportance.None => NotificationImportance.None,
                    AndroidImportance.Min => NotificationImportance.Min,
                    AndroidImportance.Low => NotificationImportance.Low,
                    AndroidImportance.Default => NotificationImportance.Default,
                    AndroidImportance.High => NotificationImportance.High,
                    AndroidImportance.Max => NotificationImportance.Max,
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                };

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static NotificationVisibility ToNative(this AndroidVisibilityType type) => type switch
        {
            AndroidVisibilityType.Private => NotificationVisibility.Private,
            AndroidVisibilityType.Public => NotificationVisibility.Public,
            AndroidVisibilityType.Secret => NotificationVisibility.Secret,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToNative(this NotificationCategoryType type) => type switch
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

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static AlarmType ToNative(this AndroidAlarmType type) => type switch
        {
            AndroidAlarmType.Rtc => AlarmType.Rtc,
            AndroidAlarmType.RtcWakeup => AlarmType.RtcWakeup,
            AndroidAlarmType.ElapsedRealtime => AlarmType.ElapsedRealtime,
            AndroidAlarmType.ElapsedRealtimeWakeup => AlarmType.ElapsedRealtimeWakeup,
            _ => AlarmType.Rtc
        };

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PendingIntentFlags ToNative(this AndroidPendingIntentFlags type) => ((PendingIntentFlags)type).SetImmutableIfNeeded();

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PendingIntentFlags SetImmutableIfNeeded(this PendingIntentFlags type)
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(31) &&
                type.HasFlag(PendingIntentFlags.Immutable) == false)
            {
                type |= PendingIntentFlags.Immutable;
            }

            return type;
        }

        internal static bool IsValidResource(this Android.Net.Uri uri, Context context)
        {
            var contentResolver = context.ContentResolver;
            if (contentResolver is null)
            {
                return false;
            }

            try
            {
                contentResolver.OpenInputStream(uri)?.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}