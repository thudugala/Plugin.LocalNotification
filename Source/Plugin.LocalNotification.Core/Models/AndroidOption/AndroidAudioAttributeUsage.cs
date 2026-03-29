namespace Plugin.LocalNotification.Core.Models.AndroidOption;

/// <summary>
/// Represents the intended use of an audio stream for a notification, corresponding to
/// <c>Android.Media.AudioUsageKind</c>. Used together with <see cref="AndroidOptions.AudioAttributeUsage"/>
/// to control how the system treats the notification sound (e.g. as an alarm, media, or ringtone).
/// Only applied on Android API &lt; 26 (pre-channel); on API 26+ set the usage on the
/// <see cref="AndroidNotificationChannelRequest"/>.
/// </summary>
public enum AndroidAudioAttributeUsage
{
    /// <summary>Usage for notifications. This is the default.</summary>
    Notification = 5,

    /// <summary>Usage for alarm sounds. Will play even when the device is in Do Not Disturb — Alarms mode.</summary>
    Alarm = 4,

    /// <summary>Usage for ringtones.</summary>
    NotificationRingtone = 6,

    /// <summary>Usage for media playback (music, video, etc.).</summary>
    Media = 1,

    /// <summary>Usage for in-call voice communication.</summary>
    VoiceCommunication = 2,

    /// <summary>Usage for telephony ringtone.</summary>
    Unknown = 0,
}
