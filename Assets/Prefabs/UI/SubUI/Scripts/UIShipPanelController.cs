using UnityEngine;
using UnityEngine.UI;

public class UIShipPanelController : MonoBehaviour
{
    [SerializeField]
    private ScrollRect _shipCargoScrollView = null;

    [SerializeField]
    private GameObject _shipScrollDataContents = null;

    [SerializeField]
    private ScrollRect _weaponScrollView = null;

    [SerializeField]
    private GameObject _weaponScrollViewContents = null;

    [SerializeField]
    private ScrollRect _socketScrollView = null;

    [SerializeField]
    private GameObject _socketScrollViewContents = null;

    [SerializeField]
    private Button _socketAttachButton = null;

    private void Awake()
    {
        WeaponListBroadcaster.ListenWeaponListChange(_weaponScrollView);
        CargoShipListBroadcaster.ListenCargoShipListChanged(_shipCargoScrollView);
    }
}