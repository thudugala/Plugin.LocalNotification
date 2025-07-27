namespace Plugin.LocalNotification;

/// <summary>
/// Represents a notification category, which serves as a container for notification actions.
/// </summary>
/// <remarks>
/// <see cref="CategoryType"/> is the unique identifier for the category.
/// </remarks>
/// <param name="categoryType">A unique identifier for the category.</param>
public class NotificationCategory(NotificationCategoryType categoryType) : IEquatable<NotificationCategory>
{
    /// <summary>
    /// Gets or sets the list of actions associated with this category.
    /// </summary>
    public HashSet<NotificationAction> ActionList { get; set; } = [];

    /// <summary>
    /// Gets the unique identifier for the category.
    /// </summary>
    public NotificationCategoryType CategoryType { get; } = categoryType;

    /// <summary>
    /// Determines whether the specified <see cref="NotificationCategory"/> is equal to the current category.
    /// </summary>
    /// <param name="other">The other <see cref="NotificationCategory"/> to compare.</param>
    /// <returns><c>true</c> if the categories have the same <see cref="CategoryType"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(NotificationCategory? other) => other != null &&
               CategoryType == other.CategoryType;

    /// <summary>
    /// Determines whether the specified object is equal to the current category.
    /// </summary>
    /// <param name="obj">The object to compare with the current category.</param>
    /// <returns><c>true</c> if the object is a <see cref="NotificationCategory"/> with the same <see cref="CategoryType"/>; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj) => Equals(obj as NotificationCategory);

    /// <summary>
    /// Returns a hash code for the current category.
    /// </summary>
    /// <returns>A hash code for the current category.</returns>
    public override int GetHashCode() => CategoryType.GetHashCode();
}