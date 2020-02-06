using UnityEngine;
using System.Collections;
using DllSky.Patterns;
using System.Collections.Generic;
using System;
using TMPro;
using DllSky.Extensions;

[Serializable]
public class LogData
{
    public string log;
    public string stack;
    public LogType type;
    public DateTime time;

    public LogData(string _logString, string _stackTrace, LogType _type)
    {
        log = _logString;
        stack = _stackTrace;
        type = _type;
        time = DateTime.Now;
    }
}

public class LogManager : Singleton<LogManager>
{
    #region Variables
    [Header("Settings")]
    public bool logging = true;

    [Header("Logs Dialod")]
    public GameObject dialog;
    public Transform logParent;
    public GameObject logPrefab;

    [Space()]
    [SerializeField]
    private List<LogData> logs = new List<LogData>();
    #endregion

    #region Unity methods
    private void OnEnable()
    {
        Application.logMessageReceived += OnLogHandler;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= OnLogHandler;
    }
    #endregion

    #region Public methods
    public void ShowDialog()
    {
        dialog.transform.SetAsLastSibling();
        dialog.SetActive(true);
        CreateItems();
    }

    public void CloseDialog()
    {
        dialog.transform.SetAsFirstSibling();
        dialog.SetActive(false);
    }

    public List<LogData> GetLogs()
    {
        return logs;
    }

    public void ClearLogs()
    {
        logs.Clear();
    }
    #endregion

    #region Private methods
    private void OnLogHandler(string _logString, string _stackTrace, LogType _type)
    {
        var newItem = new LogData(_logString, _stackTrace, _type);
        logs.Add(newItem);
    }

    private void ClearItems()
    {
        //var chields = logParent.childCount;
        //for (int i = chields - 1; i >= 0; i--)
        //    Destroy(logParent.GetChild(i).gameObject);
        logParent.DestroyChildren();
    }

    private void CreateItems()
    {
        ClearItems();

        foreach (var log in logs)
        {
            var newObj = Instantiate(logPrefab, logParent);
            var texts = newObj.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = log.log;
            texts[1].text = log.stack;
        }
    }
    #endregion
}
