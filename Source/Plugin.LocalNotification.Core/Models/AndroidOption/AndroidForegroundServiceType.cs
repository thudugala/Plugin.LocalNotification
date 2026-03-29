namespace Plugin.LocalNotification.Core.Models.AndroidOption;

/// <summary>
/// Controls the foreground service type declared for a foreground service notification on Android 10+ (API 29+).
/// Corresponds to the <c>android:foregroundServiceType</c> manifest attribute and the
/// <c>ServiceInfo.FOREGROUND_SERVICE_TYPE_*</c> constants.
/// Multiple values can be combined with the bitwise OR operator.
/// On API &lt; 29 the type is ignored and the service starts without a type constraint.
/// </summary>
[Flags]
public enum AndroidForegroundServiceType
{
    /// <summary>No specific foreground service type. Valid on all API levels.</summary>
    None = 0,

    /// <summary>
    /// The app uploads or downloads data via the network.
    /// Requires <c>android.permission.FOREGROUND_SERVICE_DATA_SYNC</c> on API 34+.
    /// </summary>
    DataSync = 1,

    /// <summary>
    /// The app plays audio or records audio.
    /// Requires <c>android.permission.FOREGROUND_SERVICE_MEDIA_PLAYBACK</c> on API 34+.
    /// </summary>
    MediaPlayback = 2,

    /// <summary>
    /// The app establishes a phone call or manages an ongoing VoIP session.
    /// Requires <c>android.permission.FOREGROUND_SERVICE_PHONE_CALL</c> on API 34+.
    /// </summary>
    PhoneCall = 4,

    /// <summary>
    /// The app tracks the device location.
    /// Requires <c>android.permission.FOREGROUND_SERVICE_LOCATION</c> on API 34+.
    /// </summary>
    Location = 8,

    /// <summary>
    /// The app connects to an external device over Bluetooth, NFC, IR, USB, or network.
    /// Requires <c>android.permission.FOREGROUND_SERVICE_CONNECTED_DEVICE</c> on API 34+.
    /// </summary>
    ConnectedDevice = 16,

    /// <summary>
    /// The app is processing a media projection that requires the user's confirmation.
    /// Available on API 29+ (Android 10+).
    /// Requires <c>android.permission.FOREGROUND_SERVICE_MEDIA_PROJECTION</c> on API 34+.
    /// </summary>
    MediaProjection = 32,

    /// <summary>
    /// The app processes a camera stream.
    /// Available on API 29+ (Android 10+).
    /// Requires <c>android.permission.FOREGROUND_SERVICE_CAMERA</c> on API 34+.
    /// </summary>
    Camera = 64,

    /// <summary>
    /// The app accesses the microphone.
    /// Available on API 30+ (Android 11+).
    /// Requires <c>android.permission.FOREGROUND_SERVICE_MICROPHONE</c> on API 34+.
    /// </summary>
    Microphone = 128,

    /// <summary>
    /// Generic health related service. Valid on API 34+ (Android 14+).
    /// Requires <c>android.permission.FOREGROUND_SERVICE_HEALTH</c>.
    /// </summary>
    Health = 256,

    /// <summary>
    /// The app sends or receives messages to or from a remote user.
    /// Valid on API 34+ (Android 14+).
    /// Requires <c>android.permission.FOREGROUND_SERVICE_REMOTE_MESSAGING</c>.
    /// </summary>
    RemoteMessaging = 512,

    /// <summary>
    /// A short foreground task typically triggered by a user interaction.
    /// Valid on API 34+ (Android 14+). The service must stop within a few minutes.
    /// Requires <c>android.permission.FOREGROUND_SERVICE_SHORT_SERVICE</c>.
    /// </summary>
    ShortService = 2048,

    /// <summary>
    /// The app manages a special-use case not covered by the other types.
    /// Valid on API 34+ (Android 14+).
    /// Requires <c>android.permission.FOREGROUND_SERVICE_SPECIAL_USE</c>.
    /// </summary>
    SpecialUse = unchecked((int)0x80000000),
}
