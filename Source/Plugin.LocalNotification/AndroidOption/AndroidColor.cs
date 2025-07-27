namespace Plugin.LocalNotification.AndroidOption;

/// <summary>
/// Represents an ARGB color or resource name for notification icons and application name in Android notifications.
/// </summary>
public class AndroidColor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidColor"/> class with default values.
    /// </summary>
    public AndroidColor()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidColor"/> class with the specified ARGB color value.
    /// </summary>
    /// <param name="argb">The ARGB color value.</param>
    public AndroidColor(int argb) => Argb = argb;

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidColor"/> class with the specified resource name.
    /// </summary>
    /// <param name="resourceName">The name of the color resource.</param>
    public AndroidColor(string resourceName) => ResourceName = resourceName;

    /// <summary>
    /// Gets or sets the ARGB color value. If set, <see cref="ResourceName"/> is ignored.
    /// </summary>
    public int? Argb { get; set; }

    /// <summary>
    /// Gets or sets the name of the color resource.
    /// </summary>
    public string ResourceName { get; set; } = string.Empty;
}