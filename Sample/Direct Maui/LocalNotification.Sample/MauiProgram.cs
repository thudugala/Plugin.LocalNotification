using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.iOSOption;

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
                    ActionList = new HashSet<NotificationAction>(new List<NotificationAction>()
                        {
                            new NotificationAction(100)
                            {
                                Title = "Hello",
                                Android =
                                {
                                    LaunchAppWhenTapped = true,
                                    IconName =
                                    {
                                        ResourceName = "i2"
                                    }
                                }
                            },
                            new NotificationAction(101)
                            {
                                Title = "Close",
                                Android =
                                {
                                    LaunchAppWhenTapped = false,
                                    IconName =
                                    {
                                        ResourceName = "i3"
                                    }
                                }
                            }
                        })
                })
                .SetPermission(new NotificationPermission
                {
                    IOS =
                    {
                        LocationAuthorization = iOSLocationAuthorization.WhenInUse
                    }
                })
                .AddAndroid(android =>
                {
                    android.AddChannel(new NotificationChannelRequest
                    {
                        Sound = "good_things_happen"
                    });
                })
                .AddiOS(iOS =>
                {
#if IOS
                    iOS.UseCustomDelegate = true;
                    iOS.SetCustomUserNotificationCenterDelegate(new CustomUserNotificationCenterDelegate());
#endif
                });
            });

        builder.Services.AddLogging(logging =>
        {
            logging.AddDebug();
            //logging.AddConsole();
        });

        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}