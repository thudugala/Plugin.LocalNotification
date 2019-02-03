using Android.Content;
using System;

namespace Plugin.LocalNotification.Platform.Droid
{
    [BroadcastReceiver(Enabled = true, Label = "Local Notifications Broadcast Receiver")]
    internal class ScheduledNotificationReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                if (intent.HasExtra(LocalNotificationService.ExtraReturnNotification) == false)
                {
                    return;
                }

                var serializedNotification = intent.GetStringExtra(LocalNotificationService.ExtraReturnNotification);
                var notification = ObjectSerializer<LocalNotification>.DeserializeObject(serializedNotification);

                var notificationService = new LocalNotificationService();
                notificationService.Show(notification);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}