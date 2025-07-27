namespace Plugin.LocalNotification.iOSOption;

/// <summary>
/// Specifies the behaviors that can be applied to an iOS notification action.
/// </summary>
public enum iOSActionType
{
    /// <summary>
    /// No behavior is set for the action.
    /// </summary>
    None,

    /// <summary>
    /// The action performs a destructive task.
    /// </summary>
    Destructive,

    /// <summary>
    /// The action can be performed only on an unlocked device.
    /// </summary>
    AuthenticationRequired,

    /// <summary>
    /// The action causes the app to launch in the foreground.
    /// </summary>
    Foreground
}