using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Plugin.LocalNotification.Platform.Droid;
using System;

namespace Plugin.LocalNotification
{
    public static partial class NotificationCenter
    {
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

        /// <summary>
        /// Create Notification Channel when API is equal or above 26.
        /// </summary>
        /// <param name="request"></param>
        public static void CreateNotificationChannel(NotificationChannelRequest request = null)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }

            if (!(Application.Context.GetSystemService(Context.NotificationService) is NotificationManager
                notificationManager))
            {
                return;
            }

            if (request is null)
            {
                request = new NotificationChannelRequest();
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                request.Name = "General";
            }

            var channelId = GetChannelId(request.Name);

            // you can't change the importance or other notification behaviors after this.
            // once you create the channel, you cannot change these settings and
            // the user has final control of whether these behaviors are active.
            var channel = new NotificationChannel(channelId, request.Name, request.Importance)
            {
                Description = request.Description,
                Group = request.Group,
                LightColor = request.LightColor,
                LockscreenVisibility = request.LockscreenVisibility,
            };
            var soundUri = GetSoundUri(request.Sound);
            if (soundUri != null)
            {
                var audioAttributes = new AudioAttributes.Builder()
                    .SetUsage(AudioUsageKind.Notification)
                    .SetContentType(AudioContentType.Music)
                    .Build();
                channel.SetSound(soundUri, audioAttributes);
            }

            channel.SetShowBadge(true);
            channel.EnableLights(true);
            channel.EnableVibration(true);

            notificationManager.CreateNotificationChannel(channel);
        }

        internal static Android.Net.Uri GetSoundUri(string soundFileName)
        {
            if (string.IsNullOrWhiteSpace(soundFileName))
            {
                return null;
            }

            if (soundFileName.Contains("://") == false)
            {
                soundFileName =
                    $"{ContentResolver.SchemeAndroidResource}://{Application.Context.PackageName}/raw/{soundFileName}";
            }
            return Android.Net.Uri.Parse(soundFileName);
        }

        internal static string GetChannelId(string channelName)
        {
            return $"{Application.Context.PackageName}.{channelName}";
        }
    }
}