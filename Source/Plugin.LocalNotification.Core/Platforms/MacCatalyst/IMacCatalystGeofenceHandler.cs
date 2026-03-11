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

    /// <summary>
    /// Requests the specified level of permission from the user to allow the application to receive location-based
    /// notifications.
    /// </summary>
    /// <remarks>This method may display a system dialog prompting the user to grant or deny the requested
    /// permission. The result reflects the user's response.</remarks>
    /// <param name="permission">The notification permission level to request. Determines the type of location notifications the application is
    /// authorized to receive.</param>
    /// <returns>true if the user grants the requested permission; otherwise, false.</returns>
    bool RequestLocationNotificationPermission(NotificationPermission permission);
}
