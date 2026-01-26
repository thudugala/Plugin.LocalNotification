namespace Plugin.LocalNotification;

/// <summary>
/// Provides extension methods for integrating local notification functionality into a .NET MAUI application.
/// </summary>
public static class LocalNotificationGeofenceExtensions
{
    /// <summary>
    /// Configures the application to use geofence-enabled local notifications on supported platforms.
    /// </summary>
    /// <remarks>On Android and iOS, this method sets up a notification service that supports geofence-triggered local
    /// notifications. On other platforms, this method has no effect.</remarks>
    /// <param name="builder">The <see cref="MauiAppBuilder"/> instance to configure for geofence local notifications.</param>
    /// <returns>The same <see cref="MauiAppBuilder"/> instance, enabling method chaining.</returns>
    public static MauiAppBuilder UseLocalNotificationGeofence(this MauiAppBuilder builder)
    {
#if ANDROID
        LocalNotificationCenter.SetNotificationService(new Plugin.LocalNotification.Platforms.NotificationServiceImplGeofence());
#elif IOS
        LocalNotificationCenter.SetNotificationService(new Plugin.LocalNotification.Platforms.NotificationServiceImplGeofence());
#endif
        return builder;
    }
}
