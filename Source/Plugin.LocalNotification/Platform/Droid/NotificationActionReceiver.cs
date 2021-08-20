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
                    System.Diagnostics.Debug.WriteLine("Action Key Not found");
                    return;
                }

                if (intent.HasExtra(NotificationActionActionId) == false)
                {
                    System.Diagnostics.Debug.WriteLine("Action Id Key Not found");
                    return;
                }

                if (intent.HasExtra(NotificationCenter.ReturnRequest) == false)
                {
                    System.Diagnostics.Debug.WriteLine("Request Key Not found");
                    return;
                }

                var actionId = intent.GetIntExtra(NotificationActionActionId, -1000);
                if (actionId == -1000)
                {
                    System.Diagnostics.Debug.WriteLine("Action Id Not found");
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