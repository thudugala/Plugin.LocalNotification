using Android.App;
using Android.Gms.Location;
using Plugin.LocalNotification.AndroidOption;
using ApplicationAlias = Android.App.Application;

namespace Plugin.LocalNotification.Platforms;

internal class NotificationServiceImplGeofence : NotificationServiceImpl
{
    protected IGeofencingClient? MyGeofencingClient;

    protected override void OnConstructed()
    {
        try
        {
            MyGeofencingClient = LocationServices.GetGeofencingClient(ApplicationAlias.Context);
        }
        catch (Exception ex)
        {
            LocalNotificationCenter.Log(ex);
        }
    }

    protected override void RemoveGeofences(PendingIntent? pendingIntent)
    {
        if (pendingIntent is not null)
        {
            MyGeofencingClient?.RemoveGeofences(pendingIntent);
        }
    }

    internal override async Task<bool> ShowGeofence(NotificationRequest request)
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

        var serializedRequest = LocalNotificationCenter.GetRequestSerialize(request);
        var pendingIntent = CreateGeofenceIntent(request.NotificationId, serializedRequest);

        if (MyGeofencingClient is not null && pendingIntent is not null)
        {
            await MyGeofencingClient
                .AddGeofencesAsync(
                    geoRequest,
                    pendingIntent
                )
                .ConfigureAwait(false);
        }

        return true;
    }

    protected override PendingIntent? CreateGeofenceIntent(int notificationId, string? serializedRequest)
    {
        var pendingIntent = base.CreateActionIntent(notificationId, serializedRequest, new NotificationAction(0)
        {
            Android =
            {
                LaunchAppWhenTapped = false,
                PendingIntentFlags = AndroidPendingIntentFlags.UpdateCurrent
            }
        }, typeof(GeofenceTransitionsIntentReceiver));
        return pendingIntent;
    }
}
