using Microsoft.Extensions.Logging;

namespace Plugin.LocalNotification;

/// <summary>
/// Initializes local notification services and configures logging and category registration for the application.
/// </summary>
public class LocalNotificationInitializeService : IMauiInitializeService
{
    /// <summary>
    /// Initializes local notification services, sets up logging, and registers notification categories.
    /// </summary>
    /// <param name="services">The application's service provider.</param>
    public void Initialize(IServiceProvider services)
    {
        LocalNotificationCenter.Logger = services.GetService<ILogger<LocalNotificationCenter>>();

        var builder = services.GetService<LocalNotificationBuilder>();
        if (builder is not null)
        {
            LocalNotificationCenter.Serializer = builder.Serializer;
            LocalNotificationCenter.Current.RegisterCategoryList(builder.CategorySet);
        }
    }
}