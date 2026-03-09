using Android.App;
using Android.Gms.Location;
using Plugin.LocalNotification.Core;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.Core.Models.AndroidOption;
using Plugin.LocalNotification.Core.Platforms.Android;
using ApplicationAlias = Android.App.Application;

namespace Plugin.LocalNotification.Platforms;

internal class GeofenceHandler : IAndroidGeofenceHandler
{
    private readonly IGeofencingClient? _geofencingClient;

    public GeofenceHandler()
    {
        try
        {
            _geofencingClient = LocationServices.GetGeofencingClient(ApplicationAlias.Context);
        }
        catch (Exception ex)
        {
            LocalNotificationLogger.Log(ex);
        }
    }

    public async Task<bool> ShowGeofence(NotificationRequest request, string serializedRequest)
    {
        var geofenceBuilder = new GeofenceBuilder()
        .SetRequestId(request.NotificationId.ToString())
        .SetExpirationDuration(request.Geofence.Android.ExpirationDurationInMilliseconds)
        .SetNotificationResponsiveness(request.Geofence.Android.ResponsivenessMilliseconds)
        .SetCircularRegion(
            request.Geofence.Center.Latitude,
            request.Geofence.Center.Longitude,
            Convert.ToSingle(request.Geofence.RadiusInMeters)
        );

        var transitionType = 0;
        if ((request.Geofence.NotifyOn & NotificationRequestGeofence.GeofenceNotifyOn.OnEntry) == NotificationRequestGeofence.GeofenceNotifyOn.OnEntry)
        {
            transitionType |= Android.Gms.Location.Geofence.GeofenceTransitionEnter;
        }
        if ((request.Geofence.NotifyOn & NotificationRequestGeofence.GeofenceNotifyOn.OnExit) == NotificationRequestGeofence.GeofenceNotifyOn.OnExit)
        {
            transitionType |= Android.Gms.Location.Geofence.GeofenceTransitionExit;
        }

        if (request.Geofence.Android.LoiteringDelayMilliseconds > 0)
        {
            transitionType = Android.Gms.Location.Geofence.GeofenceTransitionDwell;
            geofenceBuilder.SetLoiteringDelay(request.Geofence.Android.LoiteringDelayMilliseconds);
        }
        geofenceBuilder.SetTransitionTypes(transitionType);

        var geofence = geofenceBuilder.Build();

        var geoRequest = new GeofencingRequest.Builder()
            .SetInitialTrigger(0)
            .AddGeofence(geofence)
            .Build();

        var pendingIntent = CreateGeofenceIntent(request.NotificationId, serializedRequest);

        if (_geofencingClient is not null && pendingIntent is not null)
        {
            await _geofencingClient
                .AddGeofencesAsync(
                    geoRequest,
                    pendingIntent
                )
                .ConfigureAwait(false);
        }

        return true;
    }

    public PendingIntent? CreateGeofenceIntent(int notificationId, string? serializedRequest)
    {
        var pendingIntent = AndroidUtils.CreateActionIntent(notificationId, serializedRequest, new NotificationAction(0)
        {
            Android =
            {
                LaunchAppWhenTapped = false,
                PendingIntentFlags = AndroidPendingIntentFlags.UpdateCurrent
            }
        }, typeof(GeofenceTransitionsIntentReceiver));
        return pendingIntent;
    }

    public void RemoveGeofences(PendingIntent? pendingIntent)
    {
        if (pendingIntent is not null)
        {
            _geofencingClient?.RemoveGeofences(pendingIntent);
        }
    }
}
