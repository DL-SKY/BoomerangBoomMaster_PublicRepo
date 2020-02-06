using DllSky.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesLayerController : MonoBehaviour
{
    #region Variables
    [Header("Spawners")]
    public Transform[] borderPoints;
    [Range(0.0f, 0.5f)]
    public float borderNormalizeOffset = 0.1f;

    [Header("Settings")]
    public float timerSpawn = 2.5f;
    public Vector2 offsetSpawn = new Vector2(-1.5f, 1.5f);

    [Header("Boomerang")]
    public Transform boomerang;

    [Header("Obstacles")]
    public bool isLastSpecObstacle = false;                     //Флаг того, что последний обстакл - был "специальным"(призом)
    public List<int> lastBorderIndexes = new List<int>();       //Индексы предыдущего отрезка, на котором был сгенерирован объект
    public EnumObstacles lastObstacle = EnumObstacles.NA;       //Последний обстакл (во избежание двух подряд "горизонтальных" препятствий)

    public GameObject[] obstaclesSmall;
    public GameObject[] obstaclesHorizontal;

    public GameObject obstacleMoney;
    public GameObject obstacleTime;
    public GameObject obstacleHP;

    //Для тестов, задать значения аналогичные Бумерангу
    [Header("[DEBUG] Settings trajectory")]
    public float A = 0.0f;
    public float B = 1.0f;
    public float C = 1.0f;
    public float D = 0.0f;
    #endregion

    #region Unity methods
    private void OnEnable()
    {
        var instance = GameManager.Instance;

        instance.EventUpdateBoomerang += OnUpdateBoomerangHandle;
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

        instance.EventUpdateBoomerang -= OnUpdateBoomerangHandle;
        instance.EventShowMainMenu -= ApplyDefault;
        instance.EventStartGame -= ApplyStartGame;
        instance.EventGameOver -= ApplyGameOver;
        instance.EventRespawnBoomerang -= ApplyRespawn;
    }

    private void Update()
    {
        //Перемещение родительского объекта-контроллера вслед за Бумерангом
        if (boomerang)
            transform.position = new Vector3(transform.position.x, boomerang.position.y, transform.position.z);
    }

    private void OnDrawGizmos()
    {
        //Точки
        for (int i = 0; i < borderPoints.Length; i++)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(borderPoints[i].position, 0.15f);
        }

        //Отрезки
        var tmpBool = true;
        for (int i = 0; i < borderPoints.Length - 1; i++)
        {
            Gizmos.color = tmpBool ? Color.green : Color.yellow;
            Gizmos.DrawLine(borderPoints?[i]?.position ?? Vector3.zero, borderPoints?[i+1]?.position ?? Vector3.zero);

            tmpBool = !tmpBool;
        }

        //Траектория
        Gizmos.color = Color.cyan;
        var T = 0.0f;
        for (int i = 0; i < 150; i++)
        {
            var y = transform.localPosition.y + T;
            var x = A + B * (float)System.Math.Sin(C * y + D);

            Gizmos.DrawSphere(new Vector3(x, y, 0.0f), 0.1f);

            T += 0.1f;
        }
    }
    #endregion

    #region Public methods
    #endregion

    #region Private methods
    private void OnUpdateBoomerangHandle()
    {
        boomerang = GameManager.Instance.boomerang.transform;
    }

    private void ApplyDefault()
    {
        StopAllCoroutines();
    }

    private void ApplyStartGame()
    {
        isLastSpecObstacle = false;
        lastBorderIndexes.Clear();

        StartCoroutine(SpawnerCoroutine());
    }

    private void ApplyGameOver()
    {
        StopAllCoroutines();
    }

    private void ApplyRespawn()
    {
        StartCoroutine(SpawnerCoroutine());
    }

    private float GetNextTimerSpawn()
    {
        var result = 0.0f;

        result = timerSpawn / GameManager.Instance.startSpeed;

        return result;
    }

    private void Spawn()
    {
        //Кол-во за раз
        lastBorderIndexes.Clear();
        var count = GetSpawnerCount();

        for (int i = 0; i < count; i++)
        {
            //Тип
            var obstacleType = GetObstacleType(count);
            lastObstacle = obstacleType;

            GameObject usingPref = obstaclesSmall.GetRandom();
            switch (obstacleType)
            {
                case EnumObstacles.Small:
                    usingPref = obstaclesSmall.GetRandom();
                    break;

                case EnumObstacles.Horizontal:
                    usingPref = obstaclesHorizontal.GetRandom();
                    break;

                //case EnumObstacles.Vertical:
                //    usingPref = obstacleVertical;
                //    break;

                case EnumObstacles.Time:
                    usingPref = obstacleTime;
                    break;

                case EnumObstacles.Money:
                    usingPref = obstacleMoney;
                    break;

                case EnumObstacles.HP:
                    usingPref = obstacleHP;
                    break;
            }
            
            Instantiate(usingPref, GetObstaclePosition(), usingPref.transform.rotation, transform);
        }
    }

    private int GetSpawnerCount()
    {
        var counts = new int[] { 2, 3, 1, 2, 3, 2, 3, 3 };

        return counts.GetRandom();
    }

    private EnumObstacles GetObstacleType(int _count)
    {
        //TODO:
        if (_count == 1)
        {
            return EnumObstacles.Horizontal;
        }
        else if (_count == 2)
        {
            //Шанс генерации особых объектов
            var specObstacle = GenerateSpecObstacle();
            if (specObstacle != EnumObstacles.NA)
                return specObstacle;

            //Обычная генерация
            var types = new EnumObstacles[] { EnumObstacles.Small, lastObstacle == EnumObstacles.Horizontal ? EnumObstacles.Horizontal : EnumObstacles.Small, EnumObstacles.Small, };
            return types.GetRandom();
        }
        else    //(_count > 2)
        {
            //Шанс генерации особых объектов
            var specObstacle = GenerateSpecObstacle();
            if (specObstacle != EnumObstacles.NA)
                return specObstacle;

            //Обычная генерация
            var types = new EnumObstacles[] { EnumObstacles.Small, };
            return types.GetRandom();
        }        
    }

    private EnumObstacles GenerateSpecObstacle()
    {
        if (isLastSpecObstacle)
        {
            isLastSpecObstacle = false;
            return EnumObstacles.NA;
        }

        var instance = GameManager.Instance;

        //Шанс зависиит от текущего кол-ва денег у Игрока
        var moneyT = Mathf.InverseLerp(ConstantsSettings.maxMoney, 0.0f, instance.moneyToday);
        var moneyChance = instance.moneyToday >= ConstantsSettings.maxMoney
                        ? ConstantsSettings.minMoneyChance
                        : Mathf.Lerp(ConstantsSettings.minMoneyChance, ConstantsSettings.maxMoneyChance, moneyT);

        //Шанс постоянный, если не максимальное значение Hit Points 
        var hpChance = instance.boomerang.hp == instance.boomerang.GetMaxHitPoints()
                        ? 0.0f
                        : ConstantsSettings.maxHPChance;

        //Шанс зависит от текущего заполнения шкалы Заморозки Времени
        var timeT = Mathf.InverseLerp(ConstantsSettings.maxTimeFreezeNormalize, 0.0f, instance.boomerang.GetTimeFreezeNormalize());
        var timeChance = instance.boomerang.GetTimeFreezeNormalize() >= ConstantsSettings.maxTimeFreezeNormalize
                        ? 0.0f
                        : Mathf.Lerp(ConstantsSettings.minTimeFreezeChance, ConstantsSettings.maxTimeFreezeChance, timeT);

        //Рандом
        var sumChances = moneyChance + hpChance + timeChance + ConstantsSettings.blockChance;
        var chance = Random.Range(0.0f, sumChances);
        Debug.LogWarning(string.Format("moneyChance: {0}, hpChance: {1}, timeChance: {2}, CHANCE: {3}, MAX: {4}", moneyChance, hpChance, timeChance, chance, sumChances));

        //Проверка
        if (chance < moneyChance)
        {
            isLastSpecObstacle = true;
            return EnumObstacles.Money;
        }
        else if (chance < hpChance + moneyChance)
        {
            isLastSpecObstacle = true;
            return EnumObstacles.HP;
        }
        else if (chance < timeChance + hpChance + moneyChance)
        {
            isLastSpecObstacle = true;
            return EnumObstacles.Time;
        }
        else
        {
            return EnumObstacles.NA;
        }
    }

    private Vector3 GetObstaclePosition()
    {
        var borderIndexes = new List<int>();
        for (int i = 0; i < borderPoints.Length - 1; i++)
            if (!lastBorderIndexes.Contains(i))
                borderIndexes.Add(i);

        var currBorderIndex = borderIndexes.GetRandom();

        //var t = Random.Range(0.1f, 0.9f);           //Рандом точки отрезка (отступ по 10% ро бокам)
        var t = Random.Range(borderNormalizeOffset, 1.0f - borderNormalizeOffset);
        var instOffset = new Vector3(0.0f, Random.Range(offsetSpawn.x, offsetSpawn.y), 0.0f);
        var newPos = Vector3.Lerp(borderPoints[currBorderIndex].position, borderPoints[currBorderIndex+1].position, t) + instOffset;

        lastBorderIndexes.Add(currBorderIndex);

        return newPos;
    }
    #endregion

    #region Coroutines
    private IEnumerator SpawnerCoroutine()
    {
        //Ждем кадр
        yield return null;

        //Первый спаун препятствия без таймера
        Spawn();

        //В бесконечном цикле по таймеру генерируем препятствия
        bool isLoop = true;
        while (isLoop)
        {
            var nextTimer = GetNextTimerSpawn();
            yield return new WaitForSeconds(nextTimer);

            Spawn();
        }
    }
    #endregion
}
