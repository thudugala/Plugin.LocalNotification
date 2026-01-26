using CoreLocation;
using System.Globalization;
using UserNotifications;

namespace Plugin.LocalNotification.Platforms;

internal class NotificationServiceImplGeofence : NotificationServiceImpl
{
    protected override UNNotificationTrigger? GetGeofenceTrigger(NotificationRequest request)
    {
        var notificationId = request.NotificationId.ToString(CultureInfo.CurrentCulture);

        var center = new CLLocationCoordinate2D(request.Geofence.Center.Latitude, request.Geofence.Center.Longitude);
        var region = new CLCircularRegion(center, request.Geofence.RadiusInMeters, notificationId)
        {
            NotifyOnEntry = (request.Geofence.NotifyOn & NotificationRequestGeofence.GeofenceNotifyOn.OnEntry) == NotificationRequestGeofence.GeofenceNotifyOn.OnEntry,
            NotifyOnExit = (request.Geofence.NotifyOn & NotificationRequestGeofence.GeofenceNotifyOn.OnExit) == NotificationRequestGeofence.GeofenceNotifyOn.OnExit,
        };

        var trigger = UNLocationNotificationTrigger.CreateTrigger(region, request.Geofence.IOS.Repeats);
        return trigger;
    }
}
