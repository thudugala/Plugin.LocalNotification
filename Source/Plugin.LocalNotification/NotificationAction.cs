using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;
using Plugin.LocalNotification.WindowsOption;

namespace Plugin.LocalNotification;

/// <summary>
/// Represents an action that can be performed from a local notification, such as tapping a button.
/// </summary>
/// <remarks>
/// <see cref="ActionId"/> is the unique identifier for the action within a notification category.
/// </remarks>
/// <param name="actionId">A unique identifier for the action.</param>
public class NotificationAction(int actionId) : IEquatable<NotificationAction>
{
    /// <summary>
    /// Gets the unique identifier for the action.
    /// </summary>
    public int ActionId { get; } = actionId;

    /// <summary>
    /// Gets or sets iOS-specific properties for the action.
    /// </summary>
    public iOSAction IOS { get; set; } = new();

    /// <summary>
    /// Gets or sets Android-specific properties for the action.
    /// </summary>
    public AndroidAction Android { get; set; } = new();

    /// <summary>
    /// Gets or sets Windows-specific properties for the action.
    /// </summary>
    public WindowsAction Windows { get; set; } = new();

    /// <summary>
    /// Gets or sets the display title for the action.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Determines whether the specified <see cref="NotificationAction"/> is equal to the current action.
    /// </summary>
    /// <param name="other">The other <see cref="NotificationAction"/> to compare.</param>
    /// <returns><c>true</c> if the actions have the same <see cref="ActionId"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(NotificationAction? other) => other != null &&
               ActionId == other.ActionId;

    /// <summary>
    /// Determines whether the specified object is equal to the current action.
    /// </summary>
    /// <param name="obj">The object to compare with the current action.</param>
    /// <returns><c>true</c> if the object is a <see cref="NotificationAction"/> with the same <see cref="ActionId"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) => Equals(obj as NotificationAction);

    /// <summary>
    /// Returns a hash code for the current action.
    /// </summary>
    /// <returns>A hash code for the current action.</returns>
    public override int GetHashCode() => ActionId.GetHashCode();
}