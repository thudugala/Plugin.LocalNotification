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
        public static bool NotifyNotificationTapped(Intent intent)
        {
            try
            {
                if (intent is null)
                {
                    return false;
                }

                if (intent.HasExtra(ExtraReturnNotification) == false)
                {
                    return false;
                }

                var serializedNotification = intent.GetStringExtra(ExtraReturnNotification);
                if (string.IsNullOrWhiteSpace(serializedNotification))
                {
                    return false;
                }
                var notification = ObjectSerializer.DeserializeObject<NotificationRequest>(serializedNotification);

                var subscribeItem = new NotificationTappedEventArgs
                {
                    Title = notification.Title,
                    Description = notification.Description,
                    Data = notification.ReturningData
                };

                Current.OnNotificationTapped(subscribeItem);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return false;
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
        public static bool CreateNotificationChannelGroup(Func<NotificationChannelGroupRequestBuilder, NotificationChannelGroupRequest> builder) => CreateNotificationChannelGroup(builder.Invoke(new NotificationChannelGroupRequestBuilder()));

        /// <summary>
        /// Create Notification Channel Group when API >= 26.
        /// If you'd like to further organize the appearance of your channels in the settings UI, you can create channel groups.
        /// This is a good idea when your app supports multiple user accounts (such as for work profiles),
        /// so you can create a notification channel group for each account.
        /// This way, users can easily identify and control multiple notification channels that have identical names.
        /// </summary>
        /// <param name="request"></param>
        public static bool CreateNotificationChannelGroup(NotificationChannelGroupRequest request)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return false;
            }

            if (!(Application.Context.GetSystemService(Context.NotificationService) is NotificationManager
                notificationManager))
            {
                return false;
            }

            if (request is null ||
                string.IsNullOrWhiteSpace(request.Group) ||
                string.IsNullOrWhiteSpace(request.Name))
            {
                return false;
            }

            using var channelGroup = new NotificationChannelGroup(request.Group, request.Name);
            notificationManager.CreateNotificationChannelGroup(channelGroup);
            return true;
        }

        /// <summary>
        /// Create Notification Channel with builder when API >= 26.
        /// </summary>
        /// <param name="builder"></param>
        public static bool CreateNotificationChannel(Func<NotificationChannelRequestBuilder, NotificationChannelRequest> builder) => CreateNotificationChannel(builder.Invoke(new NotificationChannelRequestBuilder()));

        /// <summary>
        /// Create Notification Channel when API >= 26.
        /// </summary>
        /// <param name="request"></param>
        public static bool CreateNotificationChannel(NotificationChannelRequest request = null)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return false;
            }

            if (!(Application.Context.GetSystemService(Context.NotificationService) is NotificationManager notificationManager))
            {
                return false;
            }

            request ??= new NotificationChannelRequest();

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
                LockscreenVisibility = request.LockScreenVisibility,
            };
            var soundUri = GetSoundUri(request.Sound);
            if (soundUri != null)
            {
                using var audioAttributesBuilder = new AudioAttributes.Builder();
                var audioAttributes = audioAttributesBuilder.SetUsage(AudioUsageKind.Notification)
                        ?.SetContentType(AudioContentType.Music)
                        ?.Build();
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

            return true;
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