#if IOS
using Plugin.LocalNotification.Platforms;
#endif

namespace Plugin.LocalNotification.iOSOption
{
    /// <summary>
    /// 
    /// </summary>
    public interface IiOSLocalNotificationBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        IiOSLocalNotificationBuilder SetPermission(iOSNotificationPermission permission);

#if IOS    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customUserNotificationCenterDelegate"></param>
        /// <returns></returns>
        IiOSLocalNotificationBuilder SetCustomUserNotificationCenterDelegate(UserNotificationCenterDelegate customUserNotificationCenterDelegate) ;
#endif
    }
}
