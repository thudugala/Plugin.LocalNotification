namespace Plugin.LocalNotification.Core.Models;

/// <summary>
/// 
/// </summary>
public static class RequestConstants
{
    /// <summary>
    /// The key used to return a notification request.
    /// </summary>
    public const string ReturnRequest = "Plugin.LocalNotification.RETURN_REQUEST";

    /// <summary>
    /// The key used to return a notification action id.
    /// </summary>
    public const string ReturnRequestActionId = "Plugin.LocalNotification.RETURN_ActionId";

    /// <summary>
    /// The key used to indicate a notification was handled.
    /// </summary>
    public const string ReturnRequestHandled = "Plugin.LocalNotification.RETURN_Handled";

    /// <summary>
    /// The <c>RemoteInput</c> result key used for inline-reply actions on Android.
    /// </summary>
    public const string RemoteInputKey = "Plugin.LocalNotification.REMOTE_INPUT";
}
