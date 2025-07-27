using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;
using Plugin.LocalNotification.Json;

namespace Plugin.LocalNotification;

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
    /// Gets the iOS-specific notification builder.
    /// </summary>
    internal iOSLocalNotificationBuilder IOSBuilder { get; } = new();

    /// <inheritdoc/>
    public ILocalNotificationBuilder AddAndroid(Action<IAndroidLocalNotificationBuilder> android)
    {
        android?.Invoke(AndroidBuilder);
        return this;
    }

    /// <inheritdoc/>
    public ILocalNotificationBuilder AddiOS(Action<IiOSLocalNotificationBuilder> iOS)
    {
        iOS?.Invoke(IOSBuilder);
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