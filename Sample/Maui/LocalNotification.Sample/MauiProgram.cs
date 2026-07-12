using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models.AndroidOption;
using Plugin.LocalNotification.Geofence;

namespace LocalNotification.Sample
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseLocalNotification(config =>
                {
                    config.AddAndroid(android =>
                    {
                        android.AddChannel(new AndroidNotificationChannelRequest
                        {
                            Id = "general",
                            Name = "General",
                            Description = "General sample notifications",
                            Sound = "good_things_happen"
                        });

                        android.AddChannel(new AndroidNotificationChannelRequest
                        {
                            Id = "location",
                            Name = "Location Notifications",
                            Description = "Notifications triggered when geofence boundaries are crossed"
                        });
                    });
                })
                .UseLocalNotificationGeofence()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
