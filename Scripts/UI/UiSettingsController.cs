using UnityEngine;
using System.Collections;
using TMPro;

public class UiSettingsController : UiBase
{
    #region Variables
    public TextMeshProUGUI version;
    #endregion

    #region Unity methods
    private void OnEnable()
    {
        var instance = GameManager.Instance;
        GameManager.Instance.EventUpdateLanguage += HandlerOnApplyLanguage;

        Initialize();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        var instance = GameManager.Instance;

        if (instance == null)
            return;

        GameManager.Instance.EventUpdateLanguage -= HandlerOnApplyLanguage;
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
        ApplyLocalization();

        //SocialManager.Instance.SendIncrementAchievement(ConstantsAchievements.TEST, 50);
        SocialManager.Instance.SendProgressAchievement(ConstantsAchievements.TEST);
    }

    private void HandlerOnApplyLanguage()
    {
        ApplyLocalization();
    }

    private void ApplyLocalization()
    {
        version.text = string.Format("{0} {1}", LocalizationManager.Instance.GetString("version"), Application.version);
    }
    #endregion

    #region IUiController
    #endregion
}
