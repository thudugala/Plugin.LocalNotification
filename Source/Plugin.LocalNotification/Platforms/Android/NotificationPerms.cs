﻿using Android;

namespace Plugin.LocalNotification.Platforms;

public partial class NotificationPerms : Permissions.BasePlatformPermission
{
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions
    {
        get
        {
            var result = new List<(string androidPermission, bool isRuntime)>();
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                result.Add((Manifest.Permission.PostNotifications, true));
            }
            return [.. result];
        }
    }
}