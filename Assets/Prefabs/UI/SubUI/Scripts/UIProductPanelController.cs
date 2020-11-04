using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIProductPanelController : MonoBehaviour
{
    [SerializeField]
    private ScrollRect _shipScrollView = null;

    [SerializeField]
    private ScrollRect _weaponScrollView = null;

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

    private void Awake()
    {
        PlayerUIController.DisableUIPanelEventCallbacks += DisableTargetData;

        PlayerKingdom.ListenAvailableProduction((PlayerKingdom.ProductionTask changed) =>
        {
            PawnBaseController pbc = changed.Product.GetComponent<PawnBaseController>();
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
                    GlobalLogger.CallLogError(changed.TaskName, GErrorType.InspectorValueException);
                    break;
            }   

            GameObject cache = Instantiate(_productScrollViewContentUI, targetRectTransform);
            Button cacheButton = cache.GetComponent<Button>();
            Text cacheText = cache.GetComponentInChildren<Text>();

            cacheText.text = changed.TaskName;
            cacheButton.onClick.AddListener(() => OnClickProductionContents(changed));
        });

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
    }

    private void Update()
    {
        if (_selectedProductType == PawnBaseController.PawnType.Weapon)
            _targetCount.text = PlayerKingdom.GetInstance().WeaponCount(_selectedTask).ToString();
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
}
