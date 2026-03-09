#if ANDROID
using Plugin.LocalNotification.Core.Platforms.Android;
#elif IOS
using Plugin.LocalNotification.Core.Platforms.iOS;
#elif MACCATALYST
using Plugin.LocalNotification.Core.Platforms.MacCatalyst;
#endif

namespace Plugin.LocalNotification.Core;

/// <summary>
/// Static registry for geofence handler implementations.
/// The handler is registered by Plugin.LocalNotification.Geofence and consumed by Plugin.LocalNotification.
/// </summary>
public static class GeofenceHandlerRegistry
{
    /// <summary>
    /// The registered platform-specific geofence handler.
    /// </summary>    
#if ANDROID
    public static IAndroidGeofenceHandler? Handler { get; set; }
#elif IOS
    public static IiOSGeofenceHandler? Handler { get; set; }
#elif MACCATALYST
    public static IMacCatalystGeofenceHandler? Handler { get; set; }
#else
    public static IGeofenceHandler? Handler { get; set; }
#endif

    /// <summary>
    /// Callback to show a notification from a serialized request string.
    /// Set by Plugin.LocalNotification at startup, invoked by Plugin.LocalNotification.Geofence
    /// when a geofence transition fires (e.g., from a BroadcastReceiver on Android).
    /// </summary>
    public static Func<string, Task<bool>>? ShowNotificationFromSerializedRequest { get; set; }
}
