using DllSky.Components;
using DllSky.Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UiGameController : UiBase
{
    #region Variables
    [Header("Hit Points")]
    public Transform hitPointsPanel;
    public GameObject prefHP;
    public List<GameObject> hps;

    [Header("Score")]
    public TextMeshProUGUI scoreText;

    [Header("Adding Money")]
    public TextMeshProUGUI addMoneyText;

    [Header("Time Freeze")]
    public ProgressBar timeFreezeProgressBar;

    [Header("Markers")]
    public bool isShowedMarker;
    public GameObject pauseBtnMarker;

    [Header("Combo")]
    public GameObject comboPanel;
    public TextMeshProUGUI comboText;

    [Header("Boosters")]
    public GameObject boosterTime;
    public GameObject boosterHitPoints;
    public GameObject boosterScore;
    #endregion

    #region Unity methods
    private void OnEnable()
    {
        var instance = GameManager.Instance;

        instance.EventGameOver += ApplyGameOver;

        instance.EventChangeHP += ChangeHP;
        instance.EventChangeTimeFreeze += ChangeTimeFreeze;
        instance.EventUpdateScore += UpdateScore;
        instance.EventUpdateAddingMoney += UpdateAddMoney;
        instance.EventShowCombo += ShowComboHeader;

        Initialize();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        var instance = GameManager.Instance;

        if (instance == null)
            return;

        instance.EventGameOver -= ApplyGameOver;

        instance.EventChangeHP -= ChangeHP;
        instance.EventChangeTimeFreeze -= ChangeTimeFreeze;
        instance.EventUpdateScore -= UpdateScore;
        instance.EventUpdateAddingMoney -= UpdateAddMoney;
        instance.EventShowCombo -= ShowComboHeader;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (transform.GetSiblingIndex() == transform.parent.childCount - 1)
                OnClickPause();
        }
    }
    #endregion

    #region Public methods
    public void OnClickActiveatedTimeFreeze(bool _value)
    {
        GameManager.Instance.EventChangeUsingTimeFreeze?.Invoke(_value);
    }

    public void OnClickPause()
    {
        isShowedMarker = true;
        GameManager.Instance.ShowUI(EnumUI.Pause);

        UpdatePauseButtonMarker();
    }

    public void OnClickTestReturnToMenu()
    {
        GameManager.Instance.ShowUI(EnumUI.MainMenu);
    }
    #endregion

    #region Private methods
    private void Initialize()
    {
        var instance = GameManager.Instance;
        
        //Начало игры...

        //Счет
        UpdateScore(instance.curScore);
        //Доп.деньги
        UpdateAddMoney(instance.addMoney);
        //Здоровье
        ClearHitPointsPanel();
        CreateHitPoints();
        UpdateHitPoints();
        //Замедление времени
        UpdateTimeFreeze();

        //Комбо
        comboPanel.SetActive(false);

        //Маркер
        isShowedMarker = false;
        UpdatePauseButtonMarker();
    }

    private void ClearHitPointsPanel()
    {
        //var chields = hitPointsPanel.childCount;
        //for (int i = chields - 1; i >= 0; i--)
        //    Destroy(hitPointsPanel.GetChild(i).gameObject);
        hitPointsPanel.DestroyChildren();
    }

    private void CreateHitPoints()
    {
        hps.Clear();

        var maxHP = GameManager.Instance.boomerang.GetMaxHitPoints();
        for (int i = 0; i < maxHP; i++)
        {
            var newHP = Instantiate(prefHP, hitPointsPanel);
            hps.Add(newHP);
        }
    }    

    private void UpdateHitPoints()
    {
        for (int i = 0; i < hps.Count; i++)
            hps[i].SetActive(i < GameManager.Instance.boomerang.hp);
    }

    private void UpdateTimeFreeze()
    {
        var progress = GameManager.Instance.boomerang.GetTimeFreezeNormalize();
        timeFreezeProgressBar.FillAmount = progress;
    }

    private void UpdatePauseButtonMarker()
    {
        //Если маркер уже просмотрен - не отображаем
        if (isShowedMarker)
        {
            pauseBtnMarker.SetActive(false);
            return;
        }

        //Иначе - просчитываем необходимость отображения
        var isMarker = GameManager.Instance.boostersData.GetBoostersMarker();
        pauseBtnMarker.SetActive(isMarker);
    }

    private void ApplyGameOver()
    {
        var instance = GameManager.Instance;
        if (instance.restoreHitPointsFromSession < ConstantsSettings.maxRestoreHitPointsAds)
        {
            instance.ShowUI(EnumUI.RespawnDialog);
        }
        else
        {
            instance.ShowUI(EnumUI.GameOver);
        }
    }

    private void ChangeHP(int _delta)
    {
        UpdateHitPoints();
    }

    private void ChangeTimeFreeze(float _delta)
    {
        UpdateTimeFreeze();
    }

    private void UpdateScore(int _value)
    {
        scoreText.text = string.Format("{0}", _value);
    }

    private void UpdateAddMoney(int _value)
    {
        addMoneyText.text = string.Format("{0}", _value);
    }

    private void ShowComboHeader(int _xCombo)
    {
        if (_xCombo < 2)
            return;

        comboText.text = string.Format("x{0}", _xCombo);
        comboPanel.SetActive(true);
    }
    #endregion

    #region Coroutines
    #endregion

    #region IUiController
    #endregion
}
