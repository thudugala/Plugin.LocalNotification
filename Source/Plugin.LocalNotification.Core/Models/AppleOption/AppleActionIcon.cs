namespace Plugin.LocalNotification.Core.Models.AppleOption;

/// <summary>
/// Represents an icon for an IOS notification action, including its type and image name.
/// </summary>
public class AppleActionIcon
{
    /// <summary>
    /// Gets or sets the type of the icon. Default is <see cref="AppleActionIconType.None"/>.
    /// </summary>
    public AppleActionIconType Type { get; set; } = AppleActionIconType.None;

    /// <summary>
    /// Gets or sets the image name for the icon.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
