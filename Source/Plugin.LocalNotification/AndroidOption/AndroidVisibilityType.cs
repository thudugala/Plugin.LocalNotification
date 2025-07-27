namespace Plugin.LocalNotification.AndroidOption;

/// <summary>
/// Specifies the sphere of visibility for Android notifications, affecting how and when the SystemUI reveals the notification's presence and contents in untrusted situations (such as on the secure lockscreen).
/// </summary>
public enum AndroidVisibilityType
{
    /// <summary>
    /// The default level. The notification's icon and tickerText (if available) are shown in all situations, but the contents are only available if the device is unlocked for the appropriate user.
    /// </summary>
    Private,

    /// <summary>
    /// A more permissive policy. The notification can be read even in an "insecure" context (above a secure lockscreen). Use <c>PublicVersion</c> to redact portions if needed.
    /// </summary>
    Public,

    /// <summary>
    /// Suppresses the notification's icon and ticker until the user has bypassed the lockscreen.
    /// </summary>
    Secret
}