using Android.App;
using Android.Content;
using Android.Support.V4.App;
using System;
using System.Diagnostics;
using Plugin.LocalNotification.Platform.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(LocalNotificationService))]
namespace Plugin.LocalNotification.Platform.Droid
{
    /// <inheritdoc />
    [Android.Runtime.Preserve]
    public class LocalNotificationService : ILocalNotificationService
    {
        /// <summary>
        /// Return Data Key.
        /// </summary>
        public static string ExtraReturnData = "Plugin.LocalNotification.Platform.Android.RETURN_DATA";

        /// <summary>
        /// Get or Set Resource Icon to display.
        /// </summary>
        public static int NotificationIconId { get; set; }

        private readonly NotificationManager _notificationManager;

        /// <inheritdoc />
        public LocalNotificationService()
        {
            try
            {
                _notificationManager = Application.Context.GetSystemService(Android.Content.Context.NotificationService) as NotificationManager;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
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
                if (intent.HasExtra(ExtraReturnData) == false)
                {
                    return;
                }

                var subscribeItem = new LocalNotificationTappedEvent
                {
                    Data = intent.GetStringArrayListExtra(ExtraReturnData)
                };
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    Xamarin.Forms.MessagingCenter.Instance.Send(subscribeItem, typeof(LocalNotificationTappedEvent).FullName);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <inheritdoc />
        public void Cancel(int notificationId)
        {
            try
            {
                _notificationManager.Cancel(notificationId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <inheritdoc />
        public void CancelAll()
        {
            try
            {
                _notificationManager.CancelAll();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <inheritdoc />
        public void Show(LocalNotification localNotification)
        {
            try
            {
                if (localNotification is null)
                {
                    return;
                }

                Cancel(localNotification.NotificationId);

                var notificationIntent = Application.Context.PackageManager.GetLaunchIntentForPackage(Application.Context.PackageName);
                notificationIntent.SetFlags(ActivityFlags.SingleTop);

                notificationIntent.PutStringArrayListExtra(ExtraReturnData, localNotification.ReturningData);

                var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, notificationIntent, PendingIntentFlags.Immutable);

                var builder = new NotificationCompat.Builder(Application.Context);
                builder.SetContentTitle(localNotification.Title);
                builder.SetContentText(localNotification.Description);
                builder.SetStyle(new NotificationCompat.BigTextStyle().BigText(localNotification.Description));
                builder.SetNumber(localNotification.BadgeNumber);
                builder.SetAutoCancel(true);
                builder.SetDefaults((int) (NotificationDefaults.Sound | NotificationDefaults.Vibrate));
                builder.SetContentIntent(pendingIntent);

                if (NotificationIconId != 0)
                {
                    builder.SetSmallIcon(NotificationIconId);
                }
                else
                {
                    var iconId = Application.Context.ApplicationInfo.Icon;
                    if (iconId == 0)
                    {
                        Application.Context.Resources.GetIdentifier("icon", "drawable",
                            Application.Context.PackageName);
                    }
                    builder.SetSmallIcon(iconId);
                }

                var notification = builder.Build();
                notification.Defaults = NotificationDefaults.All;

                _notificationManager.Notify(localNotification.NotificationId, notification);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}