using UnityEngine;
using System.Collections;
using TMPro;

public class UiGameOverController : UiBase
{
    #region Variables
    [Header("Score")]
    public TextMeshProUGUI currScoreText;
    public TextMeshProUGUI bestScoreText;

    [Header("Buttons")]
    public TextMeshProUGUI btnAdsText;
    public TextMeshProUGUI btnComText;
    public GameObject btnAdsActive;
    public GameObject btnAdsDisable;

    private GameManager instance;
    #endregion

    #region Unity methods
    private void OnEnable()
    {
        Initialize();
    }

    private void Update()
    {
        //if (Input.GetKeyUp(KeyCode.Escape))
        //{
        //    OnClickGameOverCommon();
        //}
    }
    #endregion

    #region Public methods
    public void OnClickGameOverWithAds()
    {
        //Проверка на кол-во уже просмотренной рекламы
        if (GameManager.Instance.adsToday >= ConstantsSettings.maxAds)
        {
            OnClickGameOverCommon();
            return;
        }

        SetVisibleAdsButton(false);

        //Просмотр рекламы
        AdsManager.Instance.ShowRewardedVideo(OnReturnToMainMenuAds, OnUpdateAdsButton);
    }

    public void OnClickGameOverCommon()
    {
        //Забор награды обычный
        var addMoney = instance.addMoney;

        instance.ChangeMoney(addMoney);
        instance.moneyToday += addMoney;

        //Analytics
        var analyticsData = new AnalyticsData();
        analyticsData.customEventName = ConstantsAnalyticsEventName.GAME_OVER;
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.SCORE, instance.curScore);
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.ADD_MONEY, addMoney);
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.STATUS, ConstantsAnalyticsEventValue.COMMON);
        AnalyticsManager.Instance.SendAnalytics(analyticsData);

        OnClose();
    }    
    #endregion

    #region Private methods
    private void Initialize()
    {
        instance = GameManager.Instance;

        //Текущий СЧЕТ
        currScoreText.text = string.Format("{0}", instance.curScore);
        //Рекордный СЧЕТ
        bestScoreText.text = string.Format("{0}", instance.maxScore);

        //Кнопки
        SetVisibleAdsButton(true);

        //Конвертация текщего счета в деньги
        var convertMoney = instance.curScore / 100;
        convertMoney = Mathf.Max(1, convertMoney);
        instance.addMoney += convertMoney;

        //Отображение награды на кнопках
        btnAdsText.text = string.Format("{0}", instance.addMoney * 2);
        btnComText.text = string.Format("{0}", instance.addMoney);

        //Social-Achievements
        var social = SocialManager.Instance;
        social.SendIncrementAchievement(ConstantsAchievements.GAMER_1, instance.curScore);
        social.SendIncrementAchievement(ConstantsAchievements.GAMER_2, instance.curScore);
        social.SendIncrementAchievement(ConstantsAchievements.GAMER_3, instance.curScore);
        social.SendIncrementAchievement(ConstantsAchievements.GAMER_4, instance.curScore);
    }

    private void SetVisibleAdsButton(bool _value)
    {
        btnAdsActive.SetActive(_value);
        btnAdsDisable.SetActive(!_value);
    }

    private void OnReturnToMainMenuAds()
    {
        //Что бы не было межстраничной рекламы сразу после наградной
        instance.countReturnToMainMenu--;

        //Забор награды "Х2"        
        var addMoney = instance.addMoney * 2;

        instance.ChangeMoney(addMoney);
        instance.moneyToday += addMoney;

        //Analytics
        var analyticsData = new AnalyticsData();
        analyticsData.customEventName = ConstantsAnalyticsEventName.GAME_OVER;
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.SCORE, instance.curScore);
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.ADD_MONEY, addMoney);
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.STATUS, ConstantsAnalyticsEventValue.ADS);
        AnalyticsManager.Instance.SendAnalytics(analyticsData);

        OnClose();
    }

    private void OnUpdateAdsButton()
    {
        SetVisibleAdsButton(true);
    }

    private void OnClose()
    {
        var instance = GameManager.Instance;

        instance.startSpeed = instance.minSpeed;
        instance.lastStartSpeed = instance.minSpeed;

        instance.ShowUI(EnumUI.MainMenu);

        OnHide();
    }
    #endregion

    #region Coroutines
    #endregion

    #region IUiController
    #endregion
}
