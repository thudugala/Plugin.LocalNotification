namespace Plugin.LocalNotification.Core.Models.AppleOption;

/// <summary>
/// Defines a normalized clipping rect for an iOS/macOS notification attachment thumbnail.
/// All values are in the range 0.0 to 1.0 relative to the image dimensions.
/// Maps to the <c>UNNotificationAttachmentOptionsThumbnailClippingRectKey</c> option (iOS 10+).
/// </summary>
public class AppleAttachmentThumbnailClippingRect
{
    /// <summary>
    /// The normalized x-origin of the clipping rect (0.0 to 1.0).
    /// </summary>
    public double X { get; set; }

    /// <summary>
    /// The normalized y-origin of the clipping rect (0.0 to 1.0).
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// The normalized width of the clipping rect (0.0 to 1.0). Default is 1.0 (full width).
    /// </summary>
    public double Width { get; set; } = 1.0;

    /// <summary>
    /// The normalized height of the clipping rect (0.0 to 1.0). Default is 1.0 (full height).
    /// </summary>
    public double Height { get; set; } = 1.0;
}
