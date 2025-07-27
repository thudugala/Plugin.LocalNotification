namespace Plugin.LocalNotification.AndroidOption;

/// <summary>
/// Specifies flags for configuring Android PendingIntent behavior in local notifications.
/// </summary>
[Flags]
public enum AndroidPendingIntentFlags
{
    /// <summary>
    /// If the described PendingIntent already exists, the current one should be canceled before generating a new one.
    /// Useful when changing the extra data in the Intent.
    /// </summary>
    CancelCurrent = 0x10000000,

    /// <summary>
    /// The created PendingIntent should be immutable. Additional intent arguments passed to send methods will be ignored.
    /// </summary>
    Immutable = 0x4000000,

    /// <summary>
    /// If the described PendingIntent does not already exist, simply return null instead of creating it.
    /// </summary>
    NoCreate = 0x20000000,

    /// <summary>
    /// The PendingIntent can be used only once.
    /// </summary>
    OneShot = 0x40000000,

    /// <summary>
    /// If the described PendingIntent already exists, keep it but replace its extra data with what is in this new Intent.
    /// Useful when only the extras change and you don't care about previous entities launching it with new extras.
    /// </summary>
    UpdateCurrent = 0x8000000
}