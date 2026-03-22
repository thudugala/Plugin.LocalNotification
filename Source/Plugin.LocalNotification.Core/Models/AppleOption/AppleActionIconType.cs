namespace Plugin.LocalNotification.Core.Models.AppleOption;

/// <summary>
/// Specifies the type of icon for an IOS notification action.
/// </summary>
public enum AppleActionIconType
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
