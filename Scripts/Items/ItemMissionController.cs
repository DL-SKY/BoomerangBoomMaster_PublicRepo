using UnityEngine;
using System.Collections;
using TMPro;

public class ItemMissionController : MonoBehaviour
{
    #region Variables
    [Header("Buttons")]
    public GameObject[] buttons;        //[0]-disable; [1]-enable

    private MissionsItemData data;
    #endregion

    #region Unity methods
    #endregion

    #region Public methods
    public void Initialize(MissionsItemData _data)
    {
        data = _data;
    }    
    #endregion

    #region Private methods
    #endregion

    #region Coroutines
    #endregion
}
