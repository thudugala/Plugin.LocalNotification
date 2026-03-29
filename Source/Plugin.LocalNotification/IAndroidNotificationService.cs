using Plugin.LocalNotification.Core.Models.AndroidOption;

namespace Plugin.LocalNotification;

/// <summary>
/// Extends <see cref="INotificationService"/> with Android-specific permission and channel management APIs.
/// Cast <see cref="LocalNotificationCenter.Current"/> to this interface, or use
/// <see cref="LocalNotificationCenter.Android"/>, to access these members on Android.
/// </summary>
public interface IAndroidNotificationService : INotificationService
{
    /// <summary>
    /// Returns whether the app can schedule exact notifications (Android 12+ / API 31+).
    /// </summary>
    Task<bool> CanScheduleExactNotifications();

    /// <summary>
    /// Opens the system settings page that lets the user grant the SCHEDULE_EXACT_ALARM permission (Android 12+ / API 31+).
    /// Returns whether exact alarms are permitted after the screen is dismissed.
    /// </summary>
    Task<bool> RequestExactAlarmsPermission();

    /// <summary>
    /// Opens the system settings page that lets the user grant the USE_FULL_SCREEN_INTENT permission (Android 14+ / API 34+).
    /// Returns whether full-screen intent is permitted after the screen is dismissed.
    /// </summary>
    Task<bool> RequestFullScreenIntentPermission();

    /// <summary>
    /// Deletes the notification channel with the given identifier (Android 8+ / API 26+ only).
    /// </summary>
    Task DeleteNotificationChannel(string channelId);

    /// <summary>
    /// Returns all notification channels currently registered with the system (Android 8+ / API 26+).
    /// </summary>
    Task<IList<AndroidNotificationChannelRequest>> GetNotificationChannels();

    /// <summary>
    /// Deletes the notification channel group with the given identifier (Android 8+ / API 26+ only).
    /// </summary>
    Task DeleteNotificationChannelGroup(string groupId);
}
