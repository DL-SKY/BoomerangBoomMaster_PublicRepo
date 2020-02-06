using UnityEngine;
using System.Collections;
using TMPro;
using System;

public class SpecOfferPanel : MonoBehaviour
{
    #region Variables
    public string id = "";

    [Header("Animation")]
    public Animator animator;

    [Header("Small")]
    public GameObject smallOffer;
    public CanvasGroup smallAlpha;

    [Header("Big")]
    public GameObject bigOffer;
    public CanvasGroup bigAlpha;

    [Header("Timer")]
    public TextMeshProUGUI[] timerTexts;

    [Header("Buttons")]
    public GameObject btnBuy;
    public GameObject btnWait;
    public TextMeshProUGUI priceText;

    private MarketItemData data;
    private DateTime start;
    #endregion

    #region Unity methods
    private void Awake()
    {
        data = DefaultMarketData.offers.Find(x => x.id == id);

        if (data == null)
        {
            Destroy(gameObject);
            return;
        }

        Check();
    }

    private void Start()
    {
        if (IapManager.Instance.IsInitialized())
            UpdateIapPrice();
        else
            SetVisibleIapButton(false);
    }

    private void OnEnable()
    {
        IapManager.Instance.EventInitialized += UpdateIapPrice;

        InvokeRepeating(((Action)OnTimer).Method.Name, 0.0f, 1.0f);
    }

    private void OnDisable()
    {
        if (IapManager.Instance == null)
            return;

        IapManager.Instance.EventInitialized -= UpdateIapPrice;

        CancelInvoke();
    }
    #endregion

    #region Public methods
    public void OnClickShowBigOffer()
    {
        animator?.Play("Show");
    }

    public void OnClickCloseBigOffer()
    {
        animator?.Play("Hide");
    }

    public void OnClickBuy()
    {
        //SetVisibleIapButton(false);
        IapManager.Instance.BuyProductID(data.id, OnApplyBuy, OnUpdateButton);
    }
    #endregion

    #region Private methods
    private void Check()
    {
        var instance = GameManager.Instance;

        switch (id)
        {
            case DefaultMarketData.IdOfferD1:
                if (!instance.marketData.isStartedOfferD1)
                {
                    instance.marketData.isStartedOfferD1 = true;
                    instance.marketData.dateStartedOfferD1 = DateTime.UtcNow;
                }
                //Проверка по сроку давности
                start = instance.marketData.dateStartedOfferD1;
                var spanD1 = start.AddHours(24) - DateTime.UtcNow;
                if (spanD1.TotalSeconds <= 0)
                {
                    //Меняем флаги и удаляем объект
                    instance.marketData.isStartedOfferD1 = false;
                    instance.marketData.isEndOfferD1 = true;
                    Destroy(gameObject);
                }           
                break;

            case DefaultMarketData.IdOfferD3:
                if (!instance.marketData.isStartedOfferD3)
                {
                    instance.marketData.isStartedOfferD3 = true;
                    instance.marketData.dateStartedOfferD3 = DateTime.UtcNow;
                }
                //Проверка по сроку давности
                start = instance.marketData.dateStartedOfferD3;
                var spanD3 = start.AddHours(24) - DateTime.UtcNow;
                if (spanD3.TotalSeconds <= 0)
                {
                    //Меняем флаги и удаляем объект
                    instance.marketData.isStartedOfferD3 = false;
                    instance.marketData.isEndOfferD3 = true;
                    Destroy(gameObject);
                }
                break;

            case DefaultMarketData.IdOfferD7:
                if (!instance.marketData.isStartedOfferD7)
                {
                    instance.marketData.isStartedOfferD7 = true;
                    instance.marketData.dateStartedOfferD7 = DateTime.UtcNow;
                }
                //Проверка по сроку давности
                start = instance.marketData.dateStartedOfferD7;
                var spanD7 = start.AddHours(24) - DateTime.UtcNow;
                if (spanD7.TotalSeconds <= 0)
                {
                    //Меняем флаги и удаляем объект
                    instance.marketData.isStartedOfferD7 = false;
                    instance.marketData.isEndOfferD7 = true;
                    Destroy(gameObject);
                }
                break;
        }        
    }

    private void OnTimer()
    {
        var span = start.AddHours(24) - DateTime.UtcNow;

        //Проверка на завершение
        if (span.TotalSeconds <= 0)
        {
            Check();
            return;
        }

        foreach (var timer in timerTexts)
            timer.text = string.Format(@"{0:00}:{1:00}:{2:00}", (int)span.TotalHours, span.Minutes, span.Seconds);
    }

    private void UpdateIapPrice()
    {
        SetVisibleIapButton(true);
        priceText.text = string.Format("{0}", IapManager.Instance.GetPrice(data.id));
    }

    private void SetVisibleIapButton(bool _value)
    {
        btnBuy.SetActive(_value);
        btnWait.SetActive(!_value);
    }

    private void OnUpdateButton()
    {
        SetVisibleIapButton(true);
    }

    private void OnApplyBuy()
    {
        SetVisibleIapButton(true);

        //Выдаем призы
        var dic1 = new System.Collections.Generic.Dictionary<string, int>() { { data.resourceItem, data.amount } };
        GameManager.Instance.ApplyPrizes(dic1);

        //Show dialog with prizes
        var dic2 = DefaultResources.list.Find(x => x.id == data.resourceItem);
        GameManager.Instance.ShowUI(EnumUI.ClaimDialog, dic2.prizes);

        //Analytics
        var analyticsData = new AnalyticsData();
        analyticsData.customEventName = ConstantsAnalyticsEventName.MARKET;
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.ACTION, ConstantsAnalyticsEventValue.BUY);
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.ID, data.id);
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.RESOURCE, data.resourcePrice);
        AnalyticsManager.Instance.SendAnalytics(analyticsData);

        //Social-Achievements
        var social = SocialManager.Instance;
        social.SendProgressAchievement(ConstantsAchievements.SPONSOR);

        //TODO: VFX покупки/приобретения
    }
    #endregion
}
