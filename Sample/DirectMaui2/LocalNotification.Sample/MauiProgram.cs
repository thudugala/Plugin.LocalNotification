using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using Plugin.LocalNotification.Core;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.Core.Models.AndroidOption;
using Plugin.LocalNotification.Core.Models.AppleOption;
using Plugin.LocalNotification.Geofence;

namespace LocalNotification.Sample;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
        //.UseLocalNotification();
        .UseLocalNotification(config =>
        {
            config.AddCategory(new NotificationCategory(NotificationCategoryType.Status)
            {
                ActionList =
                    [
                        new(100)
                        {
                            Title = "Hello",
                            Android =
                            {
                                LaunchAppWhenTapped = true,
                                IconName =
                                {
                                    ResourceName = "i2"
                                }
                            },
                            Apple =
                            {
                                Action = AppleActionType.Foreground
                            }                            
                        },
                        new(101)
                        {
                            Title = "Close",
                            Android =
                            {
                                LaunchAppWhenTapped = false,
                                IconName =
                                {
                                    ResourceName = "i3"
                                }
                            },
                            Apple =
                            {
                                Action = AppleActionType.Destructive
                            }
                        }
                    ]
            })
            .AddAndroid(android =>
            {
                android.AddChannel(new AndroidNotificationChannelRequest
                {
                    Sound = "good_things_happen"
                });
            })
            .AddiOS(iOS =>
            {
#if IOS
                //iOS.SetCustomUserNotificationCenterDelegate(new CustomUserNotificationCenterDelegate());
#endif
            });
        })
        .UseLocalNotificationGeofence();

#if DEBUG
        LocalNotificationLogger.LogLevel = LogLevel.Debug;
        //builder.Logging.AddDebug();
        builder.Logging.AddDebug();
        builder.Services.AddLogging(configure => configure.AddDebug());
#endif

        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}
