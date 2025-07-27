namespace Plugin.LocalNotification.iOSOption;

/// <summary>
/// Represents notification permission options for iOS, including notification and location authorization settings.
/// </summary>
public class iOSNotificationPermission
{
    /// <summary>
    /// Gets or sets the notification authorization options for iOS notifications. Default is <see cref="iOSAuthorizationOptions.Alert"/>, <see cref="iOSAuthorizationOptions.Badge"/>, and <see cref="iOSAuthorizationOptions.Sound"/>.
    /// </summary>
    public iOSAuthorizationOptions NotificationAuthorization { get; set; } = iOSAuthorizationOptions.Alert |
        iOSAuthorizationOptions.Badge |
        iOSAuthorizationOptions.Sound;

    /// <summary>
    /// Gets or sets the location authorization options for iOS notifications. Default is <see cref="iOSLocationAuthorization.No"/>.
    /// </summary>
    public iOSLocationAuthorization LocationAuthorization { get; set; } = iOSLocationAuthorization.No;
}