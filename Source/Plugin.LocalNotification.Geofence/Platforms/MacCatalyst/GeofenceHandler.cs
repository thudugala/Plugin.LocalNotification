using CoreLocation;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.Core.Models.AppleOption;
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
