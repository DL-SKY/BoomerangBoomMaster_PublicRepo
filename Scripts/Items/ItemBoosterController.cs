using UnityEngine;
using System.Collections;
using TMPro;

public class ItemBoosterController : MonoBehaviour
{
    #region Variables
    [Header("Settings")]
    public bool autoinitializing = false;
    public EnumBoosterType boosterType;
    [SerializeField]
    private int counter;

    [Space()]
    public TextMeshProUGUI counterText;
    public GameObject marker;
    public GameObject[] objectsActive;
    public GameObject[] objectsDisable;

    [Header("Settings")]
    public float animationTime = 0.5f;

    [SerializeField]
    private int visBoosterCount;
    [SerializeField]
    private int curBoosterCount;

    private GameManager instance;
    #endregion

    #region Unity methods
    private void OnEnable()
    {
        instance = GameManager.Instance;
        instance.EventUpdateBoosters += OnUpdateBoostersHandler;

        if (autoinitializing)
            Initialize();
    }

    private void OnDisable()
    {
        if (instance == null)
            return;

        instance.EventUpdateBoosters -= OnUpdateBoostersHandler;
    }
    #endregion

    #region Public methods
    public void Initialize()
    {
        //Кол-во
        if (instance == null)
            instance = GameManager.Instance;

        visBoosterCount = curBoosterCount = counter = instance.boostersData.GetBoosterCount(boosterType);       
        ShowBoosterText(counter);        
    }

    public void OnClickActive()
    {
        //Особая проверка для бустера на очки Score
        if (boosterType == EnumBoosterType.Score)
        {
            //Если Бустер уже используется
            if (instance.usingBoosterScore)
            {
                //TODO: Show alert
                //...

                return;
            }
        }

        //Меняем кол-во бустеров
        instance.boostersData.DecrementBoosterCount(boosterType);

        //Применяем бустер
        switch (boosterType)
        {
            case EnumBoosterType.HitPoints:
                instance.EventChangeHP?.Invoke(instance.boomerang.GetMaxHitPoints());
                break;

            case EnumBoosterType.Freeze:
                instance.EventChangeTimeFreeze?.Invoke(instance.boomerang.GetMaxTimeFreeze());
                break;

            case EnumBoosterType.Score:
                instance.usingBoosterScore = true;
                break;
        }

        //Analytics
        var analyticsData = new AnalyticsData();
        analyticsData.customEventName = ConstantsAnalyticsEventName.BOOSTER;
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.ID, boosterType.ToString());
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.AMOUNT, instance.boostersData.GetBoosterCount(boosterType));
        AnalyticsManager.Instance.SendAnalytics(analyticsData);

        //Сообщение... Or VFX
        //TODO: show using booster info

        //Событие об использовании бустера
        instance.EventUpdateBoosters?.Invoke();
    }

    public void OnClickDisable()
    {
        var itemID = "";

        switch (boosterType)
        {
            case EnumBoosterType.HitPoints:
                itemID = DefaultMarketData.IdBoosterHitPoints;
                break;

            case EnumBoosterType.Freeze:
                itemID = DefaultMarketData.IdBoosterTimeFreeze;
                break;

            case EnumBoosterType.Score:
                itemID = DefaultMarketData.IdBoosterScore;
                break;
        }

        instance.ShowUI(EnumUI.Market, itemID);
    }
    #endregion

    #region Private methods
    private void OnUpdateBoostersHandler()
    {
        counter = instance.boostersData.GetBoosterCount(boosterType);

        StopAllCoroutines();

        StartCoroutine(AnimateCountText(counter));
    }

    private void ShowBoosterText(int _value)
    {
        counterText.text = string.Format("{0}", _value);

        //Активность элементов
        var isActive = _value > 0;
        foreach (var obj in objectsActive)
            obj.SetActive(isActive);
        foreach (var obj in objectsDisable)
            obj.SetActive(!isActive);

        //Маркер
        marker?.SetActive(isActive);
    }
    #endregion

    #region Coroutines
    private IEnumerator AnimateCountText(int _value)
    {
        //Если дельта не больше 1, то применяем изменения моментально
        if (Mathf.Abs(visBoosterCount - _value) <= 1)
        {
            visBoosterCount = curBoosterCount = _value;
            ShowBoosterText(visBoosterCount);

            yield break;
        }

        curBoosterCount = _value;
        var startMoney = visBoosterCount;

        var T = 0.0f;
        var time = 0.0f;

        while (T < 1.0f)
        {
            T = Mathf.InverseLerp(0.0f, animationTime, time);
            visBoosterCount = (int)Mathf.Lerp(startMoney, curBoosterCount, T);

            ShowBoosterText(visBoosterCount);

            yield return null;

            time += Time.unscaledDeltaTime;
        }

        visBoosterCount = curBoosterCount;
        ShowBoosterText(visBoosterCount);
    }
    #endregion
}
