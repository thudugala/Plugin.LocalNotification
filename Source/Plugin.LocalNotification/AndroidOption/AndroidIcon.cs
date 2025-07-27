namespace Plugin.LocalNotification.AndroidOption;

/// <summary>
/// Represents an Android icon resource used for local notifications.
/// </summary>
public class AndroidIcon
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidIcon"/> class with default values.
    /// </summary>
    public AndroidIcon()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidIcon"/> class with the specified resource name.
    /// </summary>
    /// <param name="resourceName">The name of the desired resource.</param>
    public AndroidIcon(string resourceName)
    {
        ResourceName = resourceName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndroidIcon"/> class with the specified resource name and type.
    /// </summary>
    /// <param name="resourceName">The name of the desired resource.</param>
    /// <param name="type">The type of the resource (e.g., "drawable"). If null, uses <see cref="DefaultType"/>.</param>
    public AndroidIcon(string resourceName, string? type)
    {
        ResourceName = resourceName;
        Type = type ?? DefaultType;
    }

    /// <summary>
    /// Gets the default resource type for Android icons ("drawable").
    /// </summary>
    public static string DefaultType => "drawable";

    /// <summary>
    /// Gets or sets the name of the desired resource.
    /// </summary>
    public string ResourceName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the resource type. If not specified, defaults to <see cref="DefaultType"/>.
    /// </summary>
    public string Type { get; set; } = DefaultType;
}