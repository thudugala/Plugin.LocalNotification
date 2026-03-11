using CoreLocation;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.Core.Models.iOSOption;
using Plugin.LocalNotification.Core.Platforms.MacCatalyst;
using UserNotifications;

namespace Plugin.LocalNotification.Platforms;

internal class GeofenceHandler : IMacCatalystGeofenceHandler
{
    public UNNotificationTrigger? GetGeofenceTrigger(NotificationRequest request)
    {
        // MacCatalyst geofence support is not yet implemented.
        return null;
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
