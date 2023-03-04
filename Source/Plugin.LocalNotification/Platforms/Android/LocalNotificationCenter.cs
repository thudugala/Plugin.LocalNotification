using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Microsoft.Extensions.Logging;
#if ANDROID
using Microsoft.Maui.ApplicationModel;
#endif
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.EventArgs;
using Plugin.LocalNotification.Platforms;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Plugin.LocalNotification
{
    public partial class LocalNotificationCenter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> RequestNotificationPermissionAsync(NotificationPermission permission = null)
        {
#if ANDROID
            if (!OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                return false;
            }

            permission ??= new NotificationPermission();

            if (!permission.AskPermission)
            {
                return false;
            }

            var status = await Permissions.RequestAsync<NotificationPerms>();
            return status == PermissionStatus.Granted;
#else
            var result = await Task.FromResult(true);
            return result;
#endif
        }

        /// <summary>
        /// Notify Local Notification Tapped.
        /// </summary>
        /// <param name="intent"></param>
        public static void NotifyNotificationTapped(Intent intent)
        {
            try
            {
                var actionId = intent.GetIntExtra(ReturnRequestActionId, -1000);
                if (actionId == -1000)
                {
                    return;
                }

                var requestSerialize = intent.GetStringExtra(ReturnRequest);
                var request = GetRequest(requestSerialize);

                var actionArgs = new NotificationActionEventArgs
                {
                    ActionId = actionId,
                    Request = request
                };
                Current.OnNotificationActionTapped(actionArgs);
            }
            catch (Exception ex)
            {
                Log(ex);
            }
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

            if (channelRequest.EnableSound)
            {
                var soundUri = GetSoundUri(channelRequest.Sound);
                if (soundUri != null)
                {
                    using var audioAttributesBuilder = new AudioAttributes.Builder();
                    var audioAttributes = audioAttributesBuilder.SetUsage(AudioUsageKind.Notification)
                            ?.SetContentType(AudioContentType.Sonification)
                            ?.Build();
                    channel.SetSound(soundUri, audioAttributes);
                }
            }

            if (channelRequest.VibrationPattern != null)
            {
                channel.SetVibrationPattern(channelRequest.VibrationPattern);
                channel.ShouldVibrate();
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
                return RingtoneManager.GetDefaultUri(RingtoneType.Notification);
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
            var logMessage = $"{callerName}: {message}";
            Logger?.LogTrace(logMessage);
            Android.Util.Log.Info(Application.Context.PackageName, logMessage);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="callerName"></param>
        internal static void Log(Exception ex, string message = null, [CallerMemberName] string callerName = "")
        {
            var logMessage = $"{callerName}: {message}";
            Logger?.LogError(ex, logMessage);
            Android.Util.Log.Error(Application.Context.PackageName, $"{logMessage}: {ex.Message}");
        }
    }
}