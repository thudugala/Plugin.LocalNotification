namespace Plugin.LocalNotification.Json
{
    /// <summary>
    /// 
    /// </summary>
    public interface INotificationSerializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        TValue Deserialize<TValue>(string json);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        string Serialize<TValue>(TValue value);
    }
}