using UnityEngine;
using System.Collections;
using DllSky.Patterns;
using GooglePlayGames;
using System;

public static class ConstantsAchievements
{
    public const string TEST = "CgkImbiZ9cgeEAIQAg";

    //100 + (скрытое) - СПОНСОР - совершить покупку IAP
    //100 + (скрытое) - ПИКАБУШНИК - активировать промо-код "ПИКАБУ"
    public const string SPONSOR = "CgkImbiZ9cgeEAIQBA";
    public const string PIKABU = "CgkImbiZ9cgeEAIQBQ";

    //75 + ЧИТЕР - использовать промокод
    ////КОЛЛЕКЦИОНЕР - собрать 10 бумерангов
    //25 + НАЧИНАЮЩИЙ ИГРОК - набить в сумме 500 очков рейтинга
    //50 + ОПЫТНЫЙ ИГРОК - набить в сумме 2500 очков рейтинга
    //100 + МАСТЕРСКИЙ ИГРОК - набить в сумме 5000 очков рейтинга
    //150 + МАСТЕР БУМЕРАНГА - набить в сумме 10000 очков рейтинга
    //25 + МОНЕТКА - собрать бумерангом 10 монет
    //50 + КОШЕЛЕК - собрать бумерангом 100 монет
    //100 + БАНКИР - собрать бумерангом 1000 монет
    public const string CHEATER = "CgkImbiZ9cgeEAIQBg";
    //public const string COLLECTOR = "";
    public const string GAMER_1 = "CgkImbiZ9cgeEAIQBw";
    public const string GAMER_2 = "CgkImbiZ9cgeEAIQCA";
    public const string GAMER_3 = "CgkImbiZ9cgeEAIQCQ";
    public const string GAMER_4 = "CgkImbiZ9cgeEAIQCg";
    public const string MONEY_1 = "CgkImbiZ9cgeEAIQCw";
    public const string MONEY_2 = "CgkImbiZ9cgeEAIQDA";
    public const string MONEY_3 = "CgkImbiZ9cgeEAIQDQ";
}

public static class ConstantsLeaderboard
{
    public const string ANDROID_LEADERBOARD = "CgkImbiZ9cgeEAIQAQ";
}

public class SocialManager : Singleton<SocialManager>
{
    #region Variables
    private const string StoreUriAndroid = "http://play.google.com/store/apps/details?id={0}";
    #endregion

    #region Unity methods
    private void Start()
    {
        Initialize();
    }
    #endregion

    #region Public methods
    public void Initialize()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        PlayGamesPlatform.Activate();
#elif UNITY_EDITOR
        Debug.Log("[SocialManager] Initialize()");
#endif
    }

    public void Authenticate(Action<bool> _callback)
    {
        if (!Social.localUser.authenticated)
            Social.localUser.Authenticate(_callback);
    }

    public void SendLeaderboardScore(long _score, Action<bool> _callback = null)
    {
        var board = "";

#if UNITY_ANDROID && !UNITY_EDITOR
        board = ConstantsLeaderboard.ANDROID_LEADERBOARD;
        if (Social.localUser.authenticated)
            Social.ReportScore(_score, board, _callback);
#elif UNITY_EDITOR

#endif

        Debug.Log(string.Format("[SocialManager] SendLeaderboardScore({0}) on board \"{1}\"", _score, board));
    }

    public void ShowLeaderboard()
    {
        if (Social.localUser.authenticated)
            Social.ShowLeaderboardUI();
        else
            Authenticate((result) => { if (result) Social.ShowLeaderboardUI(); });
    }

    public void SendProgressAchievement(string _achieventID, double _progress = 100.0d, Action<bool> _callback = null)
    {
        //progress = _progress
#if UNITY_ANDROID && !UNITY_EDITOR
        if (Social.localUser.authenticated)
            Social.ReportProgress(_achieventID, _progress, _callback);
#elif UNITY_EDITOR

#endif

        Debug.Log(string.Format("[SocialManager] SendProgressAchievement({0}, {1})", _achieventID, _progress));
    }

    public void SendIncrementAchievement(string _achieventID, int _step, Action<bool> _callback = null)
    {
        //progress += _step
#if UNITY_ANDROID && !UNITY_EDITOR
        if (Social.localUser.authenticated)
            PlayGamesPlatform.Instance.IncrementAchievement(_achieventID, _step, _callback);
#elif UNITY_EDITOR

#endif

        Debug.Log(string.Format("[SocialManager] SendIncrementAchievement({0}, {1})", _achieventID, _step));
    }

    public void ShowAchievements()
    {
        if (Social.localUser.authenticated)
            Social.ShowAchievementsUI();
        else
            Authenticate((result) => { if (result) Social.ShowAchievementsUI(); });
    }

    public void OpenStore()
    {
#if UNITY_ANDROID
        Application.OpenURL(string.Format(StoreUriAndroid, Application.identifier));
#endif
    }
    #endregion
}
