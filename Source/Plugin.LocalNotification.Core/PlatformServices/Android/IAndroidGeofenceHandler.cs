using Android.App;
using Plugin.LocalNotification.Core.Models;

namespace Plugin.LocalNotification.Core.Platforms.Android;

/// <summary>
/// Interface for Android geofence handling, implemented by Plugin.LocalNotification.Geofence.
/// </summary>
public interface IAndroidGeofenceHandler : IGeofenceHandler
{
    /// <summary>
    /// Registers a geofence and schedules the notification for the given request.
    /// </summary>
    /// <param name="request">The notification request with geofence data.</param>
    /// <param name="serializedRequest">The serialized notification request for the PendingIntent.</param>
    /// <returns><c>true</c> if the geofence was successfully registered.</returns>
    Task<bool> ShowGeofence(NotificationRequest request, string serializedRequest);

    /// <summary>
    /// Creates a PendingIntent for geofence transitions.
    /// </summary>
    /// <param name="notificationId">The notification identifier.</param>
    /// <param name="serializedRequest">The serialized notification request, or null for cancellation.</param>
    /// <returns>A PendingIntent for the geofence receiver, or null.</returns>
    PendingIntent? CreateGeofenceIntent(int notificationId, string? serializedRequest);

    /// <summary>
    /// Removes registered geofences associated with the given PendingIntent.
    /// </summary>
    /// <param name="pendingIntent">The PendingIntent to remove geofences for.</param>
    void RemoveGeofences(PendingIntent? pendingIntent);
}
