#if IOS || MACCATALYST
using System.Runtime.Versioning;

namespace Plugin.LocalNotification;

internal static class ApplePlatform
{
    internal static bool IsCurrent() =>
#if IOS
        OperatingSystem.IsIOS();
#else
        OperatingSystem.IsMacCatalyst();
#endif

#if IOS
    [SupportedOSPlatformGuard("ios11.0")]
#else
    [SupportedOSPlatformGuard("maccatalyst11.0")]
#endif
    internal static bool IsVersion11OrLater() => IsVersionAtLeast(11);

#if IOS
    [SupportedOSPlatformGuard("ios12.0")]
#else
    [SupportedOSPlatformGuard("maccatalyst12.0")]
#endif
    internal static bool IsVersion12OrLater() => IsVersionAtLeast(12);

#if IOS
    [SupportedOSPlatformGuard("ios14.0")]
#else
    [SupportedOSPlatformGuard("maccatalyst14.0")]
#endif
    internal static bool IsVersion14OrLater() => IsVersionAtLeast(14);

#if IOS
    [SupportedOSPlatformGuard("ios15.0")]
#else
    [SupportedOSPlatformGuard("maccatalyst15.0")]
#endif
    internal static bool IsVersion15OrLater() => IsVersionAtLeast(15);

#if IOS
    [SupportedOSPlatformGuard("ios16.0")]
#else
    [SupportedOSPlatformGuard("maccatalyst16.0")]
#endif
    internal static bool IsVersion16OrLater() => IsVersionAtLeast(16);

    private static bool IsVersionAtLeast(int majorVersion) =>
#if IOS
        OperatingSystem.IsIOSVersionAtLeast(majorVersion);
#else
        OperatingSystem.IsMacCatalystVersionAtLeast(majorVersion);
#endif
}
#endif
