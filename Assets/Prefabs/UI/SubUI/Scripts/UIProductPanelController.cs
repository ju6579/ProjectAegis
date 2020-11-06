using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIProductPanelController : MonoBehaviour
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

    private PlayerKingdom.ProductionTask _selectedTask = null;
    private PawnBaseController.PawnType _selectedProductType = PawnBaseController.PawnType.NotSet;

    private void Awake()
    {
        ShipListBroadcaster.ListenShipListChange(_shipScrollView);
        WeaponListBroadcaster.ListenWeaponListChange(_weaponScrollView);

        _createButton.onClick.AddListener(() =>
        {
            bool isTaskRun = false;
            if (PlayerUIController.SelectedShipTask != null)
                isTaskRun = PlayerKingdom.GetInstance().RequestTaskToKingdom(PlayerUIController.SelectedShipTask);

            if (isTaskRun)
            {
                GameObject cache = Instantiate(_taskProceedContent, _currentTaskScrollView.content);
                PlayerUIController.GetInstance().StartCoroutine(_ObserveTaskProceed(PlayerUIController.SelectedShipTask.TaskExecuteTime, cache));
            }
        });

        _shipScrollButton.onClick.AddListener(() => OnClickShipScrollButton());
        _weaponScrollButton.onClick.AddListener(() => OnClickWeaponScrollButton());
        OnClickShipScrollButton();
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
        PawnBaseController pbc = target.Product.GetComponent<PawnBaseController>();
        _selectedTask = target;
        PlayerUIController.SelectedWeaponTask = target;
        _selectedProductType = pbc.PawnActionType;
    }

    private void OnClickShipScrollButton()
    {
        _weaponScrollView.gameObject.SetActive(false);
        _shipScrollView.gameObject.SetActive(true);
    }

    private void OnClickWeaponScrollButton()
    {
        _weaponScrollView.gameObject.SetActive(true);
        _shipScrollView.gameObject.SetActive(false);
    }
}
