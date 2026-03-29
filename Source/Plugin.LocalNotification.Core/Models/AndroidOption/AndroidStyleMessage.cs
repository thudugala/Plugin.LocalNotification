namespace Plugin.LocalNotification.Core.Models.AndroidOption;

/// <summary>
/// Represents a single message in an <see cref="AndroidMessagingStyle"/> notification.
/// </summary>
public class AndroidStyleMessage
{
    /// <summary>
    /// The text content of the message.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// The time at which the message was sent. Defaults to the time the object is created.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;

    /// <summary>
    /// The person who sent this message.
    /// Set to <c>null</c> (or leave unset) to indicate the message was sent by the notification's own user
    /// (i.e., the person passed to <see cref="AndroidMessagingStyle.Person"/>).
    /// </summary>
    public AndroidStylePerson? Person { get; set; }
}
