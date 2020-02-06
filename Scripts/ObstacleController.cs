using UnityEngine;
using System.Collections;
using System;

public class ObstacleController : MonoBehaviour
{
    #region Variables
    [Header("Main")]
    public int points = 5;
    public bool isClaimedPoints = false;
    public string key = "";                     //Ключ для картинки (одинаковое название скинов в разных папках)

    [Header("Settings trajectory")]
    public float speed = 1.5f;    

    [Header("VFX")]
    public GameObject destroedObject;

    private Transform boomerang;
    #endregion

    #region Unity methods
    private void Start()
    {
        var instance = GameManager.Instance;

        speed = instance.startSpeed;
        boomerang = instance.boomerang.transform;

        //TODO:
        //Применение соответствующего скина (пока не актуально)
        //...
    }

    private void OnEnable()
    {
        var instance = GameManager.Instance;

        instance.EventSetSpeed += SetSpeed;

        instance.EventShowMainMenu += ApplyDefault;
        instance.EventGameOver += ApplyGameOver;
    }

    private void OnDisable()
    {
        var instance = GameManager.Instance;

        if (instance == null)
            return;

        instance.EventSetSpeed -= SetSpeed;

        instance.EventShowMainMenu -= ApplyDefault;
        instance.EventGameOver -= ApplyGameOver;
    }

    private void FixedUpdate()
    {
        var y = speed * Time.fixedDeltaTime;
        transform.localPosition -= new Vector3(0.0f, y, 0.0f);     
    }

    private void Update()
    {
        if (GameManager.Instance.currentStatus != EnumGameStatus.Game)
            return;

        //Проверка на необходимость начислить очки
        CheckToClaimPoints(false);
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        var otherTag = _collision.tag;
        if (otherTag == ConstantsTag.TAG_OBSTACLE
            || otherTag == ConstantsTag.TAG_HP
            || otherTag == ConstantsTag.TAG_TIME
            || otherTag == ConstantsTag.TAG_MONEY)
            return;

        //Проверка на необходимость начислить очки
        CheckToClaimPoints(true);

        Instantiate(destroedObject, transform.position, transform.rotation, transform.parent);
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        //Самоуничтожение
        try
        {
            if (transform.position.y < GameManager.Instance.boomerang.transform.position.y)
                ApplyDefault();
        }
        catch
        {

        }
    }    
    #endregion

    #region Public methods
    #endregion

    #region Private methods
    private void ApplyDefault()
    {
        Destroy(gameObject);
    }

    private void ApplyGameOver()
    {
        Instantiate(destroedObject, transform.position, transform.rotation, transform.parent);
        Destroy(gameObject);
    }

    private void SetSpeed(float _value)
    {
        speed = _value;
    }

    private void CheckToClaimPoints(bool _isCollision)
    {
        if (isClaimedPoints)
            return;

        var instance = GameManager.Instance;
        if (instance.currentStatus != EnumGameStatus.Game)
            return;

        //Множитель за Бустер Score
        var coeff = instance.usingBoosterScore ? ConstantsSettings.boosterScoreCoeff : 1.0f;
        var addPoints = (int)(points * coeff);

        switch (tag)
        {
            //Препятствия - начисляем очки, когда они по Y ниже Бумеранга
            case ConstantsTag.TAG_OBSTACLE:
                if (_isCollision)       //Если в коллизии
                    break;
                if (transform.position.y < boomerang.position.y)
                {
                    isClaimedPoints = true;
                    instance.EventSetScore.Invoke(instance.curScore + addPoints);       //Очки рейтинга
                    instance.EventSetEXP.Invoke(instance.curExp + addPoints);           //Очки опыта
                }
                break;

            //Время, Деньги и HP - начисляем при коллизии
            case ConstantsTag.TAG_TIME:
            case ConstantsTag.TAG_MONEY:
            case ConstantsTag.TAG_HP:
                if (!_isCollision)      //Если не в коллизии
                    break;
                isClaimedPoints = true;
                instance.EventSetScore.Invoke(instance.curScore + addPoints);           //Очки рейтинга
                instance.EventSetEXP.Invoke(instance.curExp + addPoints);               //Очки опыта
                break;
        }
    }
    #endregion
}
