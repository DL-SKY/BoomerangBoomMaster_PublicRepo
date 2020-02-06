using TMPro;
using UnityEngine;

[AddComponentMenu("UI/Localization/LocalizationTextMeshPro")]
[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizationTextMeshPro : MonoBehaviour
{
    #region Variables
    public string key = "";

    private TextMeshProUGUI text;
    private string localizationString = "";
    #endregion

    #region Unity methods
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        GameManager.Instance.EventUpdateLanguage += HandlerOnApplyLanguage;

        ApplyLocalization();
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null)
            return;

        GameManager.Instance.EventUpdateLanguage -= HandlerOnApplyLanguage;
    }
    #endregion

    #region Private methods
    private void HandlerOnApplyLanguage()
    {
        ApplyLocalization();
    }

    private void ApplyLocalization()
    {
        if (!text)
            return;

        localizationString = LocalizationManager.Instance.GetString(key);
        text.text = localizationString.ToString();
    }
    #endregion
}
