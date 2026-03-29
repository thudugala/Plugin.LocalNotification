using Plugin.LocalNotification.Core.Models.AndroidOption;

namespace Plugin.LocalNotification;

/// <summary>
/// Extends <see cref="INotificationService"/> with Android-specific permission and channel management APIs.
/// Cast <see cref="LocalNotificationCenter.Current"/> to this interface, or use
/// <see cref="LocalNotificationCenter.AndroidService"/>, to access these members on Android.
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

    /// <summary>
    /// Starts an Android foreground service that keeps the app alive while background work is running
    /// and shows a persistent notification to the user.
    /// <para>
    /// The app's <c>AndroidManifest.xml</c> must declare:
    /// <list type="bullet">
    ///   <item><description><c>&lt;uses-permission android:name="android.permission.FOREGROUND_SERVICE" /&gt;</c></description></item>
    ///   <item><description>A <c>&lt;service&gt;</c> element for <c>plugin.LocalNotification.NotificationForegroundService</c></description></item>
    ///   <item><description>Any additional <c>android.permission.FOREGROUND_SERVICE_*</c> permission required by the chosen <see cref="AndroidForegroundServiceType"/>.</description></item>
    /// </list>
    /// </para>
    /// Returns <see langword="true"/> when the service was started successfully.
    /// </summary>
    Task<bool> StartForegroundServiceAsync(AndroidForegroundServiceRequest request);

    /// <summary>
    /// Stops the foreground service previously started with <see cref="StartForegroundServiceAsync"/>.
    /// The persistent notification is removed and the service process is ended.
    /// </summary>
    Task StopForegroundServiceAsync();
}
