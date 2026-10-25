using CoreLocation;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.Core.Models.AppleOption;
#if IOS
using Plugin.LocalNotification.Core.Platforms.iOS;
#else
using Plugin.LocalNotification.Core.Platforms.MacCatalyst;
#endif
using System.Globalization;
using UserNotifications;

namespace Plugin.LocalNotification.Platforms;

#if IOS
internal class GeofenceHandler : IIOSGeofenceHandler
#else
internal class GeofenceHandler : IMacCatalystGeofenceHandler
#endif
{
    public UNNotificationTrigger? GetGeofenceTrigger(NotificationRequest request)
    {
#if IOS
        var notificationId = request.NotificationId.ToString(CultureInfo.CurrentCulture);

        var center = new CLLocationCoordinate2D(request.Geofence.Center.Latitude, request.Geofence.Center.Longitude);
        var region = new CLCircularRegion(center, request.Geofence.RadiusInMeters, notificationId)
        {
            NotifyOnEntry = (request.Geofence.NotifyOn & NotificationRequestGeofence.GeofenceNotifyOn.OnEntry) == NotificationRequestGeofence.GeofenceNotifyOn.OnEntry,
            NotifyOnExit = (request.Geofence.NotifyOn & NotificationRequestGeofence.GeofenceNotifyOn.OnExit) == NotificationRequestGeofence.GeofenceNotifyOn.OnExit,
        };

        var trigger = UNLocationNotificationTrigger.CreateTrigger(region, request.Geofence.Apple.Repeats);
        return trigger;
#else
        // MacCatalyst geofence support is not yet implemented.
        return null;
#endif
    }

    public bool RequestLocationNotificationPermission(NotificationPermission permission)
    {        
        if (!permission.AskPermission)
        {
            return false;
        }

        if (permission.Apple.LocationAuthorization == AppleLocationAuthorization.No)
        {
            return false;
        }

        var locationManager = new CLLocationManager();

        if (permission.Apple.LocationAuthorization == AppleLocationAuthorization.Always)
        {
            locationManager.RequestAlwaysAuthorization();
        }
        else if (permission.Apple.LocationAuthorization == AppleLocationAuthorization.WhenInUse)
        {
            locationManager.RequestWhenInUseAuthorization();
        }

        return true;
    }
}
