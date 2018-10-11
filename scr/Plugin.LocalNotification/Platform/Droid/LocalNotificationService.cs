using Android.App;
using Android.Content;
using Android.Support.V4.App;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Android.OS;
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
                if (intent.HasExtra(ExtraReturnData) == false)
                {
                    return;
                }

                var serializeReturningData = intent.GetStringExtra(ExtraReturnData);
                
                var subscribeItem = new LocalNotificationTappedEvent
                {
                    Data = DeserializeNotification(serializeReturningData)
                };
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    Xamarin.Forms.MessagingCenter.Instance.Send(subscribeItem, typeof(LocalNotificationTappedEvent).FullName);
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
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
                System.Diagnostics.Debug.WriteLine(ex);
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
                System.Diagnostics.Debug.WriteLine(ex);
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
                
                var builder = new NotificationCompat.Builder(Application.Context);
                builder.SetContentTitle(localNotification.Title);
                builder.SetContentText(localNotification.Description);
                builder.SetStyle(new NotificationCompat.BigTextStyle().BigText(localNotification.Description));
                builder.SetNumber(localNotification.BadgeNumber);
                builder.SetAutoCancel(true);
                builder.SetDefaults((int) (NotificationDefaults.Sound | NotificationDefaults.Vibrate));

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var channelId = $"{Application.Context.PackageName}.general";

                    var channel = new NotificationChannel(channelId, "General", NotificationImportance.Default);

                    _notificationManager.CreateNotificationChannel(channel);
                    
                    builder.SetChannelId(channelId);
                }

                var notificationIntent = Application.Context.PackageManager.GetLaunchIntentForPackage(Application.Context.PackageName);
                notificationIntent.SetFlags(ActivityFlags.SingleTop);

                var serializeReturningData = SerializeReturningData(localNotification.ReturningData);

                notificationIntent.PutExtra(ExtraReturnData, serializeReturningData);

                var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
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
                        Application.Context.Resources.GetIdentifier("Icon", "drawable",
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
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private string SerializeReturningData(IList<string> returningData)
        {
            var returningType = returningData.GetType();
            var xmlSerializer = new XmlSerializer(returningType);
            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, returningData);
                return stringWriter.ToString();
            }
        }

        private static List<string> DeserializeNotification(string notificationString)
        {
            var returningType = typeof(List<string>);
            var xmlSerializer = new XmlSerializer(returningType);
            using (var stringReader = new StringReader(notificationString))
            {
                var notification = (List<string>)xmlSerializer.Deserialize(stringReader);
                return notification;
            }
        }
    }
}