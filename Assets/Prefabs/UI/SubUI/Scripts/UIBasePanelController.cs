using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBasePanelController : MonoBehaviour, IUIContentsCallbacks
{
    [SerializeField]
    private ScrollRect _socketScrollRect = null;

    [SerializeField]
    private ScrollRect _weaponScrollRect = null;

    [SerializeField]
    private GameObject _socketUIContents = null;

    [SerializeField]
    private GameObject _productUIContents = null;

    [SerializeField]
    private GameObject _socketAttachButton = null;

    private void Awake()
    {
        WeaponListBroadcaster.ListenWeaponListChanged(_weaponScrollRect, this);
    }

    public void OnClickProductContents(Button clicked, PlayerKingdom.ProductionTask pTask)
    {
        
    }

    public void OnClickShipDataContents(Button clicked, PlayerKingdom.ProductWrapper pWrapper)
    {
        
    }
}
