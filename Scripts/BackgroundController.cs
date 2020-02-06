using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using DllSky.Extensions;
using System;

public class BackgroundController : MonoBehaviour
{
    #region Variables
    [Header("Settings")]
    public string bgID = "";
    public float speed;
    public float coeffSpeedBG = 0.25f;
    public float coeffSpeedPart = 0.5f;

    [Header("BackGround")]
    public Transform bgParent;
    public CanvasGroup fog;
    public List<GameObject> bgPrefabs = new List<GameObject>();
    public List<GameObject> ptPrefabs = new List<GameObject>();
    public List<GameObject> backgrounds = new List<GameObject>();
    public List<GameObject> parts = new List<GameObject>();

    private GameManager instance;
    private GameCamera cam;
    #endregion

    #region Unity methods
    private void Start()
    {
        instance = GameManager.Instance;
        cam = GameCamera.Instance;

        Initialize(_fogImmediately: true);
    }

    private void OnEnable()
    {
        if (instance == null)
            instance = GameManager.Instance;

        instance.EventSetSpeed += SetSpeed;
    }

    private void OnDisable()
    {
        if (instance == null)
            return;

        instance.EventSetSpeed -= SetSpeed;
    }

    private void Update()
    {
        //Перемещение родительского объекта-контроллера вслед за Бумерангом
        //if (boomerang)
        //    transform.position = new Vector3(transform.position.x, boomerang.position.y, transform.position.z);
        transform.position = new Vector3(transform.position.x, GameCamera.Instance.transform.position.y, transform.position.z);

        //Перемещение тумана
        fog.transform.position = new Vector3(fog.transform.position.x, GameCamera.Instance.transform.position.y, fog.transform.position.z);

        //Перемещение частей фона и других объектов параллакса
        MovingObjects();
    }

    private void OnDrawGizmos()
    {
        //Точка "смерти" фона
        var point = new Vector3(0.0f, Camera.main.transform.position.y - (ConstantsSettings.bgHeight / 100.0f), 0.0f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(point, 0.25f);
        Gizmos.DrawLine(point + Vector3.left, point + Vector3.right);
    }
    #endregion

    #region Public methods
    public void Initialize(string _background = DefaultInventoryData.IdStandardBoomerang, bool _fogImmediately = false)
    {
        bgID = _background;

        ShowFog(_fogImmediately);

        LoadBGPrefabs();
        ClearBG();
        CreateBG();

        HideFog();
    }
    #endregion

    #region Private methods
    private void SetSpeed(float _value)
    {
        speed = _value;
    }

    private void MovingObjects()
    {
        var pointOfDestroy = cam.transform.position.y  - (ConstantsSettings.bgHeight / 100.0f);

        //Основной фон
        backgrounds = backgrounds.OrderBy(x => x.transform.position.y).ToList();
        var synchPos = backgrounds?[0]?.transform?.position ?? Vector3.zero;
        var deltaBG = ConstantsSettings.bgHeight / 100.0f;
        var deltaY = speed * coeffSpeedBG * Time.deltaTime;        
        //deltaY = ((int)(y * 1000)) / 1000.0f;
        for (int i = 0; i < backgrounds.Count; i++)
        {
            var bg = backgrounds[i];
            //bg.transform.position -= new Vector3(0.0f, deltaY, 0.0f);
            bg.transform.position = new Vector3(synchPos.x, synchPos.y + i*deltaBG - deltaY, synchPos.z);

            if (bg.transform.position.y <= pointOfDestroy)
                bg.transform.position += new Vector3(0.0f, backgrounds.Count * deltaBG, 0.0f);
        }

        //Другие объекты параллакс-эффекта
        deltaY = speed * coeffSpeedPart * Time.deltaTime;
        for (int i = parts.Count - 1; i >= 0; i--)
        {
            var pt = parts[i];
            pt.transform.position -= new Vector3(0.0f, deltaY, 0.0f);

            if (pt.transform.position.y <= pointOfDestroy)
                Destroy(pt);
        }
    }

    private void LoadBGPrefabs()
    {
        //To LOG
        var now = DateTime.Now;

        //Префабы фона
        bgPrefabs.Clear();
        bgPrefabs = Resources.LoadAll<GameObject>(string.Format(@"Prefabs/Skins/{0}/Backgrounds", bgID)).ToList();
        bgPrefabs.Shuffle();

        //Префабы объектов/деталей фона
        ptPrefabs.Clear();
        ptPrefabs = Resources.LoadAll<GameObject>(string.Format(@"Prefabs/Skins/{0}/BgParts", bgID)).ToList();
        ptPrefabs.Shuffle();

        //Debug
        Debug.LogWarning(string.Format("[BackgroundController] Load prefabs long: {0} ms.", (DateTime.Now - now).TotalMilliseconds));
    }

    private void ClearBG()
    {
        for (int i = backgrounds.Count - 1; i >= 0; i--)
            Destroy(backgrounds[i]);
        backgrounds.Clear();

        for (int i = parts.Count - 1; i >= 0; i--)
            Destroy(parts[i]);
        parts.Clear();
    }

    private void CreateBG()
    {
        var y = 0.0f;

        foreach (var pref in bgPrefabs)
        {
            var newObj = Instantiate(pref, bgParent);
            newObj.transform.localPosition = new Vector3(0.0f, y, 0.0f);

            backgrounds.Add(newObj);

            y += ConstantsSettings.bgHeight / 100.0f;
        }

        //TODO:
        //Запуск корутины? с генерацией объектов/деталей фона
        //...
    }

    private void ShowFog(bool _immediately = false)
    {
        StopAllCoroutines();

        if (_immediately)
        {
            fog.alpha = 1.0f;
            fog.gameObject.SetActive(true);
        }
        else
        {
            StartCoroutine(ShowFogAnimation());
        }
    }

    private void HideFog(bool _immediately = false)
    {
        StopAllCoroutines();

        if (_immediately)
        {
            fog.alpha = 0.0f;
            fog.gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(HideFogAnimation());
        }
    }    
    #endregion

    #region Coroutines
    private IEnumerator ShowFogAnimation()
    {
        fog.gameObject.SetActive(true);

        var aStart = fog.alpha;
        var aEnd = 1.0f;

        var time = 0.5f;
        var T = 0.0f;
        var t = 0.0f;

        while (T < 1.0f)
        {
            T = Mathf.InverseLerp(0.0f, time, t);
            fog.alpha = Mathf.Lerp(aStart, aEnd, T);

            yield return null;

            t += Time.deltaTime;
        }        

        fog.alpha = aEnd;
    }

    private IEnumerator HideFogAnimation()
    {
        var aStart = fog.alpha;
        var aEnd = 0.0f;

        var time = 0.5f;
        var T = 0.0f;
        var t = 0.0f;

        while (T < 1.0f)
        {
            T = Mathf.InverseLerp(0.0f, time, t);
            fog.alpha = Mathf.Lerp(aStart, aEnd, T);

            yield return null;

            t += Time.deltaTime;
        }

        fog.alpha = aEnd;

        fog.gameObject.SetActive(false);
    }
    #endregion
}
