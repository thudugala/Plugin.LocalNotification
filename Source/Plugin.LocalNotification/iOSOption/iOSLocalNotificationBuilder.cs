#if IOS
using Plugin.LocalNotification.Platforms;
#endif

namespace Plugin.LocalNotification.iOSOption;

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