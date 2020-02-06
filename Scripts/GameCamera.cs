using DllSky.Patterns;
using UnityEngine;

public class GameCamera : Singleton<GameCamera>
{
    #region Variables
    [Header("Boomerang")]
    public Transform target;
    public float offset = 2.0f;

    [Space()]
    public Camera cam;

    private float startY;
    private float startOffset;    
    #endregion

    #region Unity methods
    private void Awake()
    {
        cam = GetComponent<Camera>();

        AutoApplyCameraSize();

        startY = transform.position.y;
        startOffset = startY - offset;
    }

    private void OnEnable()
    {
        var instance = GameManager.Instance;

        instance.EventUpdateBoomerang += OnUpdateBoomerangHandle;
        instance.EventShowMainMenu += ApplyDefault;
    }

    private void OnDisable()
    {
        var instance = GameManager.Instance;

        if (instance == null)
            return;

        instance.EventUpdateBoomerang -= OnUpdateBoomerangHandle;
        instance.EventShowMainMenu -= ApplyDefault;
    }

    //private void LateUpdate()
    private void Update()
    {
        if (target)
            if (target.localPosition.y > startOffset)
                transform.position = new Vector3(transform.position.x, target.position.y + offset, transform.position.z);
    }
    #endregion

    #region Public methods
    #endregion

    #region Private methods
    private void AutoApplyCameraSize()
    {
        //для широкоформатной камеры  Camera.main.orthographicSize = длинаПлашки * Camera.main.pixelHeight / Camera.main.pixelWidth * .5f;
        //Для вертикальной камеры     Camera.main.orthographicSize = высотаПлашки * Camera.main.pixelWidth / Camera.main.pixelHeight * .5f;
        cam.orthographicSize = ConstantsSettings.camWidthUnits * cam.pixelHeight / cam.pixelWidth * 0.5f;
    }

    private void OnUpdateBoomerangHandle()
    {
        target = GameManager.Instance.boomerang.transform;
    }

    private void ApplyDefault()
    {
        transform.position = new Vector3(transform.position.x, startY, transform.position.z);
    }
    #endregion

    #region Coroutines
    #endregion
}
