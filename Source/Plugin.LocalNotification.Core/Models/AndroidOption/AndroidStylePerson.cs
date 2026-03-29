namespace Plugin.LocalNotification.Core.Models.AndroidOption;

/// <summary>
/// Represents a person who can be the sender of a <see cref="AndroidStyleMessage"/>
/// or the user in an <see cref="AndroidMessagingStyle"/> notification.
/// </summary>
public class AndroidStylePerson
{
    /// <summary>
    /// The display name of the person. Required when used as the messaging-style user.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional unique key that identifies this person across app sessions.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Whether this person represents an automated agent (bot).
    /// </summary>
    public bool IsBot { get; set; }

    /// <summary>
    /// Whether this person is considered important (used for DND bypass decisions).
    /// </summary>
    public bool IsImportant { get; set; }
}
