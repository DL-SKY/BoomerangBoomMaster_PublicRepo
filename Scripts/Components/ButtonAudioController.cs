using UnityEngine;
using UnityEngine.UI;

public class ButtonAudioController : MonoBehaviour
{
    #region Variables
    [Header("Settings")]
    public bool isSoundActive = true;
    public string path = "";
    public string file = "";
    [Range(0.0f, 1.0f)]
    public float volume = 1.0f;

    private Button button;
    #endregion

    #region Unity methods
    private void Awake()
    {
        button = GetComponent<Button>();

        if (button)
            button.onClick.AddListener(OnClick);
    }
    #endregion

    #region Public methods
    public void OnClick()
    {
        if (!isSoundActive)
            return;

        AudioManager.Instance.PlaySound(@path, @file, _volume: volume);
    }
    #endregion
}
