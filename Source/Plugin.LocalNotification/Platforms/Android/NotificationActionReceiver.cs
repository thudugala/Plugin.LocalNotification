using Android.Content;

namespace Plugin.LocalNotification.Platforms;

/// <summary>
/// Broadcast receiver for handling notification action intents in Android and triggering notification tap events.
/// </summary>
[BroadcastReceiver(
    Name = ReceiverName,
    Enabled = true,
    Exported = false,
    Label = "Plugin LocalNotification Action Receiver"
)]
internal class NotificationActionReceiver : BroadcastReceiver
{
    /// <summary>
    /// The name of the broadcast receiver.
    /// </summary>
    public const string ReceiverName = "plugin.LocalNotification." + nameof(NotificationActionReceiver);

    /// <summary>
    /// Called when a notification action intent is received. Triggers the notification tap event.
    /// </summary>
    /// <param name="context">The context in which the receiver is running.</param>
    /// <param name="intent">The intent being received.</param>
    public override void OnReceive(Context? context, Intent? intent)
    {
        try
        {
            LocalNotificationCenter.NotifyNotificationTapped(intent);
        }
        catch (Exception ex)
        {
            LocalNotificationCenter.Log(ex);
        }
    }
}