using Android.Content;
using System;

namespace Plugin.LocalNotification
{
    public static partial class NotificationCenter
    {
        /// <summary>
        /// Get or Set Resource Icon to display.
        /// </summary>
        public static int NotificationIconId { get; set; }

        /// <summary>
        /// Return Data Key.
        /// </summary>
        internal static string ExtraReturnDataAndroid = "Plugin.LocalNotification.RETURN_DATA";

        /// <summary>
        /// Return Notification Key.
        /// </summary>
        internal static string ExtraReturnNotification = "Plugin.LocalNotification.RETURN_NOTIFICATION";

        static NotificationCenter()
        {
            try
            {
                Current = new Platform.Droid.NotificationServiceImpl();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Notify Local Notification Tapped.
        /// </summary>
        /// <param name="intent"></param>
        public static void NotifyNotificationTapped(Intent intent)
        {
            try
            {
                if (intent.HasExtra(ExtraReturnDataAndroid) == false)
                {
                    return;
                }

                var subscribeItem = new NotificationTappedEventArgs
                {
                    Data = intent.GetStringExtra(ExtraReturnDataAndroid)
                };

                Current.OnNotificationTapped(subscribeItem);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}