using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIProductPanelController : ListChangedObserveComponent<PlayerKingdom.ProductionTask, PlayerKingdom>
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
    private Image _targetImage = null;

    [SerializeField]
    private Text _targetName = null;

    [SerializeField]
    private Text _targetType = null;

    [SerializeField]
    private Text _targetCount = null;

    [SerializeField]
    private Text _targetInformation = null;

    [SerializeField]
    private GameObject _productScrollViewContentUI = null;

    [SerializeField]
    private GameObject _taskProceedContent = null;

    [SerializeField]
    private Button _createButton = null;

    [SerializeField]
    private ScrollRect _currentTaskScrollView = null;

    private PlayerKingdom.ProductionTask _selectedTask = null;
    private PawnBaseController.PawnType _selectedProductType = PawnBaseController.PawnType.NotSet;

    protected override void Awake()
    {
        base.Awake();

        PlayerUIController.DisableUIPanelEventCallbacks += DisableTargetData;

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

    private void Update()
    {
        if (_selectedProductType == PawnBaseController.PawnType.Weapon)
            _targetCount.text = PlayerKingdom.GetInstance().WeaponCount(_selectedTask).ToString();
    }

    protected override void OnListChanged(PlayerKingdom.ProductionTask changed, bool isAdd)
    {
        base.OnListChanged(changed, isAdd);
        _objectUIContentsHash[changed].SetActive(isAdd);
    }

    protected override void LoadList()
    {
        PlayerKingdom.GetInstance().ProductList.ForEach((PlayerKingdom.ProductionTask pTask) =>
        {
            PawnBaseController pbc = pTask.Product.GetComponent<PawnBaseController>();
            RectTransform targetRectTransform = null;

            switch (pbc.PawnActionType)
            {
                case PawnBaseController.PawnType.Weapon:
                    targetRectTransform = _weaponScrollView.content;
                    break;
                case PawnBaseController.PawnType.SpaceShip:
                    targetRectTransform = _shipScrollView.content;
                    break;

                default:
                    GlobalLogger.CallLogError(pTask.TaskName, GErrorType.InspectorValueException);
                    break;
            }

            GameObject cache = Instantiate(_productScrollViewContentUI, targetRectTransform);
            _objectUIContentsHash.Add(pTask, cache);

            cache.GetComponent<UIProductContentsProperty>().SetUIContentsData(pTask);

            Button cacheButton = cache.GetComponent<Button>();
            cacheButton.onClick.AddListener(() => OnClickProductionContents(pTask));

            cache.SetActive(false);
        });
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

    private void OnClickProductionContents(PlayerKingdom.ProductionTask target)
    {
        EnableTargetDataExceptCountText();

        PawnBaseController pbc = target.Product.GetComponent<PawnBaseController>();
        _selectedTask = target;
        _selectedProductType = pbc.PawnActionType;

        _targetImage.sprite = target.TaskIcon;
        _targetName.text = target.TaskName;
        _targetType.text = _selectedProductType.ToString();
        _targetInformation.text = target.TaskInformation;

        if (_selectedProductType == PawnBaseController.PawnType.Weapon)
        {
            _targetCount.gameObject.SetActive(true);
            _targetCount.text = PlayerKingdom.GetInstance().WeaponCount(target).ToString();
        }
        else
            _targetCount.gameObject.SetActive(false);
    }

    private void DisableTargetData()
    {
        _targetImage.gameObject.SetActive(false);
        _targetName.gameObject.SetActive(false);
        _targetType.gameObject.SetActive(false);
        _targetCount.gameObject.SetActive(false);
        _targetInformation.gameObject.SetActive(false);

        _selectedTask = null;
        _selectedProductType = PawnBaseController.PawnType.NotSet;
    }

    private void EnableTargetDataExceptCountText()
    {
        _targetImage.gameObject.SetActive(true);
        _targetName.gameObject.SetActive(true);
        _targetType.gameObject.SetActive(true);
        _targetInformation.gameObject.SetActive(true);
    }

    private void OnClickShipScrollButton()
    {
        _weaponScrollView.gameObject.SetActive(false);
        _shipScrollView.gameObject.SetActive(true);
        DisableTargetData();
    }

    private void OnClickWeaponScrollButton()
    {
        _weaponScrollView.gameObject.SetActive(true);
        _shipScrollView.gameObject.SetActive(false);
        DisableTargetData();
    }
}
