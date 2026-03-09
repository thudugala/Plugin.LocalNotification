#if IOS
using Plugin.LocalNotification.Platforms;
#endif

namespace Plugin.LocalNotification.iOSOption;

/// <summary>
/// Defines methods for building iOS-specific local notification options, including custom notification center delegate configuration.
/// </summary>
public interface IiOSLocalNotificationBuilder
{
#if IOS
    /// <summary>
    /// Sets a custom <see cref="UserNotificationCenterDelegate"/> for handling iOS notification events.
    /// Allows developers to extend or replace the default notification center delegate.
    /// </summary>
    /// <param name="customUserNotificationCenterDelegate">The custom notification center delegate to use.</param>
    /// <returns>The builder instance for chaining.</returns>
    IiOSLocalNotificationBuilder SetCustomUserNotificationCenterDelegate(UserNotificationCenterDelegate customUserNotificationCenterDelegate);
#endif
}

/// <summary>
/// Provides a builder for configuring iOS-specific local notification options, including custom notification center delegate.
/// </summary>
/// <inheritdoc/>
public class iOSLocalNotificationBuilder : IiOSLocalNotificationBuilder
{
#if IOS
    /// <summary>
    /// Gets the custom <see cref="UserNotificationCenterDelegate"/> used for handling iOS notification events.
    /// </summary>
    internal UserNotificationCenterDelegate? CustomUserNotificationCenterDelegate { get; private set; }

    /// <inheritdoc/>
    public IiOSLocalNotificationBuilder SetCustomUserNotificationCenterDelegate(UserNotificationCenterDelegate customUserNotificationCenterDelegate)
    {
        CustomUserNotificationCenterDelegate = customUserNotificationCenterDelegate;
        return this;
    }
#endif
}