namespace Plugin.LocalNotification.Core.Models.AppleOption;

/// <summary>
/// Represents an IOS-specific notification action, including its type and associated icon.
/// </summary>
public class AppleAction
{
    /// <summary>
    /// Gets or sets the type of the IOS notification action.
    /// </summary>
    public AppleActionType Action { get; set; } = AppleActionType.None;

    /// <summary>
    /// Gets or sets the icon associated with the action.
    /// </summary>
    public AppleActionIcon Icon { get; set; } = new ();
}