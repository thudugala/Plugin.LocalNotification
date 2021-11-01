namespace Plugin.LocalNotification.AndroidOption
{
    /// <summary>
    /// 
    /// </summary>
    public enum AndroidPendingIntentFlags
    {
        /// <summary>
        /// Flag indicating that if the described PendingIntent already exists, the current one should be canceled before generating a new one.
        /// You can use this to retrieve a new PendingIntent when you are only changing the extra data in the Intent; 
        /// by canceling the previous pending intent, this ensures that only entities given the new data will be able to launch it.
        /// If this assurance is not an issue, consider
        /// </summary>
        CancelCurrent = 0x10000000,
        /// <summary>
        /// Flag indicating that the created PendingIntent should be immutable. 
        /// This means that the additional intent argument passed to the send methods to fill in unpopulated properties of this intent will be ignored.
        /// </summary>
        Immutable = 0x4000000,
        /// <summary>
        /// Flag indicating that if the described PendingIntent does not already exist, then simply return null instead of creating it.
        /// </summary>
        NoCreate = 0x20000000,
        /// <summary>
        /// Flag indicating that this PendingIntent can be used only once. 
        /// </summary>
        OneShot = 0x40000000,
        /// <summary>
        /// Flag indicating that if the described PendingIntent already exists, then keep it but replace its extra data with what is in this new Intent.
        /// This can be used if you are creating intents where only the extras change, 
        /// and don't care that any entities that received your previous PendingIntent will be able to launch it with your new extras even if they are not explicitly given to it. 
        /// </summary>
        UpdateCurrent = 0x8000000
    }
}
