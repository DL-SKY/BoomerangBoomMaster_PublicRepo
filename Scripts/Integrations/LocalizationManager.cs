using UnityEngine;
using System.Collections;
using DllSky.Patterns;
using System;
using System.Collections.Generic;

[Serializable]
public class LocalizationData
{
    public List<LocalizationItem> strings = new List<LocalizationItem>();
}

[Serializable]
public class LocalizationItem
{
    public string id;

    public string rus;
    public string eng;
    public string deu;
}

public class LocalizationManager : Singleton<LocalizationManager>
{
    #region Variables
    [SerializeField]
    private LocalizationData data;

    [SerializeField]
    private string language = ConstantsLanguage.ENGLISH;

    private Dictionary<string, string> localization = new Dictionary<string, string>();
    #endregion

    #region Unity methods
    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        var instance = GameManager.Instance;

        instance.EventChangeLanguage += OnChangeLanguageHandler;
    }

    private void OnDisable()
    {
        var instance = GameManager.Instance;

        if (instance == null)
            return;

        instance.EventChangeLanguage -= OnChangeLanguageHandler;
    }
    #endregion

    #region Public methods
    public string GetString(string _key)
    {
        if (!localization.ContainsKey(_key) || string.IsNullOrEmpty(localization?[_key]))
            return _key;

        return localization[_key];
    }
    #endregion

    #region Private methods
    private void Initialize()
    {
        LoadLocalization();
    }

    private void OnChangeLanguageHandler()
    {
        ApplyLocalization();
    }

    private void ApplyLocalization()
    {
        localization.Clear();        

        language = GameManager.Instance.settingLanguage;
        for (int i = 0; i < data.strings.Count; i++)
        {
            string key = data.strings[i].id;
            string value = "";

            switch (language)
            {
                case ConstantsLanguage.RUSSIAN:
                    value = data.strings[i].rus;
                    break;
                case ConstantsLanguage.ENGLISH:
                    value = data.strings[i].eng;
                    break;
                case ConstantsLanguage.DEUTSCH:
                    value = data.strings[i].deu;
                    break;

                default:
                    value = data.strings[i].eng;
                    break;
            }

            value = value.Replace("\\n", Environment.NewLine);
            localization.Add(key, value);
        }

        GameManager.Instance.EventUpdateLanguage?.Invoke();
    }

    private void LoadLocalization()
    {
        string json = Resources.Load<TextAsset>(@"Localization/Localizations").text;
        //Debug.LogWarning(json);

        data = JsonUtility.FromJson<LocalizationData>(json);
    }
    #endregion

    #region Context Menu
    [ContextMenu("To JSON")]
    private void ToJson()
    {
        Debug.LogWarning(JsonUtility.ToJson(data, true));
    }
    #endregion
}
