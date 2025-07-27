namespace Plugin.LocalNotification.iOSOption;

/// <summary>
/// Specifies the location authorization options for iOS notifications.
/// </summary>
public enum iOSLocationAuthorization
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