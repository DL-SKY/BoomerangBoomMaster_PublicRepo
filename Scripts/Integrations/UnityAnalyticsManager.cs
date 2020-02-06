using DllSky.Patterns;
using UnityEngine.Analytics;

public class UnityAnalyticsManager : Singleton<UnityAnalyticsManager>, IAnalyticsManager
{
    #region Public methods
    public void SendAnalytics(AnalyticsData _data)
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        Analytics.CustomEvent(_data.customEventName, _data.eventData);
#endif
    }
    #endregion
}
