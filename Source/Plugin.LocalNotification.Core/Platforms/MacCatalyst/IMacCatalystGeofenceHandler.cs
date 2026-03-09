using Plugin.LocalNotification.Core.Models;
using UserNotifications;

namespace Plugin.LocalNotification.Core.Platforms.MacCatalyst;

/// <summary>
/// Interface for iOS/MacCatalyst geofence handling, implemented by Plugin.LocalNotification.Geofence.
/// </summary>
public interface IMacCatalystGeofenceHandler : IGeofenceHandler
{
    /// <summary>
    /// Creates a location-based notification trigger for the given geofence request.
    /// </summary>
    /// <param name="request">The notification request with geofence data.</param>
    /// <returns>A <see cref="UNNotificationTrigger"/> for the geofence, or null if it cannot be created.</returns>
    UNNotificationTrigger? GetGeofenceTrigger(NotificationRequest request);
}
