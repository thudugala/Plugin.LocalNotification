namespace Plugin.LocalNotification.Core.Models.AppleOption;

/// <summary>
/// Represents notification permission options for IOS, including notification and location authorization settings.
/// </summary>
public class AppleNotificationPermission
{
    /// <summary>
    /// Gets or sets the notification authorization options for IOS notifications. Default is <see cref="AppleAuthorizationOptions.Alert"/>, <see cref="AppleAuthorizationOptions.Badge"/>, and <see cref="AppleAuthorizationOptions.Sound"/>.
    /// </summary>
    public AppleAuthorizationOptions NotificationAuthorization { get; set; } = AppleAuthorizationOptions.Alert |
        AppleAuthorizationOptions.Badge |
        AppleAuthorizationOptions.Sound;

    /// <summary>
    /// Gets or sets the location authorization options for IOS notifications. Default is <see cref="AppleLocationAuthorization.No"/>.
    /// </summary>
    public AppleLocationAuthorization LocationAuthorization { get; set; } = AppleLocationAuthorization.No;
}