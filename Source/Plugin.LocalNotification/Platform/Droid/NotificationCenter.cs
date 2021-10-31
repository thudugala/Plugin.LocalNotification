using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.Platform.Droid;
using System;
using System.IO;
using Plugin.LocalNotification.Json;

namespace Plugin.LocalNotification
{
    public static partial class NotificationCenter
    {
        static NotificationCenter()
        {
            try
            {
                Current = new Platform.Droid.NotificationServiceImpl();
                Serializer = new NotificationSerializer();
            }
            catch (Exception ex)
            {
                Log(ex);
            }
        }

        /// <summary>
        /// Notify Local Notification Tapped.
        /// </summary>
        /// <param name="intent"></param>
        public static bool NotifyNotificationTapped(Intent intent)
        {
            if (intent is null)
            {
                return false;
            }

            if (intent.HasExtra(ReturnRequest) == false)
            {
                return false;
            }

            var requestSerialize = intent.GetStringExtra(ReturnRequest);
            if (string.IsNullOrWhiteSpace(requestSerialize))
            {
                return false;
            }

            var notification = GetRequest(requestSerialize);
            if (notification is null)
            {
                return false;
            }

            var subscribeItem = new NotificationEventArgs
            {
                Request = notification
            };

            Current.OnNotificationTapped(subscribeItem);
            return true;
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
        public static bool CreateNotificationChannelGroup(NotificationChannelGroupRequest request = null)
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

            request ??= new NotificationChannelGroupRequest();

            if (string.IsNullOrWhiteSpace(request.Group))
            {
                request.Group = AndroidOptions.DefaultGroupId;
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                request.Name = AndroidOptions.DefaultGroupName;
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

            if (string.IsNullOrWhiteSpace(request.Id))
            {
                request.Id = AndroidOptions.DefaultChannelId;
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                request.Name = AndroidOptions.DefaultChannelName;
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

            if (soundFileName.Contains("://", StringComparison.InvariantCulture))
            {
                return Android.Net.Uri.Parse(soundFileName);
            }

            soundFileName = Path.GetFileNameWithoutExtension(soundFileName);
            soundFileName =
                $"{ContentResolver.SchemeAndroidResource}://{Application.Context.PackageName}/raw/{soundFileName}";

            return Android.Net.Uri.Parse(soundFileName);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        internal static void Log(string message)
        {
            Android.Util.Log.Info(Application.Context.PackageName, message);
            NotificationLog?.Invoke(new NotificationLogArgs
            {
                Message = message
            });
        }

        internal static void Log(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            Android.Util.Log.Error(Application.Context.PackageName, ex.Message);
            NotificationLog?.Invoke(new NotificationLogArgs
            {
                Error = ex
            });
        }
    }
}