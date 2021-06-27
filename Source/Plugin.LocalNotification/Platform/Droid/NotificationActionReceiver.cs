using Android.Content;
using System;

namespace Plugin.LocalNotification.Platform.Droid
{
    /// <summary>
    ///
    /// </summary>
    [BroadcastReceiver(
        Name = ReceiverName,
        Enabled = true,
        Exported = false
    )]
    public class NotificationActionReceiver : BroadcastReceiver
    {
        /// <summary>
        ///
        /// </summary>
        public const string ReceiverName = "Plugin.LocalNotification." + nameof(NotificationActionReceiver);

        /// <summary>
        ///
        /// </summary>
        public const string EntryIntentAction = ReceiverName + ".Action";

        /// <summary>
        ///
        /// </summary>
        public const string NotificationActionActionId = "Plugin.LocalNotification.ActionId";

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="intent"></param>
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                if (intent.Action != EntryIntentAction)
                {
                    return;
                }

                if (intent.HasExtra(NotificationActionActionId) == false)
                {
                    return;
                }

                if (intent.HasExtra(NotificationCenter.ReturnRequest) == false)
                {
                    return;
                }

                var actionId = intent.GetStringExtra(NotificationActionActionId);
                if (string.IsNullOrWhiteSpace(actionId))
                {
                    return;
                }

                var requestSerialize = intent.GetStringExtra(NotificationCenter.ReturnRequest);
                var request = NotificationCenter.GetRequest(requestSerialize);

                var actionArgs = new NotificationActionEventArgs
                {
                    ActionId = actionId,
                    Request = request
                };
                var notificationService = TryGetDefaultDroidNotificationService();
                notificationService.OnNotificationActionTapped(actionArgs);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private static NotificationServiceImpl TryGetDefaultDroidNotificationService()
        {
            return NotificationCenter.Current is NotificationServiceImpl notificationService
                ? notificationService
                : new NotificationServiceImpl();
        }
    }
}