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

    /// <summary>
    /// When set, a text-input action (<c>UNTextInputNotificationAction</c>) is created instead of
    /// a plain button. This value becomes the title of the Send/Submit button.
    /// Leave <c>null</c> or empty for a regular button action.
    /// </summary>
    public string? TextInputButtonTitle { get; set; }

    /// <summary>
    /// Placeholder text shown inside the text field when <see cref="TextInputButtonTitle"/> is set.
    /// </summary>
    public string? TextInputPlaceholder { get; set; }
}