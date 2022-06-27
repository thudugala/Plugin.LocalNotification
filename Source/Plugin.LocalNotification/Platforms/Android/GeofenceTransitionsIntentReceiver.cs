using Android.Content;
using System;

namespace Plugin.LocalNotification.Platforms
{
    /// <summary>
    /// 
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
        ///
        /// </summary>
        public const string ReceiverName = "plugin.LocalNotification." + nameof(GeofenceTransitionsIntentReceiver);

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="intent"></param>
        public override async void OnReceive(Context context, Intent intent)
        {
            try
            {
                var notificationService = TryGetDefaultDroidNotificationService();

                var requestSerialize = intent.GetStringExtra(LocalNotificationCenter.ReturnRequest);
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
                await notificationService.ShowNow(request);
            }
            catch (Exception ex)
            {
                LocalNotificationCenter.Log(ex);
            }
        }

        private static NotificationServiceImpl TryGetDefaultDroidNotificationService()
        {
            return LocalNotificationCenter.Current is NotificationServiceImpl notificationService
                ? notificationService
                : new NotificationServiceImpl();
        }
    }
}
