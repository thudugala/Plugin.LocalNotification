using Plugin.LocalNotification.Core;

namespace Plugin.LocalNotification.Geofence;

/// <summary>
/// Extension methods for registering geofence notification support.
/// </summary>
public static class GeofenceExtensions
{
    /// <summary>
    /// Registers geofence notification support for Plugin.LocalNotification.
    /// Call this after <c>UseLocalNotification()</c> in your <see cref="MauiAppBuilder"/> configuration.
    /// </summary>
    /// <param name="builder">The <see cref="MauiAppBuilder"/> to configure.</param>
    /// <returns>The configured <see cref="MauiAppBuilder"/>.</returns>
    public static MauiAppBuilder UseLocalNotificationGeofence(this MauiAppBuilder builder)
    {
#if ANDROID || IOS || MACCATALYST
        GeofenceHandlerRegistry.Handler = new Platforms.GeofenceHandler();
#endif
        return builder;
    }
}
