namespace Plugin.LocalNotification.Core.Models.AndroidOption;

/// <summary>
/// Applies <c>NotificationCompat.MediaStyle</c> to the notification, which is suited for
/// media playback controls. The notification's actions become media transport controls.
/// </summary>
public class AndroidMediaStyle : AndroidStyleBase
{
    /// <summary>
    /// Indices (0-based) of the actions added via <c>NotificationCategory</c> that should
    /// appear in the compact notification view. Android allows up to 3 indices.
    /// </summary>
    public int[]? ShowActionsInCompactView { get; set; }
}
