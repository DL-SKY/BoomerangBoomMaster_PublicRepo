using DllSky.Patterns;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Variables  
    [Header("Main")]
    public int curMoney = 0;
    public int addMoney = 0;
    public int curScore = 0;
    public int maxScore = 0;
    public float curExp = 0.0f;
    public float startSpeed = 1.0f;
    public float lastStartSpeed = 0.0f;
    public float minSpeed = 1.0f;
    public float maxSpeed = 25.0f;

    [Space()]
    public EnumGameStatus currentStatus;

    [Header("Settings")]
    public bool settingSound;
    public string settingLanguage;

    [Header("Game Datas")]
    public MarketData marketData = new MarketData();
    public InventoryrData inventoryData = new InventoryrData();
    public MissionsData missionsData = new MissionsData();
    public BoostersData boostersData = new BoostersData();

    [Header("TEMP")]
    public int restoreHitPointsFromSession = 0;             //Восстановлено здоровья после Конца Игры за рекламу в "короткую сессию"
    public int countReturnToMainMenu = 0;                   //Счетчик возвратов в Главное Меню (после Игры) в "короткую сессию"
    public bool usingBoosterScore = false;                  //Флаг активности бустера на очки Score
    private int combo;
    public int xCombo                                       //Временная переменная комбо
    {
        get { return combo; }
        set { combo = Mathf.Clamp(value, 0, ConstantsSettings.maxCombo); }
    }

    [Header("Statistic")]    
    public int lastDayYearUpdateStatistic;                  //День года, когда последний раз проверялась статистика
    public DateTime dayFirstSessionUTC;
    public int moneyToday;
    public int adsToday;

    //[Header("Game")]
    //public float speed = 1.0f;
    //public BoomerangController boomerang;
    //public ObstaclesLayerController obstaclesLayer;

    [Header("UI")]
    public UiBase[] uis;
    public GameObject[] preloaders;

    [Header("Other Info")]
    public Transform gameboard;
    public BoomerangController boomerang;
    #endregion

    #region Events
    //Настройки
    public Action EventChangeLanguage;
    public Action EventUpdateLanguage;
    public Action EventChangeSettings;

    //Состояние приложения
    public Action EventStartGame;
    public Action EventGameOver;
    public Action EventShowMainMenu;

    //Бумеранг
    public Action EventChangeBoomerang;
    public Action EventUpdateBoomerang;

    //Игровой режим
    public Action EventRespawnBoomerang;
    public Action<float> EventSetSpeed;
    //public Action<float> EventChangeSpeed;
    public Action<int> EventChangeHP;
    public Action<int> EventChangeGameMoney;
    public Action<float> EventChangeTimeFreeze;
    public Action<bool> EventChangeUsingTimeFreeze;
    public Action<int> EventShowCombo;

    //Ресурсы
    public Action<int> EventSetScore;
    public Action<int> EventUpdateScore;
    public Action<float> EventSetEXP;
    public Action<float> EventUpdateEXP;
    //public Action<int> EventSetMoney;
    public Action<int> EventUpdateMoney;
    public Action<int> EventUpdateAddingMoney;
    public Action EventUpdateBoosters;
    #endregion

    #region Unity methods
    private void Start()
    {
        Initialize();
    }    

    private void OnEnable()
    {
        EventSetScore += SetScore;
        EventSetEXP += SetEXP;
        //EventSetMoney += SetMoney;
        EventChangeGameMoney += ChangeGameMoney;

        EventChangeBoomerang += OnChangeBoomerangHandle;
    }

    private void OnDisable()
    {
        EventSetScore -= SetScore;
        EventSetEXP -= SetEXP;
        //EventSetMoney -= SetMoney;
        EventChangeGameMoney -= ChangeGameMoney;

        EventChangeBoomerang -= OnChangeBoomerangHandle;
    }

    private void OnApplicationPause(bool _pause)
    {
        if (_pause)
        {
            SaveGame();
            NotificationManager.Instance.SendAllNotifications();
        }
        else
        {
            LoadGame();
            NotificationManager.Instance.ClearAllNotifications();
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
        NotificationManager.Instance.SendAllNotifications();
    }
    #endregion

    #region Public methods
    public void Initialize()
    {
        //Заглавный экран загрузки/прелоадер
        ShowPreloader(EnumPreloaders.Loading);

        LoadGame();
        NotificationManager.Instance.ClearAllNotifications();

        currentStatus = EnumGameStatus.StartApp;

        CreateBoomerang();

        //EventSetScore?.Invoke(curScore);
        //EventSetSpeed?.Invoke(minSpeed);

        ShowUI(EnumUI.MainMenu);
    }

    public void TestBtn()
    {
        EventChangeUsingTimeFreeze?.Invoke(true);
    }

    public void ShowUI(EnumUI _ui, object _data = null)
    {
        switch (_ui)
        {
            case EnumUI.MainMenu:
                StopAllCoroutines();
                StartCoroutine(ShowMainMenu());
                break;

            case EnumUI.GameMenu:
                StopAllCoroutines();
                StartCoroutine(ShowGameMenu());
                break;

            case EnumUI.Pause:
                uis[2].OnShow();
                break;

            case EnumUI.RespawnDialog:
                uis[3].OnShow();
                break;

            case EnumUI.GameOver:
                uis[4].OnShow();
                break;

            case EnumUI.Settings:
                uis[5].OnShow();
                break;

            case EnumUI.Market:
                //uis[6].OnShow();
                uis[6].OnShow(_data);
                break;

            case EnumUI.Inventory:
                uis[7].OnShow();
                break;

            case EnumUI.Missions:
                uis[8].OnShow();    //TODO
                break;

            case EnumUI.ClaimDialog:
                uis[9].OnShow(_data);
                break;

            case EnumUI.Credits:
                uis[10].OnShow(_data);
                break;
        }
    }

    public void ShowPreloader(EnumPreloaders _preloader)
    {
        switch (_preloader)
        {
            case EnumPreloaders.Loading:
                preloaders[0].SetActive(true);
                break;

            case EnumPreloaders.Common:
                preloaders[1].SetActive(true);
                break;
        }
    }

    public void HideAllPreloaders()
    {
        foreach (var obj in preloaders)
            obj.SetActive(false);
    }

    public void ChangeMoney(int _delta)
    {
        curMoney += _delta;
        curMoney = Mathf.Max(0, curMoney);

        EventUpdateMoney?.Invoke(curMoney);
    }

    public int GetDayInGame()
    {
        return (int)(DateTime.UtcNow - dayFirstSessionUTC).TotalDays;
    }

    public void ApplyPrizes(Dictionary<string, int> _dic)
    {
        foreach (var item in _dic)
        {
            //Монеты            
            if (item.Key == ConstantsResource.MONEY)
            {
                ChangeMoney(item.Value);
                continue;
            }

            //Бумеранги
            var boomerang = inventoryData.items.Find(x => x.id == item.Key);
            if (boomerang != null)
            {
                boomerang.isActive = true;
                continue;
            }

            //Бустеры
            var needContinue = false;
            foreach (var booster in Enum.GetNames(typeof(EnumBoosterType)))
            {
                if (item.Key == booster)
                {
                    needContinue = true;
                    var boosterType = Enum.Parse(typeof(EnumBoosterType), item.Key);

                    switch (boosterType)
                    {
                        case EnumBoosterType.HitPoints:
                            boostersData.boosterHP += item.Value;
                            break;

                        case EnumBoosterType.Freeze:
                            boostersData.boosterFreeze += item.Value;
                            break;

                        case EnumBoosterType.Score:
                            boostersData.boosterScore += item.Value;
                            break;
                    }

                    //Событие об изменении бустеров
                    EventUpdateBoosters?.Invoke();

                    break;
                }
            }

            if (needContinue)
                continue;

            //Другое
            var res = DefaultResources.list.Find(x => x.id == item.Key);
            if (res != null)
            {
                for (int i = 0; i < item.Value; i++)
                    ApplyPrizes(res.prizes);

                //Show dialog with prizes
                ShowUI(EnumUI.ClaimDialog, res.prizes);
            }
        }
    }
    #endregion

    #region Private methods
    private void SaveGame()
    {
        PlayerPrefs.DeleteAll();

        //Основные данные
        PlayerPrefs.SetInt(ConstantsPlayerPrefs.MONEY, curMoney);
        PlayerPrefs.SetFloat(ConstantsPlayerPrefs.SPEED, startSpeed);
        PlayerPrefs.SetInt(ConstantsPlayerPrefs.SCORE, maxScore);
        PlayerPrefs.SetFloat(ConstantsPlayerPrefs.EXP, curExp);

        //Настройки
        PlayerPrefs.SetInt(ConstantsPlayerPrefs.SETTING_SOUND, settingSound ? 1 : 0);
        PlayerPrefs.SetString(ConstantsPlayerPrefs.SETTING_LANG, settingLanguage);

        //Datas
        PlayerPrefs.SetString(ConstantsPlayerPrefs.MARKET, JsonUtility.ToJson(marketData));
        PlayerPrefs.SetString(ConstantsPlayerPrefs.INVENTORY, JsonUtility.ToJson(inventoryData));
        PlayerPrefs.SetString(ConstantsPlayerPrefs.MISSIONS, JsonUtility.ToJson(missionsData));
        PlayerPrefs.SetString(ConstantsPlayerPrefs.BOOSTERS, JsonUtility.ToJson(boostersData));

        //Статистика
        PlayerPrefs.SetString(ConstantsPlayerPrefs.DAY_FIRST_SESSION, dayFirstSessionUTC.ToString(System.Globalization.CultureInfo.InvariantCulture));
        PlayerPrefs.SetInt(ConstantsPlayerPrefs.LAST_UPDATE_STATISTIC, lastDayYearUpdateStatistic);
        PlayerPrefs.SetInt(ConstantsPlayerPrefs.MONEY_TODAY, moneyToday);
        PlayerPrefs.SetInt(ConstantsPlayerPrefs.ADS_TODAY, adsToday);

        PlayerPrefs.Save();
    }

    private void LoadGame()
    {        
        //Основные данные
        curMoney = PlayerPrefs.GetInt(ConstantsPlayerPrefs.MONEY, 0);

        startSpeed = PlayerPrefs.GetFloat(ConstantsPlayerPrefs.SPEED, minSpeed);

        maxScore = PlayerPrefs.GetInt(ConstantsPlayerPrefs.SCORE, 0);
        if (currentStatus != EnumGameStatus.Game)
            curScore = maxScore;

        curExp = PlayerPrefs.GetFloat(ConstantsPlayerPrefs.EXP, 0.0f);

        //Настройки
        settingSound = PlayerPrefs.GetInt(ConstantsPlayerPrefs.SETTING_SOUND, 1) >= 1;
        settingLanguage = PlayerPrefs.GetString(ConstantsPlayerPrefs.SETTING_LANG, "");
        if (string.IsNullOrEmpty(settingLanguage))
            settingLanguage = GetCurrentSystemLanguage();
        EventChangeLanguage?.Invoke();

        //Datas
        var jsonMarket = PlayerPrefs.GetString(ConstantsPlayerPrefs.MARKET, "");
        if (string.IsNullOrEmpty(jsonMarket))
        {
            marketData.items = DefaultMarketData.items.ToList();
        }
        else
        {
            marketData = JsonUtility.FromJson<MarketData>(jsonMarket);
            marketData.Validate();
        }

        var jsonInventory = PlayerPrefs.GetString(ConstantsPlayerPrefs.INVENTORY, "");
        if (string.IsNullOrEmpty(jsonInventory))
        {
            inventoryData.items = DefaultInventoryData.items.ToList();
        }
        else
        {
            inventoryData = JsonUtility.FromJson<InventoryrData>(jsonInventory);
            inventoryData.Validate();
        }

        var jsonMissions = PlayerPrefs.GetString(ConstantsPlayerPrefs.MISSIONS, "");
        if (string.IsNullOrEmpty(jsonMissions))
        {
            missionsData.items = DefaultMissionData.items.ToList();
        }
        else
        {
            missionsData = JsonUtility.FromJson<MissionsData>(jsonMissions);
            missionsData.Validate();
        }

        var jsonBoosters = PlayerPrefs.GetString(ConstantsPlayerPrefs.BOOSTERS, "");
        if (string.IsNullOrEmpty(jsonBoosters))
        {
            boostersData = BoostersData.GetDefault();
        }
        else
        {
            boostersData = JsonUtility.FromJson<BoostersData>(jsonBoosters);
        }

        //Статистика
        var date = PlayerPrefs.GetString(ConstantsPlayerPrefs.DAY_FIRST_SESSION, DateTime.UtcNow.ToString(System.Globalization.CultureInfo.InvariantCulture));
        dayFirstSessionUTC = DateTime.Parse(date, System.Globalization.CultureInfo.InvariantCulture);
        if (dayFirstSessionUTC.Year < 2000)
            dayFirstSessionUTC = DateTime.UtcNow;

        lastDayYearUpdateStatistic = PlayerPrefs.GetInt(ConstantsPlayerPrefs.LAST_UPDATE_STATISTIC, DateTime.UtcNow.DayOfYear);

        if (lastDayYearUpdateStatistic < DateTime.UtcNow.DayOfYear)
        {
            lastDayYearUpdateStatistic = DateTime.UtcNow.DayOfYear;
            moneyToday = 0;
            adsToday = 0;
        }
        else
        {
            moneyToday = PlayerPrefs.GetInt(ConstantsPlayerPrefs.MONEY_TODAY, 0);
            adsToday = PlayerPrefs.GetInt(ConstantsPlayerPrefs.ADS_TODAY, 0);
        }
    }

    private void OnChangeBoomerangHandle()
    {
        CreateBoomerang();
    }

    private void CreateBoomerang()
    {
        //Удаляем прежний бумеранг при необходимости
        if (boomerang)
        {
            Destroy(boomerang.gameObject);
            boomerang = null;
        }

        if (string.IsNullOrEmpty(inventoryData.activeBoomerang))
            inventoryData.activeBoomerang = DefaultInventoryData.IdStandardBoomerang;

        var data = inventoryData.items.Find(x => x.id == inventoryData.activeBoomerang);

        //Создание бумеранга
        var prefab = Resources.Load<GameObject>(string.Format(@"Prefabs/Skins/{0}/Boomerang", data.id));
        var obj = Instantiate(prefab, gameboard);
        boomerang = obj.GetComponent<BoomerangController>();
        boomerang.Initialize(data);

        //Events
        EventUpdateBoomerang?.Invoke();
        EventShowMainMenu?.Invoke();
    }

    private void SetScore(int _value)
    {
        //Debug.LogWarning("SetScore: " + currentStatus.ToString());

        curScore = _value;

        EventUpdateScore?.Invoke(curScore);

        SpeedControl();
    }

    private void SetEXP(float _value)
    {
        curExp = _value;

        EventUpdateEXP?.Invoke(curExp);
    }

    //private void SetMoney(int _value)
    //{
    //    curMoney = _value;

    //    EventUpdateMoney?.Invoke(curMoney);
    //}    

    private void ChangeGameMoney(int _delta)
    {
        addMoney += _delta;

        EventUpdateAddingMoney?.Invoke(addMoney);
    }

    public string GetCurrentSystemLanguage()
    {
        var lang = Application.systemLanguage;
        string result = "";

        switch (lang)
        {
            case SystemLanguage.Russian:
                result = ConstantsLanguage.RUSSIAN;
                break;
            case SystemLanguage.English:
                result = ConstantsLanguage.ENGLISH;
                break;
            case SystemLanguage.German:
                result = ConstantsLanguage.DEUTSCH;
                break;

            default:
                result = ConstantsLanguage.ENGLISH;
                break;
        }

        return result;
    }

    private void SpeedControl()
    {
        if (currentStatus != EnumGameStatus.Game)
            return;

        var T = Mathf.InverseLerp(0.0f, ConstantsSettings.maxScoreFromSpeed, curScore);

        lastStartSpeed = Mathf.Max(lastStartSpeed, minSpeed);

        T = Mathf.Clamp01(T);
        startSpeed = Mathf.Lerp(lastStartSpeed, maxSpeed, T);

        EventSetSpeed?.Invoke(startSpeed);
    }
    #endregion

    #region Coroutines
    private IEnumerator ShowMainMenu()
    {
        var audio = AudioManager.Instance;
        audio.StopAllMusic();
        audio.PlayMusic(@"Audio/Music/", @"MUS", _loop: true, _volume: 0.5f);

        switch (currentStatus)
        {
            case EnumGameStatus.Game:

                //Счетчик для рекламы
                var timer = 0.0f;
                countReturnToMainMenu++;
                if (countReturnToMainMenu >= ConstantsSettings.returnsToShowVideo)
                {
                    timer = 1.0f;
                    AdsManager.Instance.ShowVideo();
                }

                ShowPreloader(EnumPreloaders.Common);
                yield return null;                

                for (int i = 1; i < uis.Length; i++)
                    uis[i].OnHide();

                while (uis.Any(x => x.IsInit))
                    yield return null;

                //Пауза если реклама (доп время прелоадера для загрузки ролика)
                yield return new WaitForSeconds(timer);

                break;

            case EnumGameStatus.StartApp:

                //Заглавный экран загрузки/прелоадер
                ShowPreloader(EnumPreloaders.Loading);

                yield return null;
                SocialManager.Instance.Authenticate(null);
                yield return new WaitForSeconds(ConstantsSettings.mainPreloaderTimer);

                break;

            default:
                //Reserved
                break;
        }

        currentStatus = EnumGameStatus.MainMenu;

        //Максимальный рекорд
        maxScore = Mathf.Max(curScore, maxScore);
        SetScore(maxScore);

        //Рейтинг
        SocialManager.Instance.SendLeaderboardScore(maxScore);

        EventShowMainMenu?.Invoke();
        yield return null;

        uis[0].OnShow();        
    }

    private IEnumerator ShowGameMenu()
    {
        uis[0].OnHide();

        while (uis[0].IsInit)
            yield return null;

        lastStartSpeed = startSpeed;

        EventStartGame?.Invoke();

        curScore = 0;
        addMoney = 0;
        restoreHitPointsFromSession = 0;
        usingBoosterScore = false;
        xCombo = 0;

        uis[1].OnShow();
        currentStatus = EnumGameStatus.Game;

        var audio = AudioManager.Instance;
        audio.StopAllMusic();
        audio.PlayMusic(@"Audio/Music/", @"Curiosity", _loop: true, _volume: 0.5f);
    }
    #endregion

    #region Context menu
    [ContextMenu("Clear Save")]
    private void ClearSave()
    {
        PlayerPrefs.DeleteAll();
    }
    #endregion
}
