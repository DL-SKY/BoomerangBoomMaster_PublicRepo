using UnityEngine;
using System.Collections;

public class UiCreditsController : UiBase
{
    #region Variables
    public GameObject animationPanel;
    public float pause = 0.75f;
    #endregion

    #region Unity methods
    private void OnEnable()
    {
        Initialize();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        animationPanel.SetActive(false);
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
        animationPanel.SetActive(false);

        StartCoroutine(Animate());
    }
    #endregion

    #region Coroutines
    private IEnumerator Animate()
    {
        yield return new WaitForSeconds(pause);

        animationPanel.SetActive(true);
    }
    #endregion

    #region IUiController
    #endregion
}
