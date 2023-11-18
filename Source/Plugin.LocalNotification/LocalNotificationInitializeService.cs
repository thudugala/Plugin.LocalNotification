using Microsoft.Extensions.Logging;

namespace Plugin.LocalNotification
{
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
            LocalNotificationCenter.Serializer = builder.Serializer;

            if (builder.CategorySet != null && builder.CategorySet.Any())
            {
                LocalNotificationCenter.Current.RegisterCategoryList(builder.CategorySet);
            }
        }
    }
}