namespace Plugin.LocalNotification.Core.Models.AndroidOption;

/// <summary>
/// Base class for Android notification styles.
/// Set <see cref="AndroidOptions.Style"/> to an instance of a derived class to override the default BigText/BigPicture behaviour.
/// </summary>
public abstract class AndroidStyleBase
{
    /// <summary>
    /// Overrides the notification's content title for the expanded view.
    /// If null, the notification's <c>Title</c> is used.
    /// </summary>
    public string? ContentTitle { get; set; }

    /// <summary>
    /// Sets the summary text shown in the collapsed/footer area of the expanded notification.
    /// </summary>
    public string? SummaryText { get; set; }
}
