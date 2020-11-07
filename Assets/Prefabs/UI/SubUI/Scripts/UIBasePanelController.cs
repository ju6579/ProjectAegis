using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PlayerKindom.PlayerKindomTypes;

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

    public void OnClickProductContents(Button clicked, ProductionTask pTask)
    {
        
    }

    public void OnClickShipDataContents(Button clicked, ProductWrapper pWrapper)
    {
        
    }
}
