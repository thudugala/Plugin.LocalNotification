namespace Plugin.LocalNotification.iOSOption;

/// <summary>
///
/// </summary>
public class iOSAction
{
    /// <summary>
    ///
    /// </summary>
    public iOSActionType Action { get; set; } = iOSActionType.None;

    /// <summary>
    /// An icon associated with an action.
    /// </summary>
    public iOSActionIcon Icon { get; set; } = new ();
}