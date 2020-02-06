using DllSky.Patterns;

//https://support.appsflyer.com/hc/ru/articles/213766183-AppsFlyer-SDK-Integration-Unity

public class AppsFlyerAnalyticsManager : Singleton<AppsFlyerAnalyticsManager>, IAnalyticsManager
{
    #region Variables
    public string devKeyId = "";                    //Set your AppsFlyer’s Developer key.
    public string androidPackageName = "";          //Set your Android package name    
    public string iosAppId = "";                    //Set your apple app ID. NOTE: You should enter the number only and not the "ID" prefix
    public bool isDebug = false;
    #endregion

    #region Unity methods
    private void Start()
    {
        InitAF();
    }
    #endregion

    #region Public methods
    public void SendAnalytics(AnalyticsData _data)
    {
#if UNITY_ANDROID && !UNITY_EDITOR

#elif UNITY_IOS && !UNITY_EDITOR

#endif
    }
    #endregion

    #region Private methods
    private void InitAF()
    {
        AppsFlyer.setAppsFlyerKey(devKeyId);
        AppsFlyer.setIsDebug(isDebug);

#if UNITY_ANDROID && !UNITY_EDITOR
        AppsFlyer.setAppID(androidPackageName);
        AppsFlyer.init(devKeyId);
#elif UNITY_IOS && !UNITY_EDITOR
        AppsFlyer.setAppID(iosAppId);
        AppsFlyer.trackAppLaunch ();
#endif
    }
    #endregion
}
