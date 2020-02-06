using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class ItemInventoryController : MonoBehaviour
{
    #region Variables
    [Header("Main")]
    public Image image;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI freezeText;

    [Header("Buttons")]
    public GameObject currBoomerang;
    public GameObject btnSelect;
    public GameObject btnBuy;
    public GameObject btnWait;
    public GameObject imgMoney;
    public GameObject imgAds;
    public TextMeshProUGUI priceText;

    private InventoryItemData data;
    private static ItemInventoryController current;
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
    public void Initialize(InventoryItemData _data)
    {
        data = _data;

        UpdateStatus();
    }

    public void OnClickSelect()
    {
        GameManager.Instance.inventoryData.SetActiveBoomerang(data.id);
        current.UpdateStatus();
        UpdateStatus();

        //Analytics
        var analyticsData = new AnalyticsData();
        analyticsData.customEventName = ConstantsAnalyticsEventName.INVENTORY;
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.ACTION, ConstantsAnalyticsEventValue.SELECT);
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.ID, data.id);
        AnalyticsManager.Instance.SendAnalytics(analyticsData);

        //TODO: VFX покупки/приобретения
    }

    public void OnClickBuy()
    {
        var instance = GameManager.Instance;

        //Проверка ресурса
        switch (data.resource)
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
                    OnClaimBoomerang();
                }
                break;

            case ConstantsResource.ADS:
                SetVisibleAdsAndIapButton(false);
                AdsManager.Instance.ShowRewardedVideo(OnClaimBoomerang, OnUpdateAdsButton);
                break;

            case ConstantsResource.IAP:
                //SetVisibleAdsAndIapButton(false);
                IapManager.Instance.BuyProductID(data.id, OnClaimBoomerang, OnUpdateAdsButton);
                break;
        }
    }
    #endregion

    #region Private methods
    private void UpdateStatus()
    {
        //Бумеранг:
        image.sprite = Resources.Load<Sprite>(string.Format(@"Sprites/Skins/{0}/inventory_image", data.id));
        hpText.text = string.Format("{0}", data.hp);
        freezeText.text = string.Format("{0:F1}", data.freeze);

        //Кнопки:
        if (data.isActive)
        {
            var isCurrent = data.id == GameManager.Instance.inventoryData.activeBoomerang;
            if (isCurrent)
                current = this;

            currBoomerang.SetActive(isCurrent);
            btnSelect.SetActive(!isCurrent);

            btnBuy.SetActive(false);
            btnWait.SetActive(false);
        }
        else
        {
            currBoomerang.SetActive(false);
            btnSelect.SetActive(false);                       

            imgAds.SetActive(data.resource == ConstantsResource.ADS);
            imgMoney.SetActive(data.resource == ConstantsResource.MONEY);

            switch (data.resource)
            {
                case ConstantsResource.MONEY:
                    SetVisibleAdsAndIapButton(true);
                    priceText.text = string.Format("{0}", data.price);
                    break;

                case ConstantsResource.ADS:
                    priceText.text = "";
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
    }

    private void UpdateIapPrice()
    {
        if (data.resource != ConstantsResource.IAP)
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

    private void OnClaimBoomerang()
    {
        UpdateData();

        data.isActive = true;
        UpdateStatus();

        //Выдаем призы
        var dic = new System.Collections.Generic.Dictionary<string, int>() { { data.id, 1 } };
        GameManager.Instance.ShowUI(EnumUI.ClaimDialog, dic);

        //Analytics
        var analyticsData = new AnalyticsData();
        analyticsData.customEventName = ConstantsAnalyticsEventName.INVENTORY;
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.ACTION, ConstantsAnalyticsEventValue.BUY);
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.ID, data.id);
        analyticsData.eventData.Add(ConstantsAnalyticsEventParam.RESOURCE, data.resource);
        AnalyticsManager.Instance.SendAnalytics(analyticsData);

        //TODO: VFX покупки/приобретения
    }

    private void UpdateData()
    {
        data = GameManager.Instance.inventoryData.items.Find(x => x.id == data.id);
    }
    #endregion

    #region Coroutines
    #endregion
}
