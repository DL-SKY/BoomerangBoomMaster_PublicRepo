using DllSky.Patterns;
using Facebook.Unity;

//https://developers.facebook.com/docs/app-events/unity/?translation
//https://developers.facebook.com/docs/unity/getting-started/android

// keytool \ -exportcert \ -alias YourKeyAlias \ -storepass YourStoreKeyPassword \ -keystore PathToYourKeyStoreFile | openssl sha1 -binary | openssl base64 
// keytool -exportcert -alias android -keystore dllsky.keystore | openssl sha1 -binary | openssl base64
//sk+h5CyqoK9xK/cm48YLumFjVNU=
//kSgpw+zMvTKkRyjrwY2hjtLaC7o=

public class FaceBookAnalyticsManager : Singleton<FaceBookAnalyticsManager>, IAnalyticsManager
{
    #region Variables
    #endregion

    #region Unity methods
    private void Start()
    {
        InitFB();
    }

    private void OnApplicationPause(bool _pauseStatus)
    {
        if (!_pauseStatus)
            InitFB();
    }
    #endregion

    #region Public methods
    public void SendAnalytics(AnalyticsData _data)
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        FB.LogAppEvent(_data.customEventName, parameters: _data.eventData);
#endif
    }
    #endregion

    #region Private methods
    private void InitFB()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            //Handle FB.Init
            FB.Init( () => { FB.ActivateApp(); });
        }
#endif
    }
    #endregion
}
