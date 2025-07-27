using Android.Content;

namespace Plugin.LocalNotification.Platforms;

/// <summary>
/// Broadcast receiver for handling geofence transition intents in Android and triggering local notifications.
/// </summary>
[BroadcastReceiver(
    Name = ReceiverName,
    Enabled = true,
    Exported = false,
    Label = "Plugin LocalNotification Geofence Transitions Receiver"
)]
public class GeofenceTransitionsIntentReceiver : BroadcastReceiver
{
    /// <summary>
    /// The name of the broadcast receiver.
    /// </summary>
    public const string ReceiverName = "plugin.LocalNotification." + nameof(GeofenceTransitionsIntentReceiver);

    /// <summary>
    /// Called when a geofence transition intent is received. Triggers a local notification if the request contains geofence information.
    /// </summary>
    /// <param name="context">The context in which the receiver is running.</param>
    /// <param name="intent">The intent being received.</param>
    public override async void OnReceive(Context? context, Intent? intent)
    {
        try
        {
            var notificationService = TryGetDefaultDroidNotificationService();

            var requestSerialize = intent?.GetStringExtra(LocalNotificationCenter.ReturnRequest);
            if (string.IsNullOrWhiteSpace(requestSerialize))
            {
                LocalNotificationCenter.Log("Request Json Not Found");
                return;
            }
            var request = LocalNotificationCenter.GetRequest(requestSerialize);

            if (!request.Geofence.IsGeofence)
            {
                LocalNotificationCenter.Log($"Notification {request.NotificationId} has no Geofence isformation");
                return;
            }
            _ = await notificationService.ShowNow(request);
        }
        catch (Exception ex)
        {
            LocalNotificationCenter.Log(ex);
        }
    }

    /// <summary>
    /// Attempts to get the default Android notification service implementation.
    /// </summary>
    /// <returns>A new or existing instance of <see cref="NotificationServiceImpl"/>.</returns>
    private static NotificationServiceImpl TryGetDefaultDroidNotificationService() => LocalNotificationCenter.Current is NotificationServiceImpl notificationService
            ? notificationService
            : new NotificationServiceImpl();
}