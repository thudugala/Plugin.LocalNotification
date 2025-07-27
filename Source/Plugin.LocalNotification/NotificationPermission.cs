using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;

namespace Plugin.LocalNotification;

/// <summary>
/// Represents notification permission options for different platforms, including Android and iOS.
/// </summary>
public class NotificationPermission
{
    /// <summary>
    /// Gets or sets a value indicating whether to ask the user for notification permission. Default is <c>true</c>.
    /// </summary>
    public bool AskPermission { get; set; } = true;

    /// <summary>
    /// Gets or sets Android-specific notification permission options.
    /// </summary>
    public AndroidNotificationPermission Android { get; set; } = new();

    /// <summary>
    /// Gets or sets iOS-specific notification permission options.
    /// </summary>
    public iOSNotificationPermission IOS { get; set; } = new();
}