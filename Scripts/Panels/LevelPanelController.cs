using DllSky.Components;
using System.Linq;
using TMPro;
using UnityEngine;

public class LevelPanelController : MonoBehaviour
{
    #region Variables
    [Header("Current")]
    public TextMeshProUGUI currLevel;

    [Header("Progress")]
    public ProgressBar levelProgressBar;
    public TextMeshProUGUI levelProgressValue;

    [Header("UI Settings")]
    public Animator animator;

    private int lastLevel;
    #endregion

    #region Unity methods
    private void Start()
    {
        lastLevel = 0;

        UpdateLevel(GameManager.Instance.curExp);
    }

    private void OnEnable()
    {
        lastLevel = 0;

        var instance = GameManager.Instance;

        instance.EventUpdateEXP += UpdateLevel;
    }

    private void OnDisable()
    {
        var instance = GameManager.Instance;

        if (instance == null)
            return;

        instance.EventUpdateEXP -= UpdateLevel;
    }
    #endregion

    #region Private methods
    private void UpdateLevel(float _value)
    {
        //Текущий уровень
        var levels = ConstantsSettings.levels.ToList();
        int levelIndex = levels.FindLastIndex(x => x >= _value);

        currLevel.text = string.Format("{0}", levelIndex + 1);

        //Текущее числовое значение прогресса
        var curEXP = _value;
        var maxEXP = levelIndex >= levels.Count - 1 ? levels.Last() : levels[levelIndex + 1];

        levelProgressValue.text = string.Format("{0}/{1}", (int)curEXP, (int)maxEXP);

        //Нормализованное значение прогресса относительно уровня
        var nextLevelDelta = maxEXP - levels[levelIndex];
        var currExpDelta = curEXP - levels[levelIndex];

        levelProgressBar.FillAmount = Mathf.InverseLerp(0.0f, nextLevelDelta, currExpDelta);

        //(Проверка) Получение Нового Уровня
        if (lastLevel > 0)
        {
            //Если Новый Уровень...
            if (lastLevel < levelIndex + 1)
            {
                var instance = GameManager.Instance;

                //...анимируем и восстанавливаем очки здоровья
                animator.Play("NewLevel");
                instance.EventChangeHP?.Invoke(instance.boomerang.GetMaxHitPoints());
            }
        }
        lastLevel = levelIndex + 1;
    }
    #endregion
}
