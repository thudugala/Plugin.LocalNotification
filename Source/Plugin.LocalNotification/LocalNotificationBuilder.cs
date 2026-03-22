using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.AppleOption;
using Plugin.LocalNotification.Json;

namespace Plugin.LocalNotification;

/// <summary>
/// Defines methods for building and configuring local notification options, including categories, serializers, and platform-specific settings.
/// </summary>
public interface ILocalNotificationBuilder
{
    /// <summary>
    /// Registers a notification category and its corresponding actions.
    /// </summary>
    /// <param name="category">The notification category to register.</param>
    /// <returns>The builder instance for chaining.</returns>
    ILocalNotificationBuilder AddCategory(NotificationCategory category);

    /// <summary>
    /// Sets the serializer to use for notification requests.
    /// </summary>
    /// <param name="serializer">The notification serializer to use.</param>
    /// <returns>The builder instance for chaining.</returns>
    ILocalNotificationBuilder SetSerializer(INotificationSerializer serializer);

    /// <summary>
    /// Configures Android-specific notification options using the provided delegate.
    /// </summary>
    /// <param name="android">A delegate to configure Android notification options.</param>
    /// <returns>The builder instance for chaining.</returns>
    ILocalNotificationBuilder AddAndroid(Action<IAndroidLocalNotificationBuilder> android);

    /// <summary>
    /// Configures IOS-specific notification options using the provided delegate.
    /// </summary>
    /// <param name="iOS">A delegate to configure IOS notification options.</param>
    /// <returns>The builder instance for chaining.</returns>
    ILocalNotificationBuilder AddiOS(Action<IAppleLocalNotificationBuilder> iOS);
}


/// <summary>
/// Provides a builder for configuring local notifications, including platform-specific options and notification categories.
/// </summary>
public class LocalNotificationBuilder : ILocalNotificationBuilder
{
    /// <summary>
    /// Gets the set of registered notification categories and their corresponding actions.
    /// </summary>
    internal HashSet<NotificationCategory> CategorySet { get; } = [];

    /// <summary>
    /// Gets or sets the serializer used for notification requests.
    /// </summary>
    internal INotificationSerializer Serializer { get; private set; } = new NotificationSerializer();

    /// <summary>
    /// Gets the Android-specific notification builder.
    /// </summary>
    internal AndroidLocalNotificationBuilder AndroidBuilder { get; } = new();

    /// <summary>
    /// Gets the IOS-specific notification builder.
    /// </summary>
    internal AppleLocalNotificationBuilder AppleBuilder { get; } = new();

    /// <inheritdoc/>
    public ILocalNotificationBuilder AddAndroid(Action<IAndroidLocalNotificationBuilder> android)
    {
        android?.Invoke(AndroidBuilder);
        return this;
    }

    /// <inheritdoc/>
    public ILocalNotificationBuilder AddiOS(Action<IAppleLocalNotificationBuilder> iOS)
    {
        iOS?.Invoke(AppleBuilder);
        return this;
    }

    /// <inheritdoc/>
    public ILocalNotificationBuilder AddCategory(NotificationCategory category)
    {
        CategorySet.Add(category);
        return this;
    }

    /// <inheritdoc/>
    public ILocalNotificationBuilder SetSerializer(INotificationSerializer serializer)
    {
        Serializer = serializer ?? new NotificationSerializer();
        return this;
    }
}