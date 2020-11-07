using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PlayerKindom.PlayerKindomTypes;

public class UIShipDataContentsProperty : MonoBehaviour
{
    public ProductWrapper ShipProduct => _shipSet;

    public void AttachWeaponToSocket(ProductionTask pTask)
    {
        _selectedSocketProperty.AttachSocket(pTask);
    }

    public void SetUIContentsData(ProductWrapper product)
    {
        _shipSet = product;

        _shipName.text = product.ProductData.TaskName;
        _shipImage.sprite = product.ProductData.TaskIcon;

        if (!PawnBaseController.CompareType(_shipSet.Instance, PawnBaseController.PawnType.SpaceShip))
        {
            GlobalLogger.CallLogError(_shipSet.ProductData.TaskName, GErrorType.InspectorValueException);
            this.gameObject.SetActive(false);
            return;
        }

        ShipController ship = product.Instance.GetComponent<ShipController>();
        _shipProperty = ship.ShipData;

        ship.SocketList.ForEach((GameObject go) =>
        {
            GameObject cache = Instantiate(_socketUIContentsObject);
            Button button = cache.GetComponent<Button>();
            UISocketContentsProperty socketProperty = cache.GetComponent<UISocketContentsProperty>();

            button.onClick.AddListener(() => OnClickSocketContents(button, socketProperty));

            _shipSocketUIContents.Add(socketProperty);
            socketProperty.SetSocket(go);

            cache.SetActive(false);
        });

        PlayerUIController.GetInstance().StartCoroutine(_ObserveShipData());
    }

    private Vector3 _defaultLocalScale = new Vector3(1, 1, 0);
    public void AddContentsToScrollRect(ScrollRect targetView)
    {
        _shipSocketUIContents.ForEach((UISocketContentsProperty socket) =>
        {
            socket.transform.SetParent(targetView.content);
            socket.transform.localScale = _defaultLocalScale;
            socket.gameObject.SetActive(true);
        });
    }

    public void RemoveContentsToScrollRect()
    {
        _shipSocketUIContents.ForEach((UISocketContentsProperty socket) =>
        {
            socket.transform.SetParent(null);
            socket.transform.localScale = _defaultLocalScale;
            socket.gameObject.SetActive(false);
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
    private Text _arrivalTime = null;

    [SerializeField]
    private GameObject _socketUIContentsObject = null;
    #endregion

    #region Private Field
    private Button _contentsButton = null;
    #endregion

    #region Selected Data Field
    private ProductWrapper _shipSet;
    private ShipController.ShipProperty _shipProperty;
    private List<UISocketContentsProperty> _shipSocketUIContents = new List<UISocketContentsProperty>();

    private Button _selectedSocketButton = null;
    private UISocketContentsProperty _selectedSocketProperty = null;

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
        _arrivalTime.text = _shipProperty.ArrivalTime.ToString();
    }
}