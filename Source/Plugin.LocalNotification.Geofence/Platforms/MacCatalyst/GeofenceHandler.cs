using Plugin.LocalNotification.Core.Models;
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
}
