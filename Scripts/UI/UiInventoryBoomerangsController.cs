using DllSky.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UiInventoryBoomerangsController : UiBase
{
    #region Variables
    [Header("Boomerangs")]
    public GameObject prefItem;
    public Transform parentPanel;
    public List<ItemInventoryController> items = new List<ItemInventoryController>();
    public ScrollRect scroller;
    #endregion

    #region Unity methods
    private void OnEnable()
    {
        var instance = GameManager.Instance;

        Initialize();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        var instance = GameManager.Instance;

        if (instance == null)
            return;
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
    #endregion

    #region Private methods
    private void Initialize()
    {
        CreateItems();
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

        var list = GameManager.Instance.inventoryData.items.OrderBy(x => x.price).ToList();
        foreach (var item in list)
        {
            //Если бумеранг Скрытый и не активен/не куплен, то не обрабатываем
            if (item.isHide && !item.isActive)
                continue;

            var newObj = Instantiate(prefItem, parentPanel);
            var newItem = newObj.GetComponent<ItemInventoryController>();
            newItem.Initialize(item);

            items.Add(newItem);
        }
    }
    #endregion

    #region IUiController
    #endregion
}
