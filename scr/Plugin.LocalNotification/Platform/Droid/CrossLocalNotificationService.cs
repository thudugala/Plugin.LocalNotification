using System;
using Android.Content;

namespace Plugin.LocalNotification
{
    public static partial class CrossLocalNotificationService
    {
        /// <summary>
        /// Return Data Key.
        /// </summary>
        internal static string ExtraReturnDataAndroid = "Plugin.LocalNotification.RETURN_DATA";

        /// <summary>
        /// Return Notification Key.
        /// </summary>
        internal static string ExtraReturnNotification = "Plugin.LocalNotification.RETURN_NOTIFICATION";
        
        /// <summary>
        /// 
        /// </summary>
        public static void Init()
        {
            try
            {
                Current = new Platform.Droid.LocalNotificationServiceImpl();
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

                var subscribeItem = new LocalNotificationTappedEventArgs
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