namespace Plugin.LocalNotification.Core.Models.AndroidOption;

/// <summary>
/// Defines a text-input field attached to an Android notification action button,
/// allowing users to type a reply directly from the notification without opening the app.
/// Equivalent to <c>RemoteInput</c> on Android.
/// </summary>
public class AndroidActionInput
{
    /// <summary>
    /// The hint text displayed inside the input field.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Optional pre-defined choices shown as chips above the keyboard.
    /// When provided, users can tap a choice instead of typing.
    /// </summary>
    public string[]? Choices { get; set; }

    /// <summary>
    /// Whether the user is allowed to type free-form text in addition to (or instead of) the
    /// pre-defined <see cref="Choices"/>. Default is <c>true</c>.
    /// Set to <c>false</c> to restrict the input to the <see cref="Choices"/> list only.
    /// </summary>
    public bool AllowFreeFormInput { get; set; } = true;
}
