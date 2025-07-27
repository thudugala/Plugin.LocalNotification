using System.Text.Json.Serialization;

namespace Plugin.LocalNotification.iOSOption;

/// <summary>
/// Represents notification request options for iOS, including display, sound, badge, priority, and summary settings.
/// </summary>
public class iOSOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether to prevent iOS from displaying the default banner when a notification is received in the foreground. Default is <c>false</c>.
    /// </summary>
    public bool HideForegroundAlert { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to enable iOS to play the default notification sound even if the app is in the foreground. Default is <c>true</c>.
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
    /// Gets or sets the priority associated with the notification. Default is <see cref="iOSPriority.Active"/>.
    /// </summary>
    public iOSPriority Priority { get; set; } = iOSPriority.Active;

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
}