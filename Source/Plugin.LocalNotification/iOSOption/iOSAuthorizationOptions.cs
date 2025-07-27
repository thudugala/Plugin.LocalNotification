namespace Plugin.LocalNotification.iOSOption;

/// <summary>
/// Specifies the authorization options for iOS notifications, such as badge, sound, alert, and more.
/// </summary>
[Flags]
public enum iOSAuthorizationOptions : ulong
{
    /// <summary>
    /// No authorization options are set.
    /// </summary>
    None = 0,

    /// <summary>
    /// The app may update its badge.
    /// </summary>
    Badge = 1,

    /// <summary>
    /// The app may play sounds for notifications.
    /// </summary>
    Sound = 2,

    /// <summary>
    /// The app may display alerts for notifications.
    /// </summary>
    Alert = 4,

    /// <summary>
    /// The app may display notifications in CarPlay.
    /// </summary>
    CarPlay = 8,

    /// <summary>
    /// The app may display critical alerts.
    /// </summary>
    CriticalAlert = 16,

    /// <summary>
    /// The app provides its own notification settings.
    /// </summary>
    ProvidesAppNotificationSettings = 32,

    /// <summary>
    /// The app may display provisional notifications.
    /// </summary>
    Provisional = 64,

    /// <summary>
    /// The app may display announcement notifications.
    /// </summary>
    Announcement = 128,

    /// <summary>
    /// The app may display time-sensitive notifications.
    /// </summary>
    TimeSensitive = 256
}