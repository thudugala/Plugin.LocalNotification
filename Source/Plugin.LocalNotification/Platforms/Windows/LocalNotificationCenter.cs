using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Uwp.Notifications;
using Plugin.LocalNotification.EventArgs;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Plugin.LocalNotification
{
    public partial class LocalNotificationCenter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        public static async Task<bool> RequestNotificationPermissionAsync(NotificationPermission permission = null)
        {
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Notify Local Notification Tapped.
        /// </summary>
        /// <param name="arguments"></param>
        public static void NotifyNotificationTapped(string arguments)
        {
            try
            {                
                var args = ToastArguments.Parse(arguments);

                var actionId = args.GetInt(ReturnRequestActionId);
                if (actionId == -1000)
                {
                    return;
                }
                if (args.TryGetValue(ReturnRequest, out var requestSerialize))
                {
                    return;
                }
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
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callerName"></param>
        internal static void Log(string message, [CallerMemberName] string callerName = "")
        {
            var logMessage = $"{callerName}: {message}";
            Logger?.Log(LogLevel, logMessage);
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
        }
    }
}
