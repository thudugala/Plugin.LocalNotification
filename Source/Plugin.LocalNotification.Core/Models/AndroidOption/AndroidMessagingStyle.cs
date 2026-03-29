namespace Plugin.LocalNotification.Core.Models.AndroidOption;

/// <summary>
/// Shows a conversation thread as a series of messages in the expanded notification view.
/// Equivalent to <c>NotificationCompat.MessagingStyle</c>.
/// </summary>
public class AndroidMessagingStyle : AndroidStyleBase
{
    /// <summary>
    /// The person who represents the notification's recipient (i.e., the local user).
    /// The <see cref="AndroidStylePerson.Name"/> is required.
    /// </summary>
    public AndroidStylePerson Person { get; set; } = new();

    /// <summary>
    /// The conversation title shown above the message thread.
    /// Maps to <see cref="AndroidStyleBase.ContentTitle"/> when constructing the native style;
    /// you can also set <see cref="AndroidStyleBase.ContentTitle"/> directly.
    /// </summary>
    public string? ConversationTitle
    {
        get => ContentTitle;
        set => ContentTitle = value;
    }

    /// <summary>
    /// Whether this conversation involves more than two people (a group chat).
    /// Affects how the sender name is displayed per message.
    /// </summary>
    public bool IsGroupConversation { get; set; }

    /// <summary>
    /// The list of messages to display, in chronological order.
    /// </summary>
    public IList<AndroidStyleMessage> Messages { get; set; } = [];
}
