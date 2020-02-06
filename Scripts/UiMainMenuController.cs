using DllSky.Extensions;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class UiMainMenuController : UiBase
{
    #region Variables
    [Header("Score")]
    public TextMeshProUGUI scoreText;

    [Header("GUI Animation")]
    public float timerHintStartGame = 3.0f;
    public GameObject hintHowStartGame;

    [Header("Spec Offers")]
    public Transform offersParent;
    public GameObject[] offersPrefabs;
    #endregion

    #region Unity methods
    private void OnEnable()
    {
        var instance = GameManager.Instance;

        instance.EventUpdateScore += UpdateScore;

        Initialize();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        var instance = GameManager.Instance;

        if (instance == null)
            return;

        instance.EventUpdateScore -= UpdateScore;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (transform.GetSiblingIndex() == transform.parent.childCount - 1)
            {
                //TODO: Show Exit Dialog
                //...
            }
        }
    }
    #endregion

    #region Public methods
    public void OnClickSettings()
    {
        GameManager.Instance.ShowUI(EnumUI.Settings);
    }

    public void OnClickPlay()
    {
        GameManager.Instance.ShowUI(EnumUI.GameMenu);
    }

    public void OnClickMarket()
    {
        GameManager.Instance.ShowUI(EnumUI.Market);
    }

    public void OnClickBoomerangsInventory()
    {
        GameManager.Instance.ShowUI(EnumUI.Inventory);
    }

    public void OnClickQuests()
    {
        GameManager.Instance.ShowUI(EnumUI.Missions);
    }    

    public void OnClickAchievements()
    {
        SocialManager.Instance.ShowAchievements();
    }

    public void OnClickLeaders()
    {
        SocialManager.Instance.ShowLeaderboard();
    }
    #endregion

    #region Private methods
    private void Initialize()
    {
        var instance = GameManager.Instance;

        StopAllCoroutines();

        UpdateScore(instance.curScore);
        hintHowStartGame.SetActive(false);

        //TODO: временно отключил
        //Проверки спец-предложений
        //CheckSpecialOffers();

        //TODO
        //...

        //Прячем прелоадер и запускаем таймер анимации "Нажмите чтобы играть"
        instance.HideAllPreloaders();
        StartCoroutine(TimerHintHowStartGame());

        //Analytics
        var analyticsData = new AnalyticsData();
        analyticsData.customEventName = ConstantsAnalyticsEventName.PROFILE;

        var levels = ConstantsSettings.levels.ToList();
        int levelIndex = levels.FindLastIndex(x => x >= instance.curExp);

        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.MAX_SCORE, instance.maxScore);
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.EXP, instance.curExp);
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.LVL, levelIndex + 1);
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.MONEY, instance.curMoney);
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.DAYS_IN_GAME, instance.GetDayInGame());

        AnalyticsManager.Instance.SendAnalytics(analyticsData);
    }

    private void UpdateScore(int _value)
    {
        scoreText.text = string.Format("{0}", _value);
    }

    private void CheckSpecialOffers()
    {
        //Удаляем офферы
        offersParent.DestroyChildren();

        var instance = GameManager.Instance;
        var dayInGame = instance.GetDayInGame();
        var prefIndex = -1;

        //Предложения 1го дня D1
        if ((dayInGame == 1 && !instance.marketData.isEndOfferD1) || instance.marketData.isStartedOfferD1)
            prefIndex = 0;
        //Предложения 3го дня D3
        else if ((dayInGame == 3 && !instance.marketData.isEndOfferD3) || instance.marketData.isStartedOfferD3)
            prefIndex = 1;
        //Предложения 7го дня D7
        else if ((dayInGame == 7 && !instance.marketData.isEndOfferD7) || instance.marketData.isStartedOfferD7)
            prefIndex = 2;

        //Инстанцируем префаб
        if (prefIndex >= 0)
            Instantiate(offersPrefabs[prefIndex], offersParent);
    }
    #endregion

    #region Coroutines
    private IEnumerator TimerHintHowStartGame()
    {
        yield return new WaitForSeconds(timerHintStartGame);

        hintHowStartGame.SetActive(true);
    }
    #endregion

    #region IUiController    
    #endregion
}
