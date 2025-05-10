namespace Plugin.LocalNotification.AndroidOption;

/// <summary>
/// Sphere of visibility of this notification,
/// which affects how and when the SystemUI reveals the notification's presence and contents in untrusted situations (namely, on the secure lockscreen).
/// </summary>
public enum AndroidVisibilityType
{
    /// <summary>
    /// The default level,
    /// behaves exactly as notifications have always done on Android: The notification's icon and tickerText (if available) are shown in all situations,
    /// but the contents are only available if the device is unlocked for the appropriate user.
    /// </summary>
    Private,

    /// <summary>
    /// A more permissive policy can be expressed by Public;
    /// such a notification can be read even in an "insecure" context (that is, above a secure lockscreen).
    /// To modify the public version of this notification—for example, to redact some portions—see PublicVersion.
    /// </summary>
    Public,

    /// <summary>
    /// Will suppress its icon and ticker until the user has bypassed the lockscreen
    /// </summary>
    Secret
}