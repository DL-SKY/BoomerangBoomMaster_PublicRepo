using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.UI;
using DllSky.Extensions;

public class UiMissionsController : UiBase
{
    #region Variables
    [Header("Missions")]
    public GameObject prefItem;
    public Transform parentPanel;
    public List<ItemMissionController> items = new List<ItemMissionController>();
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

        foreach (var item in GameManager.Instance.missionsData.items)
        {
            var newObj = Instantiate(prefItem, parentPanel);
            var newItem = newObj.GetComponent<ItemMissionController>();
            newItem.Initialize(item);

            items.Add(newItem);
        }
    }
    #endregion

    #region IUiController
    #endregion
}
