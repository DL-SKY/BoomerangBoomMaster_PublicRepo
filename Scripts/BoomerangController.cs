using System;
using UnityEngine;

public class BoomerangController : MonoBehaviour
{
    #region Variables
    [Header("Debug")]
    public bool infiniteFreeze = false;
    public bool infiniteHP = false;

    [Header("Main")]
    public int hp = 3;
    private int maxHP = 3;
    public float timeFreeze = 3.0f;
    private float maxTimeFreeze = 3.0f;             //запас в секундах
    public bool usingTimeFreeze = false;
    public float restoreTimeFreeze = 0.25f;         //скорость восстановления в секунду
    public float pauseAfterFreeze = 0.25f;          //задержка перед началов восстановления
    public float timerRestore;

    [Header("Settings trajectory")]
    public float speed = 1.5f;                      //10.0
    private float lastSpeed = 1.5f;                 //

    public float A = 0.0f;                          //
    public float B = 1.0f;                          //40.0
    public float C = 1.0f;                          //0.1
    public float D = 0.0f;                          // 

    [Header("Rotation script")]
    public AutoRotation rotationScript;
    public float minRotation = -250.0f;
    public float maxRotation = -720.0f;
    public GameObject[] particlesRotation;

    [Header("Trail")]
    public TrailRenderer trail;

    private float startY;
    #endregion

    #region Unity methods
    private void Awake()
    {
        maxHP = hp;
        maxTimeFreeze = timeFreeze;

        timerRestore = pauseAfterFreeze;

        startY = transform.localPosition.y;
    }

    private void Start()
    {
        //Ссылка
        GameManager.Instance.boomerang = this;
    }

    private void OnEnable()
    {
        var instance = GameManager.Instance;

        instance.EventSetSpeed += SetSpeed;
        instance.EventChangeHP += ChangeHP;
        instance.EventChangeTimeFreeze += ChangeTimeFreeze;
        instance.EventChangeUsingTimeFreeze += ChangeUsingTimeFreeze;

        instance.EventShowMainMenu += ApplyDefault;
        instance.EventStartGame += ApplyStartGame;
        instance.EventGameOver += ApplyGameOver;
        instance.EventRespawnBoomerang += ApplyRespawn;
    }

    private void OnDisable()
    {
        var instance = GameManager.Instance;

        if (instance == null)
            return;

        instance.EventSetSpeed -= SetSpeed;
        //instance.EventChangeSpeed -= ChangeSpeed;
        instance.EventChangeHP -= ChangeHP;
        instance.EventChangeTimeFreeze -= ChangeTimeFreeze;
        instance.EventChangeUsingTimeFreeze -= ChangeUsingTimeFreeze;

        instance.EventShowMainMenu -= ApplyDefault;
        instance.EventStartGame -= ApplyStartGame;
        instance.EventGameOver -= ApplyGameOver;
        instance.EventRespawnBoomerang -= ApplyRespawn;
    }

    private void Update()
    {
        //Тратим заморозку времени
        if (usingTimeFreeze)
        {
            timerRestore = pauseAfterFreeze;

            GameManager.Instance.EventChangeTimeFreeze?.Invoke(-1.0f * Time.deltaTime);
        }
        //Восстанавливаем
        else
        {
            timerRestore -= Time.deltaTime;
            timerRestore = Mathf.Max(timerRestore, 0.0f);

            if (timerRestore <= 0.0f && timeFreeze < maxTimeFreeze)
            {
                timerRestore = 0.0f;
                GameManager.Instance.EventChangeTimeFreeze?.Invoke(restoreTimeFreeze * Time.deltaTime);
            }
        }
    }

    private void FixedUpdate()
    {
        var y = transform.localPosition.y + speed * Time.fixedDeltaTime;
        var x = A + B * (float)Math.Sin(C * y + D);

        //TODO: проверка на максимум
        if (y >= float.MaxValue - speed)
        {            
            y = 0.0f;
            trail.Clear();
        }

        transform.localPosition = new Vector3(x, y, 0.0f);
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        //Debug.LogWarning("Speed: " + speed + " / " + _collision.tag);
        var audio = AudioManager.Instance;
        var instance = GameManager.Instance;

        var otherTag = _collision.tag;

        switch (otherTag)
        {
            case ConstantsTag.TAG_OBSTACLE:                
                audio.PlaySound(@"Audio/Sound/", @"Hit");
                instance.xCombo = 0;
                instance.EventChangeHP?.Invoke(-1);
                break;

            case ConstantsTag.TAG_TIME:               
                audio.PlaySound(@"Audio/Sound/", @"Bonus");
                instance.xCombo++;
                instance.EventChangeTimeFreeze?.Invoke(2.5f * instance.xCombo);
                instance.EventShowCombo?.Invoke(instance.xCombo);
                break;

            case ConstantsTag.TAG_HP:
                audio.PlaySound(@"Audio/Sound/", @"Bonus");
                instance.xCombo++;
                instance.EventChangeHP?.Invoke(1 * instance.xCombo);
                instance.EventShowCombo?.Invoke(instance.xCombo);
                break;

            case ConstantsTag.TAG_MONEY:
                audio.PlaySound(@"Audio/Sound/", @"Money");
                instance.xCombo++;

                var additional = 1 * instance.xCombo;
                instance.EventChangeGameMoney?.Invoke(additional);
                instance.EventShowCombo?.Invoke(instance.xCombo);

                //Social-Achievements
                var social = SocialManager.Instance;
                social.SendIncrementAchievement(ConstantsAchievements.MONEY_1, additional);
                social.SendIncrementAchievement(ConstantsAchievements.MONEY_2, additional);
                social.SendIncrementAchievement(ConstantsAchievements.MONEY_3, additional);
                break;
        }
    }
    #endregion

    #region Public methods
    public void Initialize(InventoryItemData _data)
    {
        hp = maxHP = _data.hp;
        timeFreeze = maxTimeFreeze = _data.freeze;
    }

    public int GetMaxHitPoints()
    {
        return maxHP;
    }

    public float GetMaxTimeFreeze()
    {
        return maxTimeFreeze;
    }

    public float GetTimeFreezeNormalize()
    {
        return Mathf.InverseLerp(0.0f, maxTimeFreeze, timeFreeze);
    }
    #endregion

    #region Private methods
    private void ApplyDefault()
    {
        //Скорость
        var instance = GameManager.Instance;
        instance.EventSetSpeed?.Invoke(instance.minSpeed);

        //Позиция
        var x = A + B * (float)Math.Sin(C * startY + D);
        transform.localPosition = new Vector3(x, startY, 0.0f);

        trail.Clear();

        usingTimeFreeze = false;
        timerRestore = pauseAfterFreeze;
    }

    private void ApplyStartGame()
    {
        //Скорость
        var instance = GameManager.Instance;
        instance.EventSetSpeed?.Invoke(instance.startSpeed);

        //Позиция
        //...

        hp = maxHP;
        timeFreeze = maxTimeFreeze;
        usingTimeFreeze = false;
        timerRestore = pauseAfterFreeze;        
    }

    private void ApplyGameOver()
    {
        speed = lastSpeed = 0.0f;
    }

    private void ApplyRespawn()
    {
        GameManager.Instance.EventChangeHP?.Invoke(maxHP);
        speed = lastSpeed = GameManager.Instance.startSpeed;
    }

    private void SetSpeed(float _value)
    {
        if (usingTimeFreeze)
            return;

        speed = _value;
        lastSpeed = speed;

        UpdateRotationSpeed();
    }

    private void ChangeHP(int _delta)
    {
        hp += _delta;
        hp = Mathf.Clamp(hp, 0, maxHP);

        if (infiniteHP)
            hp = maxHP;

        CheckGameOver();
    }

    private void CheckGameOver()
    {
        if (hp <= 0)
            GameManager.Instance.EventGameOver?.Invoke();
    }

    private void ChangeTimeFreeze(float _delta)
    {
        if (infiniteFreeze)
            return;

        timeFreeze += _delta;
        timeFreeze = Mathf.Clamp(timeFreeze, 0.0f, maxTimeFreeze);

        if (timeFreeze <= 0.0f && usingTimeFreeze)
            GameManager.Instance.EventChangeUsingTimeFreeze?.Invoke(false);
    }

    private void ChangeUsingTimeFreeze(bool _value)
    {
        if (usingTimeFreeze == _value == true)
            _value = false;

        usingTimeFreeze = timeFreeze > 0.0f ? _value : false;

        //Замораживаем бумеранг
        if (usingTimeFreeze)
        {
            lastSpeed = speed;
            speed = 0.0f;

            //TODO: др. эффекты
            //Эффект голубого сияния, свечения
            //...
            //Снижение скорости вращения
            UpdateRotationSpeed();
            //Убираем частицы вращения
            foreach (var ps in particlesRotation)
                ps.SetActive(false);
        }
        //Размораживаем бумеранг
        else
        {
            speed = Mathf.Max(lastSpeed, GameManager.Instance.startSpeed);

            //TODO: отключить другие эффекты заморозки
            //...
            //Восстановление вращения
            UpdateRotationSpeed();
            //Отображаем частицы вращения
            foreach (var ps in particlesRotation)
                ps.SetActive(true);
        }
    }

    private void UpdateRotationSpeed()
    {
        var instance = GameManager.Instance;
        var t = Mathf.InverseLerp(instance.minSpeed, instance.maxSpeed, lastSpeed);
        var mod = usingTimeFreeze ? 0.01f : 1.0f;

        var rotationSpeed = Mathf.Lerp(minRotation, maxRotation, t) * mod;
        rotationScript.speed = rotationSpeed;
    }
    #endregion

    #region Coroutines
    #endregion
}
