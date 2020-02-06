using UnityEngine;
using System.Collections;
using DllSky.Patterns;
using UnityEngine.Advertisements;
using System;

//https://unityads.unity3d.com/help/unity/integration-guide-unity

public class AdsManager : Singleton<AdsManager>, IUnityAdsListener
{
    #region Variables
    public string androidGameID = "3344786";
    public string iosGameID = "3344787";
    public bool testMode = false;

    private Action callback;
    private Action badCallback;
    #endregion

    #region Unity methods
    private void Start()
    {
        Advertisement.AddListener(this);

        Initialize();
    }
    #endregion

    #region Public methods
    public void ShowVideo(Action _callback = null, Action _badCallback = null)
    {
        callback = _callback;
        badCallback = _badCallback;

        Advertisement.Show(ConstantsAds.INTERSTITIAL);
    }

    public void ShowRewardedVideo(Action _callback = null, Action _badCallback = null)
    {
        callback = _callback;
        badCallback = _badCallback;

        Advertisement.Show(ConstantsAds.REWARDED);
    }
    #endregion

    #region Private methods
    private void Initialize()
    {
#if UNITY_EDITOR
        Advertisement.Initialize(androidGameID, testMode);
#elif UNITY_ANDROID
        Advertisement.Initialize(androidGameID, testMode);
#elif (UNITY_IOS || UNITY_IPHONE)
        Advertisement.Initialize(iosGameID, testMode);
#endif
    }
    #endregion    

    #region Ads interfaces methods
    public void OnUnityAdsReady(string _placementId)
    {
        // If the ready Placement is rewarded, activate the button: 
    }

    public void OnUnityAdsDidStart(string _placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }

    public void OnUnityAdsDidFinish(string _placementId, ShowResult _showResult)
    {
        // Define conditional logic for each ad completion status:
        if (_showResult == ShowResult.Finished)
        {
            // Reward the user for watching the ad to completion.

            if (_placementId == ConstantsAds.REWARDED)
                GameManager.Instance.adsToday++;
            else if (_placementId == ConstantsAds.INTERSTITIAL)
                GameManager.Instance.countReturnToMainMenu = 0;

            callback?.Invoke();
        }
        else if (_showResult == ShowResult.Skipped)
        {
            // Do not reward the user for skipping the ad.

            if (_placementId == ConstantsAds.REWARDED)
                GameManager.Instance.adsToday++;
            else if (_placementId == ConstantsAds.INTERSTITIAL)
                GameManager.Instance.countReturnToMainMenu = 0;

            badCallback?.Invoke();
        }
        else if (_showResult == ShowResult.Failed)
        {
            //Debug.LogWarning(“The ad did not finish due to an error.”);

            badCallback?.Invoke();
        }

        Debug.LogWarning(_showResult.ToString());
    }

    public void OnUnityAdsDidError(string _message)
    {
        // Log the error.

        badCallback?.Invoke();
    }
    #endregion
}
