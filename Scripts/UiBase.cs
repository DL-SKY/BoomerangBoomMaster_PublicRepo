using UnityEngine;

public class UiBase : MonoBehaviour, IUiController
{
    #region Variables
    [Header("UI Settings")]
    public Animator animator;

    protected object objData = null;
    #endregion

    #region Unity methods
    virtual protected void OnDisable()
    {
        IsInit = false;

        transform.SetAsFirstSibling();
    }
    #endregion

    #region IUiController
    public bool IsShowed { get; set; }
    public bool IsInit { get; set; }

    virtual public void OnShow()
    {
        if (IsShowed)
            return;

        gameObject.SetActive(true);

        IsShowed = true;
        transform.SetAsLastSibling();

        //...

        animator.Play("Show");

        IsInit = true;
    }

    virtual public void OnShow(object _data)
    {
        objData = _data;

        OnShow();
    }

    virtual public void OnHide()
    {
        if (!IsShowed)
            return;

        IsShowed = false;

        //...

        animator.Play("Hide");
    }

    virtual public void OnDisableObject()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
