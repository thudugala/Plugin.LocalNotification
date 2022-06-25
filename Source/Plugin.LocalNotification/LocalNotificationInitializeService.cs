#if ANDROID || IOS
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
#endif
using System;

namespace Plugin.LocalNotification
{
#if ANDROID || IOS
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
        }
    }
#endif
}
