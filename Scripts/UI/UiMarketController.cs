using DllSky.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiMarketController : UiBase
{
    #region Variables
    [Header("Items")]
    public GameObject prefItem;
    public Transform parentPanel;
    public List<ItemMarketController> items = new List<ItemMarketController>();
    public ScrollRect scroller;

    [Header("Bonuses")]
    public TMP_InputField input;
    #endregion

    #region Unity methods
    private void OnEnable()
    {
        //var instance = GameManager.Instance;

        Initialize();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        //var instance = GameManager.Instance;

        //if (instance == null)
        //    return;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (transform.GetSiblingIndex() == transform.parent.childCount - 1)
                OnClickClose();
        }
    }
    #endregion

    #region Public methods
    public void OnClickClose()
    {
        OnHide();
    }

    public void CodeTextEditEnded(string _value)
    {
        var instance = GameManager.Instance;

        //Если уже использовался - пропуск
        if (instance.marketData.bonuses.Contains(_value))
        {
            input.text = "";
            return;
        }

        //Поиск по списку "промо-кодов"
        var bonusData = Codes.list.Find(x => x.id == _value);
        if (bonusData != null)
        {
            //Выдаем призы
            instance.ApplyPrizes(bonusData.prizes);

            //Show dialog with prizes
            GameManager.Instance.ShowUI(EnumUI.ClaimDialog, bonusData.prizes);

            //Сохраняем используемый код
            instance.marketData.bonuses.Add(bonusData.id);

            //Analytics
            var analyticsData = new AnalyticsData();
            analyticsData.customEventName = ConstantsAnalyticsEventName.BONUS_CODE;
            analyticsData.eventData.Add(ConstantsAnalyticsEventParam.ID, bonusData.id);
            AnalyticsManager.Instance.SendAnalytics(analyticsData);

            //Social-Achievements
            var social = SocialManager.Instance;
            social.SendProgressAchievement(ConstantsAchievements.CHEATER);
            if (bonusData.id == "PIKABU")
                social.SendProgressAchievement(ConstantsAchievements.PIKABU);
        }

        input.text = "";
    }
    #endregion

    #region Private methods
    private void Initialize()
    {
        CreateItems();

        //Скроллим до нужной точки
        StopAllCoroutines();
        if (objData != null)
            StartCoroutine(ScrollToItem());
    }

    private void ClearItems()
    {
        //var chields = parentPanel.childCount;
        //for (int i = chields - 1; i >= 0; i--)
        //    Destroy(parentPanel.GetChild(i).gameObject);
        parentPanel.DestroyChildren();
    }

    private void CreateItems()
    {
        ClearItems();
        items.Clear();

        scroller.content.anchoredPosition = Vector2.zero;

        var list = GameManager.Instance.marketData.items.OrderBy(x => x.sortIndex).ToList();
        foreach (var item in list)
        {
            var newObj = Instantiate(prefItem, parentPanel);
            var newItem = newObj.GetComponent<ItemMarketController>();
            newItem.Initialize(item);

            items.Add(newItem);
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator ScrollToItem()
    {
        yield return null;          //Кадр для отрисовки

        var itemID = (string)objData;

        if (string.IsNullOrEmpty(itemID))
            yield break;
        
        var item = items.Find(x => x.GetData().id == itemID);
        if (item != null)
        {
            var itemPos = Mathf.Abs(item.GetComponent<RectTransform>().anchoredPosition.y);
            scroller.content.anchoredPosition = new Vector2(scroller.content.anchoredPosition.x, itemPos);
        }
    }
    #endregion

    #region IUiController    
    #endregion
}
