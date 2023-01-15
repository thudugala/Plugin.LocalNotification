#if NET6_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
#endif
using System;
using System.Linq;

namespace Plugin.LocalNotification
{
#if NET6_0_OR_GREATER
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
#endif
}
