using Android.Content;
using Plugin.LocalNotification.Core;
using Plugin.LocalNotification.Core.Models;

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
            var requestSerialize = intent?.GetStringExtra(RequestConstants.ReturnRequest);
            if (string.IsNullOrWhiteSpace(requestSerialize))
            {
                LocalNotificationLogger.Log("Request Json Not Found");
                return;
            }

            var showFunc = GeofenceHandlerRegistry.ShowNotificationFromSerializedRequest;
            if (showFunc is null)
            {
                LocalNotificationLogger.Log("Plugin.LocalNotification is not available to show notification");
                return;
            }

            _ = await showFunc(requestSerialize);
        }
        catch (Exception ex)
        {
            LocalNotificationLogger.Log(ex);
        }
    }
}
