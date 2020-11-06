using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShipPanelController : ListChangedObserveComponent<PlayerKingdom.ProductWrapper, PlayerKingdom>
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

    [SerializeField]
    private Button _shipLaunchButton = null;

    private PlayerKingdom.ProductWrapper _selectedProduct;

    private Button _selectedSocketButton = null;
    private UISocketContentsProperty _selectedSocketProperty = null;

    private Button _selectedWeaponButton = null;
    private PlayerKingdom.ProductionTask _selectedTask = null;

    private Dictionary<PlayerKingdom.ProductionTask, GameObject> _weaponContentsUIHash = new Dictionary<PlayerKingdom.ProductionTask, GameObject>();

    protected override void Awake()
    {
        ListChangedObserveComponent<PlayerKingdom.ProductionTask, PlayerKingdom>.BroadcastAvailableTaskChanged += OnProductListChange;
        base.Awake();
    }

    protected override void OnListChanged(PlayerKingdom.ProductWrapper changed, bool isAdd)
    {
        base.OnListChanged(changed, isAdd);

        if(PawnBaseController.CompareType(changed.ProductData.Product, PawnBaseController.PawnType.SpaceShip))
        {
            if (isAdd)
            {
                CreateShipDataContentsOnScroll(changed);
            }
            else
            {
                GameObject cache = _objectUIContentsHash[changed];
                _objectUIContentsHash.Remove(changed);
                Destroy(cache);
            }
        }
    }

    private void OnProductListChange(PlayerKingdom.ProductionTask changed, bool isAdd)
    {
        if (PawnBaseController.CompareType(changed.Product, PawnBaseController.PawnType.Weapon))
            _weaponContentsUIHash[changed].SetActive(true);
    }

    protected override void LoadList()
    {
        PlayerKingdom.GetInstance().ShipCargoList.ForEach((PlayerKingdom.ProductWrapper product) =>
        {
            CreateShipDataContentsOnScroll(product);
        });

        Singleton<PlayerKingdom>.ListenSingletonLoaded(() =>
        {
            PlayerKingdom.GetInstance().ProductList.ForEach((PlayerKingdom.ProductionTask pTask) =>
            {
                if (PawnBaseController.CompareType(pTask.Product, PawnBaseController.PawnType.Weapon))
                {
                    GameObject cache = Instantiate(_weaponScrollViewContents, _weaponScrollView.content);
                    _weaponContentsUIHash.Add(pTask, cache);

                    cache.GetComponent<UIProductContentsProperty>().SetUIContentsData(pTask);

                    Button cacheButton = cache.GetComponent<Button>();
                    cacheButton.onClick.AddListener(() => OnClickProductionContents(pTask, cacheButton));

                    cache.SetActive(false);
                }
            });
        });

        _socketAttachButton.onClick.AddListener(() => OnClickSocketAttachButton());
        _shipLaunchButton.onClick.AddListener(() => OnClickLaunchButton());
    }

    private void CreateShipDataContentsOnScroll(PlayerKingdom.ProductWrapper product)
    {
        if(!PawnBaseController.CompareType(product.Instance, PawnBaseController.PawnType.SpaceShip))
        {
            GlobalLogger.CallLogError(product.ProductData.TaskName, GErrorType.InspectorValueException);
            return;
        }

        GameObject cache = Instantiate(_shipScrollDataContents, _shipCargoScrollView.content);
        Button buttonCache = cache.GetComponent<Button>();

        cache.GetComponent<UIShipDataContentsProperty>().SetUIContentsData(product);

        buttonCache.onClick.AddListener(() => OnClickShipDataContents(product));
        _objectUIContentsHash.Add(product, cache);
    }

    private void OnClickProductionContents(PlayerKingdom.ProductionTask pTask, Button uiButton)
    {
        if (_selectedWeaponButton != null)
            _selectedWeaponButton.image.color = Color.white;
        uiButton.image.color = Color.black;

        _selectedWeaponButton = uiButton;
        _selectedTask = pTask;
    }

    private void OnClickShipDataContents(PlayerKingdom.ProductWrapper product)
    {
        if (_socketScrollView.content.childCount > 1)
            for (int i = 1; i < _socketScrollView.content.childCount; i++)
                Destroy(_socketScrollView.content.GetChild(i).gameObject);

        _selectedProduct = product;

        List<GameObject> socketList = new List<GameObject>();
        Transform socketAnchor = product.Instance.GetComponent<PawnBaseController>().SocketAnchor.transform;

        for (int i = 0; i < socketAnchor.childCount; i++)
            socketList.Add(socketAnchor.GetChild(i).gameObject);

        socketList.ForEach((GameObject socket) =>
        {
            GameObject cache = Instantiate(_socketScrollViewContents, _socketScrollView.content);
            UISocketContentsProperty property = cache.GetComponent<UISocketContentsProperty>();
            Button cacheButton = cache.GetComponent<Button>();

            cacheButton.onClick.AddListener(() => OnClickSocketContents(property, cacheButton));
            property.SetSocket(socket);
        });
    }

    private void OnClickSocketContents(UISocketContentsProperty property, Button button)
    {
        if (_selectedSocketButton != null)
            _selectedSocketButton.image.color = Color.white;
        button.image.color = Color.black;

        _selectedSocketButton = button;
        _selectedSocketProperty = property;
    }

    private void OnClickSocketAttachButton()
    {
        if (_selectedTask != null && _selectedSocketProperty != null)
        {
            _selectedSocketProperty.AttachSocket(_selectedTask);
        }
    }

    private void OnClickLaunchButton()
    {
        if (_selectedProduct.Instance != null)
            PlayerKingdom.GetInstance().ShipToField(_selectedProduct);
        _selectedProduct = new PlayerKingdom.ProductWrapper();
    }
}