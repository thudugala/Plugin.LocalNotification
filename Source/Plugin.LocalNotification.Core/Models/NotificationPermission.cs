using Plugin.LocalNotification.Core.Models.AndroidOption;
using Plugin.LocalNotification.Core.Models.AppleOption;

namespace Plugin.LocalNotification.Core.Models;

/// <summary>
/// Represents notification permission options for different platforms, including Android and IOS.
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
    /// Gets or sets IOS-specific notification permission options.
    /// </summary>
    public AppleNotificationPermission Apple { get; set; } = new();
}