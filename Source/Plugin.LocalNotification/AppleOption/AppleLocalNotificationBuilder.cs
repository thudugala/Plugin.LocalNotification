#if IOS || MACCATALYST
using Plugin.LocalNotification.Platforms;
#endif

namespace Plugin.LocalNotification.AppleOption;

/// <summary>
/// Defines methods for building IOS-specific local notification options, including custom notification center delegate configuration.
/// </summary>
public interface IAppleLocalNotificationBuilder
{
#if IOS || MACCATALYST
    /// <summary>
    /// Sets a custom <see cref="UserNotificationCenterDelegate"/> for handling IOS notification events.
    /// Allows developers to extend or replace the default notification center delegate.
    /// </summary>
    /// <param name="customUserNotificationCenterDelegate">The custom notification center delegate to use.</param>
    /// <returns>The builder instance for chaining.</returns>
    IAppleLocalNotificationBuilder SetCustomUserNotificationCenterDelegate(UserNotificationCenterDelegate customUserNotificationCenterDelegate);
#endif
}

/// <summary>
/// Provides a builder for configuring IOS-specific local notification options, including custom notification center delegate.
/// </summary>
/// <inheritdoc/>
public class AppleLocalNotificationBuilder : IAppleLocalNotificationBuilder
{
#if IOS || MACCATALYST
    /// <summary>
    /// Gets the custom <see cref="UserNotificationCenterDelegate"/> used for handling IOS notification events.
    /// </summary>
    internal UserNotificationCenterDelegate? CustomUserNotificationCenterDelegate { get; private set; }

    /// <inheritdoc/>
    public IAppleLocalNotificationBuilder SetCustomUserNotificationCenterDelegate(UserNotificationCenterDelegate customUserNotificationCenterDelegate)
    {
        CustomUserNotificationCenterDelegate = customUserNotificationCenterDelegate;
        return this;
    }
#endif
}