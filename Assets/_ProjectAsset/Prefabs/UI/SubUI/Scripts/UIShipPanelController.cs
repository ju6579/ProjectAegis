using UnityEngine;
using UnityEngine.UI;

using PlayerKindom;
using PlayerKindom.PlayerKindomTypes;

public class UIShipPanelController : MonoBehaviour, IUIContentsCallbacks
{
    public ProductionTask SelectedWeapon => _selectedWeapon;

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
    private Button _shipLaunchButton = null;

    private Button _selectedWeaponButton = null;
    private ProductionTask _selectedWeapon = null;

    private Button _selectedShipDataButton = null;
    private UIShipDataContentsProperty _selectedShipData = null;
    private ProductWrapper _selectedShip;

    private void OnDisable()
    {
        ClearShipData();
        ClearWeaponData();
    }

    private void Awake()
    {
        WeaponListBroadcaster.ListenWeaponListChanged(_weaponScrollView, this);
        CargoShipListBroadcaster.ListenCargoShipListChanged(_shipCargoScrollView, this);

        _shipLaunchButton.onClick.AddListener(() => OnClickLaunchButton());
    }

    private void ClearWeaponData()
    {
        if(_selectedWeaponButton != null)
        {
            _selectedWeaponButton.image.color = Color.white;
        }

        _selectedWeapon = null;
    }

    private void ClearShipData()
    {
        if (_selectedShipDataButton != null)
        {
            _selectedShipDataButton.image.color = Color.white;
            _selectedShipData.RemoveContentsToScrollRect();

            _selectedShipDataButton = null;
            _selectedShipData = null;
            _selectedShip = null;
        }
    }

    public void OnClickProductContents(Button clicked, ProductionTask pTask)
    {
        ClearWeaponData();

        clicked.image.color = Color.red;

        _selectedWeaponButton = clicked;
        _selectedWeapon = pTask;
    }

    public void OnClickShipDataContents(Button clicked, ProductWrapper pWrapper)
    {
        ClearShipData();

        _selectedShipDataButton = clicked;

        clicked.image.color = Color.red;
        _selectedShipData = clicked.GetComponent<UIShipDataContentsProperty>();
        _selectedShip = _selectedShipData.ShipProduct;
        _selectedShipData.SetTargetShipPanel(this);

        _selectedShipData.AddContentsToScrollRect(_socketScrollView);
    }

    public void OnClickLaunchButton()
    {
        if(_selectedShip != null)
            PlayerKingdom.GetInstance().ShipToField(_selectedShip);
        ClearShipData();
    }
}