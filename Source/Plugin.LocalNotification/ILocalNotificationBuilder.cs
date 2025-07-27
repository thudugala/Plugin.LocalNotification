using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;
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
    /// Configures iOS-specific notification options using the provided delegate.
    /// </summary>
    /// <param name="iOS">A delegate to configure iOS notification options.</param>
    /// <returns>The builder instance for chaining.</returns>
    ILocalNotificationBuilder AddiOS(Action<IiOSLocalNotificationBuilder> iOS);
}