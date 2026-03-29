using Android.App;
using Android.Media;
using AndroidX.Core.App;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.Core.Models.AndroidOption;
using Application = Android.App.Application;
using MediaStream = Android.Media.Stream;

namespace Plugin.LocalNotification.Core.Platforms.Android;

/// <summary>
///
/// </summary>
public static class AndroidPlatformExtensions
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

    /// <summary>
    /// Converts a native <see cref="NotificationImportance"/> value back to an <see cref="AndroidImportance"/>.
    /// </summary>
    public static AndroidImportance ToLocalNotificationImportance(this NotificationImportance importance)
    {
        return !OperatingSystem.IsAndroidVersionAtLeast(26)
            ? AndroidImportance.Unspecified
            : importance switch
        {
            NotificationImportance.None => AndroidImportance.None,
            NotificationImportance.Min => AndroidImportance.Min,
            NotificationImportance.Low => AndroidImportance.Low,
            NotificationImportance.Default => AndroidImportance.Default,
            NotificationImportance.High => AndroidImportance.High,
            NotificationImportance.Max => AndroidImportance.Max,
            _ => AndroidImportance.Unspecified
        };
    }

    /// <summary>
    /// Converts an <see cref="AndroidAudioAttributeUsage"/> value to its native <see cref="AudioUsageKind"/> equivalent.
    /// Used when building <see cref="Android.Media.AudioAttributes"/> for notification channels (API 26+).
    /// </summary>
    public static AudioUsageKind ToNative(this AndroidAudioAttributeUsage usage) => usage switch
    {
        AndroidAudioAttributeUsage.Alarm => AudioUsageKind.Alarm,
        AndroidAudioAttributeUsage.NotificationRingtone => AudioUsageKind.NotificationRingtone,
        AndroidAudioAttributeUsage.Media => AudioUsageKind.Media,
        AndroidAudioAttributeUsage.VoiceCommunication => AudioUsageKind.VoiceCommunication,
        AndroidAudioAttributeUsage.Unknown => AudioUsageKind.Unknown,
        _ => AudioUsageKind.Notification,
    };

    /// <summary>
    /// Converts an <see cref="AndroidAudioAttributeUsage"/> value to the legacy audio stream type integer
    /// used by <c>NotificationCompat.Builder.SetSound(Uri, int)</c> on Android API &lt; 26.
    /// </summary>
    public static int ToStreamType(this AndroidAudioAttributeUsage usage) => usage switch
    {
        AndroidAudioAttributeUsage.Alarm => (int)MediaStream.Alarm,        
        AndroidAudioAttributeUsage.NotificationRingtone => (int)MediaStream.Ring,
        AndroidAudioAttributeUsage.Media => (int)MediaStream.Music,
        AndroidAudioAttributeUsage.VoiceCommunication => (int)MediaStream.VoiceCall,
        _ => (int)MediaStream.Notification,
    };
}