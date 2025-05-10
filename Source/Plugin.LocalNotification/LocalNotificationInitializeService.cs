using Microsoft.Extensions.Logging;

namespace Plugin.LocalNotification;

/// <summary>
///
/// </summary>
public class LocalNotificationInitializeService : IMauiInitializeService
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="services"></param>
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