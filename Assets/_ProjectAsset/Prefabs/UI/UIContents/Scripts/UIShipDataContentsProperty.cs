using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PlayerKindom.PlayerKindomTypes;
using Pawn;

public class UIShipDataContentsProperty : MonoBehaviour
{
    public ProductWrapper ShipProduct => _shipSet;
    public void SetTargetShipPanel(UIShipPanelController shipPanel) => _targetShipPanelController = shipPanel;

    public void AttachWeaponToSocket(ProductionTask pTask, UISocketContentsProperty targetSocket)
    {
        targetSocket.AttachSocket(pTask, _shipController);
    }

    public void SetUIContentsData(ProductWrapper product)
    {
        _shipSet = product;

        _shipName.text = product.ProductData.TaskName;
        _shipImage.sprite = product.ProductData.TaskIcon;

        if (!PawnBaseController.CompareType(_shipSet.Instance, PawnType.SpaceShip))
        {
            GlobalLogger.CallLogError(_shipSet.ProductData.TaskName, GErrorType.InspectorValueException);
            this.gameObject.SetActive(false);
            return;
        }

        _shipController = product.Instance.GetComponent<ShipController>();
        _shipProperty = _shipController.ShipData;

        _shipController.SocketList.ForEach((GameObject go) =>
        {
            GameObject cache = Instantiate(_socketUIContentsObject, PlayerUIController.GetInstance().Dummy);
            Button button = cache.GetComponent<Button>();
            UISocketContentsProperty socketProperty = cache.GetComponent<UISocketContentsProperty>();

            button.onClick.AddListener(() => OnClickSocketContents(button, socketProperty));

            _shipSocketUIContents.Add(socketProperty);
            socketProperty.SetSocket(go);

            cache.SetActive(false);
        });

        _shipController.ShipProduct = product;

        PlayerUIController.GetInstance().StartCoroutine(_ObserveShipData());
    }

    private Vector3 _defaultLocalScale = new Vector3(1, 1, 0);
    public void AddContentsToScrollRect(ScrollRect targetView)
    {
        _shipSocketUIContents.ForEach((UISocketContentsProperty socket) =>
        {
            if(socket != null)
            {
                socket.transform.SetParent(targetView.content);

                socket.transform.localScale = _defaultLocalScale;
                socket.transform.localRotation = Quaternion.identity;

                socket.transform.localPosition = -Vector3.forward * 0.01f;

                socket.gameObject.SetActive(true);
            }
        });
    }

    public void RemoveContentsToScrollRect()
    {
        _shipSocketUIContents.ForEach((UISocketContentsProperty socket) =>
        {
            if(socket != null)
            {
                socket.transform.SetParent(PlayerUIController.GetInstance().Dummy);
                socket.transform.localScale = _defaultLocalScale;
                socket.gameObject.SetActive(false);
            }
        });
    }

    #region Serialize Field
    [SerializeField]
    private Image _shipImage = null;

    [SerializeField]
    private Text _shipName = null;

    [SerializeField]
    private Text _shieldAmount = null;

    [SerializeField]
    private Text _armorAmount = null;

    [SerializeField]
    private GameObject _socketUIContentsObject = null;
    #endregion

    #region Private Field
    private UIShipPanelController _targetShipPanelController = null;
    private Button _contentsButton = null;
    #endregion

    #region Selected Data Field
    private ProductWrapper _shipSet;
    private SpaceShipProperty _shipProperty;
    private List<UISocketContentsProperty> _shipSocketUIContents = new List<UISocketContentsProperty>();

    private Button _selectedSocketButton = null;
    private UISocketContentsProperty _selectedSocketProperty = null;
    private ShipController _shipController;

    public void ClearSelectContents()
    {
        if (_selectedSocketButton)
        {
            _selectedSocketButton.image.color = Color.white;
        }
        
        _selectedSocketButton = null;
        _selectedSocketProperty = null;
    }
    #endregion

    #region MonoBehaviour Callbacks
    private void Awake()
    {
        _contentsButton = GetComponent<Button>();
    }

    private void OnDisable()
    {
        _contentsButton.image.color = Color.white;

        ClearSelectContents();
        RemoveContentsToScrollRect();
    }
    #endregion

    private void OnClickSocketContents(Button clicked, UISocketContentsProperty socket)
    {
        ClearSelectContents();

        clicked.image.color = Color.red;
        _selectedSocketButton = clicked;
        _selectedSocketProperty = socket;

        if(_targetShipPanelController.SelectedWeapon != null)
            AttachWeaponToSocket(_targetShipPanelController.SelectedWeapon, socket);
    }

    private IEnumerator _ObserveShipData()
    {
        WaitForEndOfFrame frameWait = new WaitForEndOfFrame();
        while(this != null)
        {
            UpdateUIContents();

            yield return frameWait;
        }
        yield return null;
    }

    private void UpdateUIContents()
    {
        _shieldAmount.text = _shipProperty.ShieldPoint.ToString();
        _armorAmount.text = _shipProperty.ArmorPoint.ToString();
    }
}