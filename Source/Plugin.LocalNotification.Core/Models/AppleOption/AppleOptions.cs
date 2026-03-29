using System.Text.Json.Serialization;

namespace Plugin.LocalNotification.Core.Models.AppleOption;

/// <summary>
/// Represents notification request options for IOS, including display, sound, badge, priority, and summary settings.
/// </summary>
public class AppleOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to prevent IOS from displaying the default banner when a notification is received in the foreground. Default is <c>false</c>.
    /// </summary>
    public bool HideForegroundAlert { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to enable IOS to play the default notification sound even if the app is in the foreground. Default is <c>true</c>.
    /// </summary>
    public bool PlayForegroundSound { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to present the notification as a banner. Default is <c>true</c>.
    /// </summary>
    public bool PresentAsBanner { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to show the notification in Notification Center. Default is <c>true</c>.
    /// </summary>
    public bool ShowInNotificationCenter { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to apply the notification's badge value to the app’s icon. Default is <c>true</c>.
    /// </summary>
    public bool ApplyBadgeValue { get; set; } = true;

    /// <summary>
    /// Gets or sets the priority associated with the notification. Default is <see cref="ApplePriority.Active"/>.
    /// </summary>
    public ApplePriority Priority { get; set; } = ApplePriority.Active;

    /// <summary>
    /// Gets or sets the relevance score (between 0 and 1) used by the system to sort notifications. The highest score gets featured in the notification summary.
    /// </summary>
    [JsonNumberHandling(JsonNumberHandling.AllowNamedFloatingPointLiterals)]
    public double RelevanceScore { get; set; }

    /// <summary>
    /// Gets or sets the string the notification adds to the category’s summary format string.
    /// </summary>
    public string SummaryArgument { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of items the notification adds to the category’s summary format string.
    /// </summary>
    public int SummaryArgumentCount { get; set; }

    /// <summary>
    /// When set, plays a critical alert sound at the specified volume even when the device is muted or Do Not Disturb is active.
    /// The value must be between 0.0 (silent) and 1.0 (full volume) inclusive.
    /// Requires the <c>NSCriticalAlertUsageDescription</c> key in Info.plist and
    /// <c>UNAuthorizationOptionCriticalAlert</c> to have been granted.
    /// Only effective on iOS 12+ and macOS 10.14+. Has no effect on older versions or on other platforms.
    /// </summary>
    [JsonNumberHandling(JsonNumberHandling.AllowNamedFloatingPointLiterals)]
    public float? CriticalSoundVolume { get; set; }

    /// <summary>
    /// When <c>true</c>, prevents the system from displaying a thumbnail preview for the notification's image attachment.
    /// Has no effect when no image is set on the notification request.
    /// Maps to <c>UNNotificationAttachmentOptionsThumbnailHiddenKey</c>.
    /// </summary>
    public bool? HideThumbnail { get; set; }

    /// <summary>
    /// Defines the normalized clipping rect used when generating the thumbnail for the notification's image attachment.
    /// All coordinates must be in the range 0.0 to 1.0 relative to the image dimensions.
    /// Has no effect when no image is set on the notification request.
    /// Maps to <c>UNNotificationAttachmentOptionsThumbnailClippingRectKey</c>.
    /// </summary>
    public AppleAttachmentThumbnailClippingRect? ThumbnailClippingRect { get; set; }}