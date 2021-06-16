namespace Plugin.LocalNotification
{
    /// <summary>
    /// The behaviors you can apply to an action
    /// </summary>
    public enum iOSActionType
    {
        /// <summary>
        ///
        /// </summary>
        None,

        /// <summary>
        /// The action performs a destructive task
        /// </summary>
        Destructive,

        /// <summary>
        /// The action can be performed only on an unlocked device
        /// </summary>
        AuthenticationRequired,

        /// <summary>
        /// The action causes the app to launch in the foreground
        /// </summary>
        Foreground
    }
}