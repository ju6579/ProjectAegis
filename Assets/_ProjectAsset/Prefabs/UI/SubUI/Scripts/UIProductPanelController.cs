using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using PlayerKindom;
using PlayerKindom.PlayerKindomTypes;
using Pawn;

public class UIProductPanelController : MonoBehaviour, IUIContentsCallbacks
{
    [SerializeField]
    private ScrollRect _shipScrollView = null;

    [SerializeField]
    private Button _shipScrollButton = null;

    [SerializeField]
    private ScrollRect _weaponScrollView = null;

    [SerializeField]
    private Button _weaponScrollButton = null;

    [SerializeField]
    private GameObject _productScrollViewContentUI = null;

    [SerializeField]
    private GameObject _taskProceedContent = null;

    [SerializeField]
    private Button _createButton = null;

    [SerializeField]
    private ScrollRect _currentTaskScrollView = null;

    [SerializeField]
    private UIInfoProductProperty _infoProperty = null;

    [SerializeField]
    private ShipListBroadcaster _shipBroadcaster = null;

    [SerializeField]
    private WeaponListBroadcaster _weaponBroadcaster = null;

    private Button _selectedButton = null;
    private ProductionTask _selectedTask = null;
    private PawnType _selectedProductType = PawnType.NotSet;

    private void ClearSelectedData()
    {
        if (_selectedButton != null)
            _selectedButton.image.color = Color.white;

        _selectedTask = null;
        _selectedProductType = PawnType.NotSet;
    }

    private void Awake()
    {
        _shipBroadcaster.ListenShipListChanged(_shipScrollView, this);
        _weaponBroadcaster.ListenWeaponListChanged(_weaponScrollView, this);

        _createButton.onClick.AddListener(() =>
        {
            bool isTaskRun = false;
            if (_selectedTask != null)
                isTaskRun = PlayerKingdom.GetInstance().RequestTaskToKingdom(_selectedTask);

            if (isTaskRun)
            {
                GameObject cache = Instantiate(_taskProceedContent, _currentTaskScrollView.content);
                PlayerUIController.GetInstance().StartCoroutine(_ObserveTaskProceed(_selectedTask.TaskExecuteTime, cache));
            }
        });

        _shipScrollButton.onClick.AddListener(() => OnClickShipScrollButton());
        _weaponScrollButton.onClick.AddListener(() => OnClickWeaponScrollButton());
        OnClickShipScrollButton();
    }

    private void OnEnable()
    {
        _selectedButton = null;
        _selectedTask = null;
        _selectedProductType = PawnType.NotSet;
    }

    private void OnDisable()
    {
        _selectedButton = null;
        _selectedTask = null;
        _selectedProductType = PawnType.NotSet;
    }

    private IEnumerator _ObserveTaskProceed(float totalTime, GameObject contents)
    {
        WaitForEndOfFrame _executePerFrame = new WaitForEndOfFrame();
        Slider targetSlider = contents.GetComponentInChildren<Slider>();
        float startTime = Time.time;
        float ratio = 0f;

        while (ratio < 1)
        {
            ratio = (Time.time - startTime) / totalTime;
            targetSlider.value = ratio;
            yield return _executePerFrame;
        }

        Destroy(contents);
    }

    private void OnClickShipScrollButton()
    {
        ClearSelectedData();
        _weaponScrollView.gameObject.SetActive(false);
        _shipScrollView.gameObject.SetActive(true);
    }

    private void OnClickWeaponScrollButton()
    {
        ClearSelectedData();
        _weaponScrollView.gameObject.SetActive(true);
        _shipScrollView.gameObject.SetActive(false);
    }

    public void OnClickProductContents(Button clicked, ProductionTask pTask)
    {
        ClearSelectedData();

        clicked.image.color = Color.black;

        _selectedButton = clicked;
        _selectedTask = pTask;
        _selectedProductType = pTask.ProductType;

        _infoProperty.ReplaceProductInfo(pTask);
    }

    public void OnClickShipDataContents(Button clicked, ProductWrapper pWrapper)
    {
        
    }
}
