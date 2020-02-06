using UnityEngine;
using System.Collections;
using TMPro;
using System.Linq;

public class SettingsPanelController : MonoBehaviour
{
    #region Variables
    [Header("Sound/Music")]
    public GameObject[] btnsSound;

    [Header("Language")]
    public GameObject[] btnsLang;

    private GameManager instance;
    #endregion

    #region Unity methods
    private void OnEnable()
    {
        instance = GameManager.Instance;

        Initialize();
    }
    #endregion

    #region Public methods
    public void Initialize()
    {
        UpdateUI();
    }

    public void OnClickSound()
    {
        instance.settingSound = !instance.settingSound;

        UpdateSoundButtons();

        instance.EventChangeSettings?.Invoke();
    }

    public void OnClickLanguage(string _lang)
    {
        instance.settingLanguage = _lang;

        UpdateLanguageButtons();

        instance.EventChangeLanguage?.Invoke();
    }

    public void OnClickPlaymarket()
    {
        SocialManager.Instance.OpenStore();
    }

    public void OnClickCredits()
    {
        instance.ShowUI(EnumUI.Credits);
    }
    #endregion

    #region Private methods
    private void UpdateUI()
    {
        UpdateSoundButtons();
        UpdateLanguageButtons();
    }

    private void UpdateSoundButtons()
    {
        btnsSound[0].SetActive(!instance.settingSound);             //Откл
        btnsSound[1].SetActive(instance.settingSound);              //Вкл
    }

    private void UpdateLanguageButtons()
    {
        var langIndex = btnsLang.ToList().FindIndex(x => x.name == instance.settingLanguage);
        langIndex = Mathf.Max(0, langIndex);

        //Поочередно перебираем кнопки с флагами
        for (int i = 0; i < btnsLang.Length; i++)
            btnsLang[i].SetActive(i == langIndex);
    }
    #endregion
}
