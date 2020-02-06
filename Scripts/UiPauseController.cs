using UnityEngine;
using System.Collections;

public class UiPauseController : UiBase
{
    #region Variables
    [Header("Settings")]
    public float timeOfStop = 0.25f;

    [Header("Boosters")]
    public ItemBoosterController[] boosters;
    #endregion

    #region Unity methods
    private void OnEnable()
    {
        Initialize();
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

    public void OnClickToMainMenu()
    {
        Time.timeScale = 1.0f;

        var instance = GameManager.Instance;

        instance.EventGameOver?.Invoke();

        //Применяем Очки Score и Монеты...
        //Монеты
        var addMoney = instance.addMoney;
        instance.ChangeMoney(addMoney);
        instance.moneyToday += addMoney;
        
        //Score применится автоматически при открытии Главного Меню
        instance.ShowUI(EnumUI.MainMenu);

        //Social-Achievements
        var social = SocialManager.Instance;
        social.SendIncrementAchievement(ConstantsAchievements.GAMER_1, instance.curScore);
        social.SendIncrementAchievement(ConstantsAchievements.GAMER_2, instance.curScore);
        social.SendIncrementAchievement(ConstantsAchievements.GAMER_3, instance.curScore);
        social.SendIncrementAchievement(ConstantsAchievements.GAMER_4, instance.curScore);

        OnHide();
    }
    #endregion

    #region Private methods
    private void Initialize()
    {
        //Обновляем кнопки бустеров
        UpdateBoosters();
    }

    private void UpdateBoosters()
    {
        foreach (var booster in boosters)
            booster.Initialize();
    }
    #endregion

    #region Coroutines
    private IEnumerator PauseActivate()
    {
        var t = 0.0f;
        var T = 0.0f;

        while (T < 1.0f)
        {
            T = Mathf.InverseLerp(0.0f, timeOfStop, t);

            Time.timeScale = Mathf.Lerp(1.0f, 0.0f, T);

            yield return null;

            t += Time.unscaledDeltaTime;
        }

        IsInit = true;
    }

    private IEnumerator PauseDisable()
    {
        var t = 0.0f;
        var T = 0.0f;

        while (T < 1.0f)
        {
            T = Mathf.InverseLerp(0.0f, timeOfStop, t);

            Time.timeScale = Mathf.Lerp(0.0f, 1.0f, T);

            yield return null;

            t += Time.unscaledDeltaTime;
        }

        yield return null;

        gameObject.SetActive(false);
    }
    #endregion

    #region IUiController
    override public void OnShow()
    {
        if (IsShowed)
            return;

        Debug.LogWarning("OnShow");

        gameObject.SetActive(true);

        IsShowed = true;
        transform.SetAsLastSibling();

        //...
        animator.Play("Show");

        StopAllCoroutines();
        StartCoroutine(PauseActivate());
    }

    override public void OnHide()
    {
        if (!IsShowed)
            return;        

        IsShowed = false;

        //...
        animator.Play("Hide");

        StopAllCoroutines();

        if (Time.timeScale < 1.0f)
            StartCoroutine(PauseDisable());
        else
            gameObject.SetActive(false);
    }

    override public void OnDisableObject()
    {

    }
    #endregion
}
