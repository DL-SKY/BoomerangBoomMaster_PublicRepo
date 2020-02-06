using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using DllSky.Extensions;

public class UiClaimDialogController : UiBase
{
    #region Variables
    [Header("Items")]
    public GameObject prefItem;
    public Transform parentPanel;

    private Dictionary<string, int> items = new Dictionary<string, int>();
    #endregion

    #region Unity methods
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (transform.GetSiblingIndex() == transform.parent.childCount - 1)
                OnClickClose();
        }
    }

    private void OnEnable()
    {
        Initialize((Dictionary<string, int>)objData);
    }

    override protected void OnDisable()
    {
        base.OnDisable();
    }
    #endregion

    #region Public methods
    public void OnClickClose()
    {
        OnHide();
    }
    #endregion

    #region Private methods
    public void Initialize(Dictionary<string, int> _dic)
    {
        items = _dic;

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

        if (items == null)
            return;

        foreach (var item in items)
        {
            var newObj = Instantiate(prefItem, parentPanel);
            var newItem = newObj.GetComponent<ItemClaimController>();

            newItem.imgResource.sprite = Resources.Load<Sprite>(string.Format(@"Sprites/ResourcesImg/{0}", item.Key));
            newItem.amountResource.text = string.Format("x{0}", item.Value);
            newItem.nameResource.text = LocalizationManager.Instance.GetString(string.Format("{0}", item.Key));
        }
    }
    #endregion

    #region IUiController
    #endregion
}