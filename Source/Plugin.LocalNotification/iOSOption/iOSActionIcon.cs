namespace Plugin.LocalNotification.iOSOption;

/// <summary>
/// Represents an icon for an iOS notification action, including its type and image name.
/// </summary>
public class iOSActionIcon
{
    /// <summary>
    /// Gets or sets the type of the icon. Default is <see cref="iOSActionIconType.None"/>.
    /// </summary>
    public iOSActionIconType Type { get; set; } = iOSActionIconType.None;

    /// <summary>
    /// Gets or sets the image name for the icon.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
