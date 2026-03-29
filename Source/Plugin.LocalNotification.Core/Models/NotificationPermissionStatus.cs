namespace Plugin.LocalNotification.Core.Models;

/// <summary>
/// Represents a detailed breakdown of the notification permissions currently granted to the app.
/// Returned by <c>INotificationService.GetNotificationPermissionStatus</c>.
/// </summary>
public class NotificationPermissionStatus
{
    /// <summary>
    /// Gets a value indicating whether notifications are globally enabled for the app.
    /// On Apple platforms this reflects whether the user has granted authorization.
    /// On Android this reflects <c>NotificationManager.AreNotificationsEnabled()</c>.
    /// On Windows this reflects <c>AppNotificationManager.Setting == Enabled</c>.
    /// </summary>
    public bool IsEnabled { get; init; }

    /// <summary>
    /// Gets a value indicating whether alert/banner notifications are enabled.
    /// Maps to <c>UNNotificationSettings.AlertSetting == Enabled</c> on Apple platforms.
    /// Matches <see cref="IsEnabled"/> on Android and Windows.
    /// </summary>
    public bool IsAlertEnabled { get; init; }

    /// <summary>
    /// Gets a value indicating whether notification sounds are enabled.
    /// Maps to <c>UNNotificationSettings.SoundSetting == Enabled</c> on Apple platforms.
    /// Matches <see cref="IsEnabled"/> on Android and Windows.
    /// </summary>
    public bool IsSoundEnabled { get; init; }

    /// <summary>
    /// Gets a value indicating whether badge updates are enabled.
    /// Maps to <c>UNNotificationSettings.BadgeSetting == Enabled</c> on Apple platforms.
    /// Matches <see cref="IsEnabled"/> on Android and Windows.
    /// </summary>
    public bool IsBadgeEnabled { get; init; }

    /// <summary>
    /// Gets a value indicating whether the app has provisional authorization, meaning
    /// notifications are delivered quietly without an explicit user prompt.
    /// Apple platforms only (iOS 12+ / macOS 10.14+). Always <c>false</c> on other platforms.
    /// </summary>
    public bool IsProvisionalEnabled { get; init; }

    /// <summary>
    /// Gets a value indicating whether critical alerts are enabled.
    /// Maps to <c>UNNotificationSettings.CriticalAlertSetting == Enabled</c>.
    /// Apple platforms only (iOS 12+ / macOS 10.14+). Always <c>false</c> on other platforms.
    /// </summary>
    public bool IsCriticalAlertEnabled { get; init; }

    /// <summary>
    /// Gets a value indicating whether CarPlay notifications are enabled.
    /// Maps to <c>UNNotificationSettings.CarPlaySetting == Enabled</c>.
    /// Apple platforms only. Always <c>false</c> on other platforms.
    /// </summary>
    public bool IsCarPlayEnabled { get; init; }

    /// <summary>
    /// Gets a value indicating whether time-sensitive notifications are enabled.
    /// Maps to <c>UNNotificationSettings.TimeSensitiveSetting == Enabled</c>.
    /// Apple platforms only (iOS 15+ / macOS 12+). Always <c>false</c> on other platforms.
    /// </summary>
    public bool IsTimeSensitiveEnabled { get; init; }

    /// <summary>
    /// Gets a value indicating whether the app can schedule exact alarms.
    /// On Android 12+ (API 31+) this reflects <c>AlarmManager.CanScheduleExactAlarms()</c>.
    /// Always <c>true</c> on Android versions below API 31 and on all other platforms.
    /// </summary>
    public bool CanScheduleExactAlarms { get; init; }
}
