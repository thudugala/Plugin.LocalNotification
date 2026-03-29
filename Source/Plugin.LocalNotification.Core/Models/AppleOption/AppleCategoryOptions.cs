namespace Plugin.LocalNotification.Core.Models.AppleOption;

/// <summary>
/// Controls the behaviour of an iOS/macOS notification category, corresponding to
/// <c>UNNotificationCategoryOptions</c>.
/// </summary>
[Flags]
public enum AppleCategoryOptions
{
    /// <summary>No special options.</summary>
    None = 0,

    /// <summary>
    /// Send a dismiss action when the user explicitly dismisses the notification.
    /// Corresponds to <c>UNNotificationCategoryOptions.CustomDismissAction</c>.
    /// </summary>
    CustomDismissAction = 1 << 0,

    /// <summary>
    /// Allow this category to be used in CarPlay environments.
    /// Corresponds to <c>UNNotificationCategoryOptions.AllowInCarPlay</c>.
    /// </summary>
    AllowInCarPlay = 1 << 1,

    /// <summary>
    /// Show the notification title even when notification previews are hidden.
    /// Requires iOS 11+. Corresponds to <c>UNNotificationCategoryOptions.HiddenPreviewShowTitle</c>.
    /// </summary>
    HiddenPreviewShowTitle = 1 << 2,

    /// <summary>
    /// Show the notification subtitle even when notification previews are hidden.
    /// Requires iOS 11+. Corresponds to <c>UNNotificationCategoryOptions.HiddenPreviewShowSubtitle</c>.
    /// </summary>
    HiddenPreviewShowSubtitle = 1 << 3,

    /// <summary>
    /// Allow Siri to automatically read out the notification when the user is wearing AirPods.
    /// Requires iOS 13+. Corresponds to <c>UNNotificationCategoryOptions.AllowAnnouncement</c>.
    /// </summary>
    AllowAnnouncement = 1 << 4,
}
