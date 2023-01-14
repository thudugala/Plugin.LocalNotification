#if ANDROID
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls.PlatformConfiguration;
#endif
using System;
using System.Collections.Generic;
using System.Text;

namespace Plugin.LocalNotification.Platforms
{
#if ANDROID
    public partial class NotificationPerms : Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
        new List<(string androidPermission, bool isRuntime)>
        {
            ("android.permission.POST_NOTIFICATIONS", true),
        }.ToArray();
    }
#endif
}
