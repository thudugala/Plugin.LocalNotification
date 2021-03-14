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
        public static string ExtraReturnDataAndroid => "Plugin.LocalNotification.RETURN_DATA";

        /// <summary>
        /// Return Notification Key.
        /// </summary>
        public static string ExtraReturnNotification => "Plugin.LocalNotification.RETURN_NOTIFICATION";

        /// <summary>
        /// Default Channel Id.
        /// </summary>
        public static string DefaultChannelId => "Plugin.LocalNotification.GENERAL";

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
                if (intent is null)
                {
                    return;
                }

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
        /// Create Notification Channel Group with builder when API >= 26.
        /// If you'd like to further organize the appearance of your channels in the settings UI, you can create channel groups.
        /// This is a good idea when your app supports multiple user accounts (such as for work profiles),
        /// so you can create a notification channel group for each account.
        /// This way, users can easily identify and control multiple notification channels that have identical names.
        /// </summary>
        /// <param name="builder"></param>
        public static void CreateNotificationChannelGroup(Func<NotificationChannelGroupRequestBuilder, NotificationChannelGroupRequest> builder) => CreateNotificationChannelGroup(builder.Invoke(new NotificationChannelGroupRequestBuilder()));

        /// <summary>
        /// Create Notification Channel Group when API >= 26.
        /// If you'd like to further organize the appearance of your channels in the settings UI, you can create channel groups.
        /// This is a good idea when your app supports multiple user accounts (such as for work profiles),
        /// so you can create a notification channel group for each account.
        /// This way, users can easily identify and control multiple notification channels that have identical names.
        /// </summary>
        /// <param name="request"></param>
        public static void CreateNotificationChannelGroup(NotificationChannelGroupRequest request)
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

            if (request is null ||
                string.IsNullOrWhiteSpace(request.Group) ||
                string.IsNullOrWhiteSpace(request.Name))
            {
                return;
            }

            using (var channelGroup = new NotificationChannelGroup(request.Group, request.Name))
            {
                notificationManager.CreateNotificationChannelGroup(channelGroup);
            }
        }

        /// <summary>
        /// Create Notification Channel with builder when API >= 26.
        /// </summary>
        /// <param name="builder"></param>
        public static void CreateNotificationChannel(Func<NotificationChannelRequestBuilder, NotificationChannelRequest> builder) => CreateNotificationChannel(builder.Invoke(new NotificationChannelRequestBuilder()));

        /// <summary>
        /// Create Notification Channel when API >= 26.
        /// </summary>
        /// <param name="request"></param>
        public static void CreateNotificationChannel(NotificationChannelRequest request = null)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return;
            }

            if (Application.Context.GetSystemService(Context.NotificationService) is not NotificationManager
                notificationManager)
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

            if (string.IsNullOrWhiteSpace(request.Id))
            {
                request.Id = DefaultChannelId;
            }

            // you can't change the importance or other notification behaviors after this.
            // once you create the channel, you cannot change these settings and
            // the user has final control of whether these behaviors are active.
            using var channel = new NotificationChannel(request.Id, request.Name, request.Importance)
            {
                Description = request.Description,
                Group = request.Group,
                LightColor = request.LightColor,
                LockscreenVisibility = request.LockscreenVisibility,                
            };
            var soundUri = GetSoundUri(request.Sound);
            if (soundUri != null)
            {
                using var audioAttributesBuilder = new AudioAttributes.Builder();
                var audioAttributes = audioAttributesBuilder.SetUsage(AudioUsageKind.Notification)
                    .SetContentType(AudioContentType.Music)
                    .Build();

                channel.SetSound(soundUri, audioAttributes);
            }

            if (request.VibrationPattern != null)
            {
                channel.SetVibrationPattern(request.VibrationPattern);
            }

            channel.SetShowBadge(request.ShowBadge);
            channel.EnableLights(request.EnableLights);
            channel.EnableVibration(request.EnableVibration);
            channel.SetBypassDnd(request.CanBypassDnd);

            notificationManager.CreateNotificationChannel(channel);
        }

        internal static Android.Net.Uri GetSoundUri(string soundFileName)
        {
            if (string.IsNullOrWhiteSpace(soundFileName))
            {
                return null;
            }

            if (soundFileName.Contains("://", StringComparison.InvariantCulture) == false)
            {
                soundFileName =
                    $"{ContentResolver.SchemeAndroidResource}://{Application.Context.PackageName}/raw/{soundFileName}";
            }

            return Android.Net.Uri.Parse(soundFileName);
        }
    }
}