using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : Singleton<PlayerUIController>
{
    public delegate void UIActiveEvent();
    public static UIActiveEvent ActiveUIPanelEventCallbacks;
    public static UIActiveEvent DisableUIPanelEventCallbacks;

    public Transform Dummy => _dummyCanvas.transform;

    [SerializeField]
    private Transform _canvasAnchor = null;

    [SerializeField]
    private GameObject _kingdomUIPanel = null;

    [SerializeField]
    private GameObject _mapUIPanel = null;

    [SerializeField]
    private GameObject _shipUIPanel = null;

    [SerializeField]
    private GameObject _productUIPanel = null;

    [SerializeField]
    private GameObject _dummyCanvas = null;

    [SerializeField]
    private List<Transform> _panelPositionAnchorSet = null;

    private Dictionary<Transform, Transform> _panelDefaultTransformHash = new Dictionary<Transform, Transform>();
    private Transform _currentActivePanel = null;

    private bool _isMainPanelActive = false;
    private float _horizontalMove = 0f;
    private float _verticalMove = 0f;

    #region Public Method Area
    public void MainUIOnOffAction() 
    {
        if (_isMainPanelActive)
            DisableUIPanelEventCallbacks();
        else
            ActiveUIPanelEventCallbacks();
    }

    public void ActiveSpecificPanel(MainMenuButtonType actionType)
    {
        if (_isMainPanelActive)
        {
            if (_currentActivePanel != null)
                _currentActivePanel.position = _panelDefaultTransformHash[_currentActivePanel].position;

            Transform targetTransform = null;

            switch (actionType)
            {
                case MainMenuButtonType.Map: targetTransform = _mapUIPanel.transform; break;
                case MainMenuButtonType.Ship: targetTransform = _shipUIPanel.transform; break;
                case MainMenuButtonType.Product: targetTransform = _productUIPanel.transform; break;
                case MainMenuButtonType.Kingdom: targetTransform = _kingdomUIPanel.transform; break;
            }

            if(targetTransform != null)
            {
                targetTransform.localPosition = Vector3.zero;

                _currentActivePanel = targetTransform;
            }
        }
    }

    public void MoveMainUIPanel(Vector3 velocity)
    {
        if (_isMainPanelActive)
        {
            _horizontalMove += velocity.z * Time.deltaTime;
            _verticalMove += velocity.y * Time.deltaTime;

            Vector3 anchorDirection = (_canvasAnchor.position - transform.position).normalized;
            Vector3 horizontalMoveDirection = Vector3.Cross(anchorDirection, transform.up).normalized;

            transform.position = Vector3.Lerp(transform.position,
                                        transform.position + horizontalMoveDirection * velocity.x, 
                                        Time.deltaTime * 5f);

            transform.position -= anchorDirection * velocity.z * Time.deltaTime * 3f;
            transform.position += Vector3.up * velocity.y * Time.deltaTime * 3f;
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

        _panelDefaultTransformHash.Add(_productUIPanel.transform, _panelPositionAnchorSet[0]);
        _panelDefaultTransformHash.Add(_shipUIPanel.transform, _panelPositionAnchorSet[1]);
        _panelDefaultTransformHash.Add(_mapUIPanel.transform, _panelPositionAnchorSet[2]);
        _panelDefaultTransformHash.Add(_kingdomUIPanel.transform, _panelPositionAnchorSet[3]);
    }

    private void Start()
    {
        DisableUIPanelEventCallbacks();
    }

    private void Update()
    {
        transform.LookAt(_canvasAnchor);
        transform.Rotate(0, 180, 0);
    }
    #endregion

    #region Private Method Area
    private void ActiveMainPanel()
    {
        _isMainPanelActive = true;

        ActiveSpecificPanel(MainMenuButtonType.Kingdom);
    }

    private void DisableMainPanel()
    {
        _isMainPanelActive = false;

        DisableAllSubPanel();
    }

    private void DisableAllSubPanel()
    {
        var panel = _panelDefaultTransformHash.GetEnumerator();

        while (panel.MoveNext())
            panel.Current.Key.position = panel.Current.Value.position;

        _currentActivePanel = null;
    }
    #endregion
}

public enum MainMenuButtonType
{
    Map = 0,
    Ship = 1,
    Product = 2,
    Kingdom = 3
}