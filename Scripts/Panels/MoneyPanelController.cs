using UnityEngine;
using System.Collections;
using TMPro;

public class MoneyPanelController : MonoBehaviour
{
    #region Variables
    [Header("Money")]
    public TextMeshProUGUI moneyText;

    [Header("Settings")]
    public float animationTime = 0.5f;

    [SerializeField]
    private int visMoney;
    [SerializeField]
    private int curMoney;

    private GameManager instance;
    #endregion

    #region Unity methods
    private void OnEnable()
    {
        instance = GameManager.Instance;

        instance.EventUpdateMoney += OnUpdateMoneyHeader;

        Initialize();
    }

    private void OnDisable()
    {
        if (instance == null)
            return;

        instance.EventUpdateMoney -= OnUpdateMoneyHeader;
    }
    #endregion

    #region Public methods
    #endregion

    #region Private methods
    private void Initialize()
    {
        StopAllCoroutines();

        //StartCoroutine(AnimateMoneyText(instance.curMoney));
        visMoney = curMoney = instance.curMoney;
        ShowMoneyText(visMoney);
    }

    private void OnUpdateMoneyHeader(int _value)
    {
        StopAllCoroutines();

        StartCoroutine(AnimateMoneyText(_value));
    }

    private void ShowMoneyText(int _value)
    {
        moneyText.text = string.Format("{0}", _value);
    }
    #endregion

    #region Coroutine
    private IEnumerator AnimateMoneyText(int _value)
    {
        curMoney = _value;
        var startMoney = visMoney;

        var T = 0.0f;
        var time = 0.0f;

        while (T < 1.0f)
        {
            T = Mathf.InverseLerp(0.0f, animationTime, time);
            visMoney = (int)Mathf.Lerp(startMoney, curMoney, T);

            ShowMoneyText(visMoney);

            yield return null;

            time += Time.unscaledDeltaTime;
        }

        visMoney = curMoney;
        ShowMoneyText(visMoney);
    }
    #endregion
}
