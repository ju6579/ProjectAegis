using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : Singleton<PlayerUIController>
{
    public delegate void UIActiveEvent();
    public static UIActiveEvent ActiveUIPanelEventCallbacks;
    public static UIActiveEvent DisableUIPanelEventCallbacks;

    public Transform Dummy => _dummyCanvas.transform;

    [SerializeField]
    private GameObject _mapUIPanel = null;

    [SerializeField]
    private GameObject _shipUIPanel = null;

    [SerializeField]
    private GameObject _productUIPanel = null;

    [SerializeField]
    private GameObject _dummyCanvas = null;

    [SerializeField]
    private Text _timeText = null;

    private bool _isMainPanelActive = false;
    private UIMapPanelController _mapPanelController = null;

    #region Public Method Area
    public void ActiveSpecificPanel(int actionNumber)
    {
        MainMenuButtonType actionType = (MainMenuButtonType)actionNumber;
        DisableAllSubPanel();
        switch (actionType)
        {
            case MainMenuButtonType.Map: _mapUIPanel.SetActive(true); break;
            case MainMenuButtonType.Ship: _shipUIPanel.SetActive(true); break;
            case MainMenuButtonType.Product: _productUIPanel.SetActive(true); break;
        }
    }
    #endregion

    #region MonoBehaviour Callbacks
    protected override void Awake()
    {
        base.Awake();
        ActiveUIPanelEventCallbacks += ActiveMainPanel;
        DisableUIPanelEventCallbacks += DisableMainPanel;

        _dummyCanvas.SetActive(false);
    }

    private void Start()
    {
        DisableUIPanelEventCallbacks();
        ActiveUIPanelEventCallbacks();

        _mapPanelController = GetComponentInChildren<UIMapPanelController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_isMainPanelActive)
            {
                ActiveUIPanelEventCallbacks();
            }
            else
            {
                DisableUIPanelEventCallbacks();
            }
        }

        if (_mapPanelController != null)
            _timeText.text = ((int)_mapPanelController.RemainTime).ToString();
    }
    #endregion

    #region Private Method Area
    private void ActiveMainPanel()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _isMainPanelActive = true;

        ActiveAllSubPanel();
    }

    private void DisableMainPanel()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _isMainPanelActive = false;

        DisableAllSubPanel();
    }

    private void DisableAllSubPanel()
    {
        _mapUIPanel.SetActive(false);
        _shipUIPanel.SetActive(false);
        _productUIPanel.SetActive(false);
    }

    private void ActiveAllSubPanel()
    {
        _mapUIPanel.SetActive(true);
        _shipUIPanel.SetActive(true);
        _productUIPanel.SetActive(true);
    }
    #endregion

    #region Custom Type
    public enum MainMenuButtonType
    {
        Map = 0,
        Team = 1,
        Ship = 2,
        Base = 3,
        Upgrade = 4,
        Product = 5
    }
    #endregion
}
