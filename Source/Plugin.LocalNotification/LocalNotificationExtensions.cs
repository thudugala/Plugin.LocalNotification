#if ANDROID || IOS
#if ANDROID
using Android.OS;
#endif
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;
#endif
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;
using System.Collections.Generic;
using System;

namespace Plugin.LocalNotification
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILocalNotificationBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelRequest"></param>
        /// <returns></returns>
        ILocalNotificationBuilder AddAndroidChannel(NotificationChannelRequest channelRequest);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupChannelRequest"></param>
        /// <returns></returns>
        ILocalNotificationBuilder AddAndroidChannelGroup(NotificationChannelGroupRequest groupChannelRequest);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        ILocalNotificationBuilder SetiOSNotificationPermission(iOSNotificationPermission permission);
    }

    /// <summary>
    /// 
    /// </summary>
    public class LocalNotificationBuilder : ILocalNotificationBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        public IList<NotificationChannelRequest> AndroidChannelRequestList { get; }

        /// <summary>
        /// 
        /// </summary>
        public IList<NotificationChannelGroupRequest> AndroidGroupChannelRequestList { get; }

        /// <summary>
        /// 
        /// </summary>
        public iOSNotificationPermission iOSNotificationPermission { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public LocalNotificationBuilder()
        {
            AndroidChannelRequestList = new List<NotificationChannelRequest>();
            AndroidGroupChannelRequestList = new List<NotificationChannelGroupRequest>();
        }

        /// <inheritdoc/>
        public ILocalNotificationBuilder AddAndroidChannel(NotificationChannelRequest channelRequest)
        {
            AndroidChannelRequestList.Add(channelRequest);
            return this;
        }

        /// <inheritdoc/>
        public ILocalNotificationBuilder AddAndroidChannelGroup(NotificationChannelGroupRequest groupChannelRequest)
        {
            AndroidGroupChannelRequestList.Add(groupChannelRequest);
            return this;
        }

        /// <inheritdoc/>
        public ILocalNotificationBuilder SetiOSNotificationPermission(iOSNotificationPermission permission)
        {
            iOSNotificationPermission = permission;
            return this;
        }
    }

    /// <summary>
    /// 
    /// </summary> 
    public static class LocalNotificationExtensions
    {
#if ANDROID || IOS               

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureDelegate"></param>
        /// <returns></returns>
        public static MauiAppBuilder UseLocalNotification(this MauiAppBuilder builder, Action<ILocalNotificationBuilder> configureDelegate = null)
        {
            var localNotificationBuilder = new LocalNotificationBuilder();
            configureDelegate?.Invoke(localNotificationBuilder);

            builder.ConfigureLifecycleEvents(life =>
            {
#if ANDROID
                life.AddAndroid(android =>
                {
                    android.OnCreate((activity, savedInstanceState) =>
                    { 
                        if (localNotificationBuilder.AndroidChannelRequestList.Count > 0)
                        {
                            foreach(var channelRequest in localNotificationBuilder.AndroidChannelRequestList)
                            {
                                LocalNotificationCenter.CreateNotificationChannel(channelRequest);
                            }
                        }
                        if (localNotificationBuilder.AndroidGroupChannelRequestList.Count > 0)
                        {
                            foreach (var groupChannelReques in localNotificationBuilder.AndroidGroupChannelRequestList)
                            {
                                LocalNotificationCenter.CreateNotificationChannelGroup(groupChannelReques);
                            }
                        }
                        LocalNotificationCenter.NotifyNotificationTapped(activity.Intent);
                    })
                    .OnNewIntent((activity, intent) =>
                    {
                        LocalNotificationCenter.NotifyNotificationTapped(intent);
                    });
                });
#elif IOS
                life.AddiOS(iOS =>
                {
                    iOS.FinishedLaunching((application, _) =>
                    {
                        if (localNotificationBuilder.iOSNotificationPermission.SetUserNotificationCenterDelegate)
                        {
                            LocalNotificationCenter.SetCustomUserNotificationCenterDelegate();
                        }
                        LocalNotificationCenter.RequestNotificationPermissionAsync(localNotificationBuilder.iOSNotificationPermission).GetAwaiter().GetResult();
                        return true;
                    });
                    iOS.WillEnterForeground(application =>
                    {
                        LocalNotificationCenter.ResetApplicationIconBadgeNumber(application).GetAwaiter().GetResult();
                    });
                });
#endif
            });

            return builder;
        }
#endif
    }
}
