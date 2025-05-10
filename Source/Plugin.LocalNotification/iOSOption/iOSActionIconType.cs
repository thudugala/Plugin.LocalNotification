namespace Plugin.LocalNotification.iOSOption;

/// <summary>
/// 
/// </summary>
public enum iOSActionIconType
{
    /// <summary>
    /// No Image is set
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
