namespace Plugin.LocalNotification.iOSOption;

/// <summary>
/// Represents an iOS-specific notification action, including its type and associated icon.
/// </summary>
public class iOSAction
{
    /// <summary>
    /// Gets or sets the type of the iOS notification action.
    /// </summary>
    public iOSActionType Action { get; set; } = iOSActionType.None;

    /// <summary>
    /// Gets or sets the icon associated with the action.
    /// </summary>
    public iOSActionIcon Icon { get; set; } = new ();
}