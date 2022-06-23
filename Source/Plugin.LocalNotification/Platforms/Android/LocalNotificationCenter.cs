using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.EventArgs;
using Plugin.LocalNotification.Platforms;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Plugin.LocalNotification
{
    public partial class LocalNotificationCenter
    {        
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

            var notificationRequest = GetRequest(requestSerialize);
            if (notificationRequest is null)
            {
                return false;
            }

            var args = new NotificationActionEventArgs
            {
                ActionId = NotificationActionEventArgs.TapActionId,
                Request = notificationRequest
            };
            Current.OnNotificationActionTapped(args);
            return true;
        }

        /// <summary>
        /// Create Notification Channel Group when API >= 26.
        /// If you'd like to further organize the appearance of your channels in the settings UI, you can create channel groups.
        /// This is a good idea when your app supports multiple user accounts (such as for work profiles),
        /// so you can create a notification channel group for each account.
        /// This way, users can easily identify and control multiple notification channels that have identical names.
        /// </summary>
        /// <param name="groupChannelRequest"></param>
        public static bool CreateNotificationChannelGroup(NotificationChannelGroupRequest groupChannelRequest)
        {
#if MONOANDROID
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return false;
            }
#elif ANDROID
            if (!OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                return false;
            }
#endif

            if (!(Application.Context.GetSystemService(Context.NotificationService) is NotificationManager
            notificationManager))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(groupChannelRequest.Group))
            {
                groupChannelRequest.Group = AndroidOptions.DefaultGroupId;
            }

            if (string.IsNullOrWhiteSpace(groupChannelRequest.Name))
            {
                groupChannelRequest.Name = AndroidOptions.DefaultGroupName;
            }

            using var channelGroup = new NotificationChannelGroup(groupChannelRequest.Group, groupChannelRequest.Name);
            notificationManager.CreateNotificationChannelGroup(channelGroup);
            return true;
        }

        /// <summary>
        /// Create Notification Channel when API >= 26.
        /// </summary>
        /// <param name="channelRequest"></param>
        public static bool CreateNotificationChannel(NotificationChannelRequest channelRequest = null)
        {
#if MONOANDROID
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                return false;
            }
#elif ANDROID
            if (!OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                return false;
            }
#endif

            if (!(Application.Context.GetSystemService(Context.NotificationService) is NotificationManager notificationManager))
            {
                return false;
            }

            channelRequest ??= new NotificationChannelRequest();

            if (string.IsNullOrWhiteSpace(channelRequest.Id))
            {
                channelRequest.Id = AndroidOptions.DefaultChannelId;
            }

            if (string.IsNullOrWhiteSpace(channelRequest.Name))
            {
                channelRequest.Name = AndroidOptions.DefaultChannelName;
            }

            // you can't change the importance or other notification behaviors after this.
            // once you create the channel, you cannot change these settings and
            // the user has final control of whether these behaviors are active.
            using var channel = new NotificationChannel(channelRequest.Id, channelRequest.Name, channelRequest.Importance.ToNative())
            {
                Description = channelRequest.Description,
                Group = channelRequest.Group,
                LightColor = channelRequest.LightColor.ToNative(),
                LockscreenVisibility = channelRequest.LockScreenVisibility.ToNative(),
            };
            var soundUri = GetSoundUri(channelRequest.Sound);
            if (soundUri != null)
            {
                using var audioAttributesBuilder = new AudioAttributes.Builder();
                var audioAttributes = audioAttributesBuilder.SetUsage(AudioUsageKind.Notification)
                        ?.SetContentType(AudioContentType.Sonification)
                        ?.Build();
                channel.SetSound(soundUri, audioAttributes);
            }

            if (channelRequest.VibrationPattern != null)
            {
                channel.SetVibrationPattern(channelRequest.VibrationPattern);
            }

            channel.SetShowBadge(channelRequest.ShowBadge);
            channel.EnableLights(channelRequest.EnableLights);
            channel.EnableVibration(channelRequest.EnableVibration);
            channel.SetBypassDnd(channelRequest.CanBypassDnd);

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
        /// <param name="callerName"></param>
        internal static void Log(string message, [CallerMemberName] string callerName = "")
        {
            Android.Util.Log.Info(Application.Context.PackageName, $"{callerName}: {message}");
            NotificationLog?.Invoke(new NotificationLogArgs
            {
                Message = $"{callerName}: {message}"
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="callerName"></param>
        internal static void Log(Exception ex, [CallerMemberName] string callerName = "")
        {
            System.Diagnostics.Debug.WriteLine(ex);
            Android.Util.Log.Error(Application.Context.PackageName, $"{callerName}: {ex.Message}");
            NotificationLog?.Invoke(new NotificationLogArgs
            {
                Error = ex
            });
        }
    }
}