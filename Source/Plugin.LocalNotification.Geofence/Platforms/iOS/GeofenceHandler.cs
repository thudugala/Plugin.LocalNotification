using CoreLocation;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.Core.Models.iOSOption;
using Plugin.LocalNotification.Core.Platforms.iOS;
using System.Globalization;
using UserNotifications;

namespace Plugin.LocalNotification.Platforms;

internal class GeofenceHandler : IiOSGeofenceHandler
{
    public UNNotificationTrigger? GetGeofenceTrigger(NotificationRequest request)
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

    public bool RequestLocationNotificationPermission(NotificationPermission permission)
    {        
        if (!permission.AskPermission)
        {
            return false;
        }

        if (permission.IOS.LocationAuthorization == iOSLocationAuthorization.No)
        {
            return false;
        }

        var locationManager = new CLLocationManager();

        if (permission.IOS.LocationAuthorization == iOSLocationAuthorization.Always)
        {
            locationManager.RequestAlwaysAuthorization();
        }
        else if (permission.IOS.LocationAuthorization == iOSLocationAuthorization.WhenInUse)
        {
            locationManager.RequestWhenInUseAuthorization();
        }

        return true;
    }
}
