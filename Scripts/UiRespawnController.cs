using UnityEngine;
using System.Collections;
using TMPro;

public class UiRespawnController : UiBase
{
    #region Variables
    [Header("Timer")]
    public TextMeshProUGUI timerText;

    [Header("Buttons")]
    public TextMeshProUGUI btnPriceToRespawnText;
    public GameObject btnAdsActive;
    public GameObject btnAdsDisable;
    #endregion

    #region Unity methods
    private void OnEnable()
    {
        Initialize();
    }

    private void Update()
    {
        //if (Input.GetKeyUp(KeyCode.Escape))
        //{
        //    OnClickCancel();
        //}
    }
    #endregion

    #region Public methods
    public void OnClickRespawnFromAds()
    {
        //Проверка на кол-во уже просмотренной рекламы
        if (GameManager.Instance.adsToday >= ConstantsSettings.maxAds)
            return;

        //Останавливаем таймер
        StopTimer();

        SetVisibleAdsButton(false);

        //Просмотр рекламы
        AdsManager.Instance.ShowRewardedVideo(OnRespawn, OnUpdateAdsButton);
    }

    public void OnClickRespawnFromMoney()
    {
        var instance = GameManager.Instance;
        
        //Проверка на деньги
        if (instance.curMoney < ConstantsSettings.priceToRestore)
            return;

        //Останавливаем таймер
        StopTimer();

        //Восстановление
        instance.ChangeMoney(-ConstantsSettings.priceToRestore);
        OnRespawn();
    }

    public void OnClickCancel()
    {
        GameManager.Instance.ShowUI(EnumUI.GameOver);
        OnHide();
    }
    #endregion

    #region Private methods
    private void Initialize()
    {
        //Buttons
        SetVisibleAdsButton(true);

        btnPriceToRespawnText.text = string.Format("{0}", ConstantsSettings.priceToRestore);

        StopAllCoroutines();

        StartCoroutine(Timer());
    }

    private void SetVisibleAdsButton(bool _value)
    {
        btnAdsActive.SetActive(_value);
        btnAdsDisable.SetActive(!_value);
    }

    private void StopTimer()
    {
        StopAllCoroutines();
        timerText.text = string.Format("{0:F0}", 0.000f);
    }

    private void OnRespawn()
    {
        var instance = GameManager.Instance;

        instance.EventRespawnBoomerang?.Invoke();
        instance.restoreHitPointsFromSession++;

        OnHide();
    }

    private void OnUpdateAdsButton()
    {
        SetVisibleAdsButton(true);
    }
    #endregion

    #region Coroutines
    private IEnumerator Timer()
    {
        var time = ConstantsSettings.timerRestoreHitPointsAdsDialog;

        while (time >= 0.0f)
        {
            time = Mathf.Max(0.0f, time);
            timerText.text = string.Format("{0:F0}", time);

            yield return null;

            time -= Time.deltaTime;
        }

        //Завершение игры после таймера
        OnClickCancel();
    }
    #endregion

    #region IUiController
    #endregion
}
