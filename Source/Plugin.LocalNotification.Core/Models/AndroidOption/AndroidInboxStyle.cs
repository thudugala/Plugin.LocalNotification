namespace Plugin.LocalNotification.Core.Models.AndroidOption;

/// <summary>
/// Shows multiple short lines of text in the expanded notification view (Android Inbox style).
/// Equivalent to <c>NotificationCompat.InboxStyle</c>.
/// </summary>
public class AndroidInboxStyle : AndroidStyleBase
{
    /// <summary>
    /// The lines of text to display. Each line is shown as a separate row.
    /// Up to 5-7 lines are visible depending on available height.
    /// </summary>
    public IList<string> Lines { get; set; } = [];
}
