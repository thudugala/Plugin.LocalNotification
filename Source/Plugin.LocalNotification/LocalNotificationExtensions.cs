using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.LifecycleEvents;

namespace Plugin.LocalNotification;

/// <summary>
/// Provides extension methods for integrating local notification functionality into a .NET MAUI application.
/// </summary>
public static class LocalNotificationExtensions
{
    /// <summary>
    /// Configures and enables local notification support for a .NET MAUI application.
    /// Registers required services and sets up platform-specific lifecycle events.
    /// </summary>
    /// <param name="builder">The <see cref="MauiAppBuilder"/> to configure.</param>
    /// <param name="configureDelegate">An optional delegate to further configure local notification options.</param>
    /// <returns>The configured <see cref="MauiAppBuilder"/>.</returns>
    public static MauiAppBuilder UseLocalNotification(this MauiAppBuilder builder, Action<ILocalNotificationBuilder>? configureDelegate = null)
    {
        var localNotificationBuilder = new LocalNotificationBuilder();
        configureDelegate?.Invoke(localNotificationBuilder);

        builder.Services.AddSingleton(localNotificationBuilder);
        builder.Services.AddSingleton(LocalNotificationCenter.Current);
        builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IMauiInitializeService, LocalNotificationInitializeService>());

        builder.ConfigureLifecycleEvents(life =>
        {
#if ANDROID
            life.AddAndroid(android =>
            {
                android.OnCreate((activity, _) =>
                {
                    LocalNotificationCenter.CreateNotificationChannelGroups(localNotificationBuilder.AndroidBuilder.GroupChannelRequestList);

                    LocalNotificationCenter.CreateNotificationChannels(localNotificationBuilder.AndroidBuilder.ChannelRequestList);

                    LocalNotificationCenter.NotifyNotificationTapped(activity.Intent);
                })
                .OnNewIntent((_, intent) =>
                {
                    LocalNotificationCenter.NotifyNotificationTapped(intent);
                });
            });
#elif IOS
            life.AddiOS(iOS =>
            {
                iOS.FinishedLaunching((application, _) =>
                {
                    LocalNotificationCenter.SetUserNotificationCenterDelegate(localNotificationBuilder.IOSBuilder.CustomUserNotificationCenterDelegate);
                    return true;
                });
                iOS.WillEnterForeground(application =>
                {
                    LocalNotificationCenter.ResetApplicationIconBadgeNumber(application);
                });
            });
#endif
        });

        return builder;
    }
}