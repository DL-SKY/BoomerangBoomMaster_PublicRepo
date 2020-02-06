using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.Monetization;

public class ItemMarketController : MonoBehaviour
{
    #region Variables
    [Header("Main")]
    public Transform imgParent;

    [Header("Buttons")]
    public GameObject btnBuy;
    public GameObject btnWait;
    public GameObject imgMoney;
    public GameObject imgAds;
    public TextMeshProUGUI priceText;

    private MarketItemData data;
    #endregion

    #region Unity methods
    private void OnEnable()
    {
        IapManager.Instance.EventInitialized += UpdateIapPrice;
    }

    private void OnDisable()
    {
        if (IapManager.Instance == null)
            return;

        IapManager.Instance.EventInitialized -= UpdateIapPrice;
    }
    #endregion

    #region Public methods
    public void Initialize(MarketItemData _data)
    {
        data = _data;

        UpdateStatus();
    }

    public void OnClickBuy()
    {
        var instance = GameManager.Instance;

        //Проверка ресурса
        switch (data.resourcePrice)
        {
            case ConstantsResource.MONEY:
                if (instance.curMoney < data.price)
                {
                    //TODO: Alert
                }
                else
                {
                    //Покупаем
                    instance.ChangeMoney(-data.price);
                    OnApplyBuy();
                }
                break;

            case ConstantsResource.ADS:
                SetVisibleAdsAndIapButton(false);
                AdsManager.Instance.ShowRewardedVideo(OnApplyBuy, OnUpdateAdsButton);
                break;

            case ConstantsResource.IAP:
                //SetVisibleAdsAndIapButton(false);
                IapManager.Instance.BuyProductID(data.id, OnApplyBuy, OnUpdateAdsButton);
                break;
        }
    }

    public MarketItemData GetData()
    {
        return data;
    }
    #endregion

    #region Private methods
    private void UpdateStatus()
    {
        //Товар:        
        var prefab = Resources.Load<GameObject>(string.Format(@"Prefabs/Market/{0}", data.id));
        Instantiate(prefab, imgParent);

        //Кнопки:
        imgAds.SetActive(data.resourcePrice == ConstantsResource.ADS);
        imgMoney.SetActive(data.resourcePrice == ConstantsResource.MONEY);
        if (data.resourcePrice == ConstantsResource.IAP)
            priceText.transform.localPosition = Vector3.zero;

        switch (data.resourcePrice)
        {
            case ConstantsResource.MONEY:
                SetVisibleAdsAndIapButton(true);
                priceText.text = string.Format("{0}", data.price);
                break;

            case ConstantsResource.ADS:
                SetVisibleAdsAndIapButton(true);
                break;

            case ConstantsResource.IAP:
                if (IapManager.Instance.IsInitialized())
                    UpdateIapPrice();
                else
                    SetVisibleAdsAndIapButton(false);                    
                break;
        }
    }

    private void UpdateIapPrice()
    {
        if (data.resourcePrice != ConstantsResource.IAP)
            return;

        SetVisibleAdsAndIapButton(true);
        priceText.text = string.Format("{0}", IapManager.Instance.GetPrice(data.id));
    }

    private void SetVisibleAdsAndIapButton(bool _value)
    {
        btnBuy.SetActive(_value);
        btnWait.SetActive(!_value);
    }

    private void OnUpdateAdsButton()
    {
        SetVisibleAdsAndIapButton(true);
    }

    private void OnApplyBuy()
    {
        SetVisibleAdsAndIapButton(true);

        //Выдаем призы
        var dic = new System.Collections.Generic.Dictionary<string, int>() { { data.resourceItem, data.amount } };
        GameManager.Instance.ApplyPrizes(dic);

        //Show dialog with prizes
        GameManager.Instance.ShowUI(EnumUI.ClaimDialog, dic);

        //Analytics
        var analyticsData = new AnalyticsData();
        analyticsData.customEventName = ConstantsAnalyticsEventName.MARKET;
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.ACTION, ConstantsAnalyticsEventValue.BUY);
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.ID, data.id);
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.RESOURCE, data.resourcePrice);
        AnalyticsManager.Instance.SendAnalytics(analyticsData);

        //Social-Achievements
        if (data.resourcePrice == ConstantsResource.IAP)
        {
            var social = SocialManager.Instance;
            social.SendProgressAchievement(ConstantsAchievements.SPONSOR);
        }

        //TODO: VFX покупки/приобретения
    }
    #endregion

    #region Coroutines
    #endregion
}
