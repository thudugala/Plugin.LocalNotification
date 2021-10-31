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
                    throw new ApplicationException("NotificationActionReceiver.OnReceive: Action Key Not found");
                }

                if (intent.HasExtra(NotificationActionActionId) == false)
                {
                    throw new ApplicationException("NotificationActionReceiver.OnReceive: Action Id Key Not found");
                }

                if (intent.HasExtra(NotificationCenter.ReturnRequest) == false)
                {
                    throw new ApplicationException("NotificationActionReceiver.OnReceive: Request Key Not found");
                }

                var actionId = intent.GetIntExtra(NotificationActionActionId, -1000);
                if (actionId == -1000)
                {
                    throw new ApplicationException("NotificationActionReceiver.OnReceive: Action Id Not found");
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
                NotificationCenter.Log(ex);
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