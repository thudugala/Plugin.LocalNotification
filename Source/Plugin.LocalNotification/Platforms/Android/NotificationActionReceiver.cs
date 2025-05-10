using Android.Content;

namespace Plugin.LocalNotification.Platforms;

/// <summary>
///
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
    ///
    /// </summary>
    public const string ReceiverName = "plugin.LocalNotification." + nameof(NotificationActionReceiver);

    /// <summary>
    ///
    /// </summary>
    /// <param name="context"></param>
    /// <param name="intent"></param>
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