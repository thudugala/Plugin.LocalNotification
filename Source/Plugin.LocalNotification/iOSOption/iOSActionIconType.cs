namespace Plugin.LocalNotification.iOSOption;

/// <summary>
/// Specifies the type of icon for an iOS notification action.
/// </summary>
public enum iOSActionIconType
{
    /// <summary>
    /// No image is set for the icon.
    /// </summary>
    None,

    /// <summary>
    /// Creates an action icon by using a system symbol image.
    /// </summary>
    System,

    /// <summary>
    /// Creates an action icon based on an image in your app’s bundle, preferably in an asset catalog.
    /// </summary>
    Template
}
