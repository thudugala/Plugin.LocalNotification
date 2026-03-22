namespace Plugin.LocalNotification.Core.Models.AppleOption;

/// <summary>
/// Specifies the location authorization options for IOS notifications.
/// </summary>
public enum AppleLocationAuthorization
{
    /// <summary>
    /// No location authorization is granted.
    /// </summary>
    No,

    /// <summary>
    /// Location authorization is always granted.
    /// </summary>
    Always,

    /// <summary>
    /// Location authorization is granted only when the app is in use.
    /// </summary>
    WhenInUse
}