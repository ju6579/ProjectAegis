using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : Singleton<PlayerUIController>
{
    public delegate void UIActiveEvent();
    public static UIActiveEvent ActiveUIPanelEventCallbacks;
    public static UIActiveEvent DisableUIPanelEventCallbacks;

    protected override void OnDestroy()
    {
        base.OnDestroy();

        ActiveUIPanelEventCallbacks = null;
        DisableUIPanelEventCallbacks = null;
    }

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
    private float _distanceMove = 0f;

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

    private float _horizontalMoveMax = 0.7f;
    private float _verticalMoveMax = 0.4f;
    private float _distanceMoveMax = 0.6f;
    public void MoveMainUIPanel(Vector3 velocity)
    {
        if (_isMainPanelActive)
        {

            _horizontalMove += velocity.x * Time.deltaTime * 3f;
            _horizontalMove = Mathf.Clamp(_horizontalMove, -_horizontalMoveMax, _horizontalMoveMax);

            _verticalMove += velocity.y * Time.deltaTime;
            _verticalMove = Mathf.Clamp(_verticalMove, -_verticalMoveMax, _verticalMoveMax);

            _distanceMove += velocity.z * Time.deltaTime;
            _distanceMove = Mathf.Clamp(_distanceMove, -_distanceMoveMax, _distanceMoveMax);

            Vector3 anchorDirection = (_canvasAnchor.position - transform.position).normalized;

            if (Mathf.Abs(_horizontalMove) < _horizontalMoveMax)
            {
                Vector3 horizontalMoveDirection = Vector3.Cross(anchorDirection, transform.up).normalized;

                //transform.position = Vector3.Lerp(transform.position,
                //            transform.position + horizontalMoveDirection * velocity.x,
                //            Time.deltaTime * 5f);

                transform.position += transform.right * velocity.x * Time.deltaTime * 3f;
            }

            if (Mathf.Abs(_distanceMove) < _distanceMoveMax)
            {
                transform.position -= anchorDirection * velocity.z * Time.deltaTime;
                //transform.position -= anchorDirection * velocity.z * Time.deltaTime * 3f;
            }

            if(Mathf.Abs(_verticalMove) < _verticalMoveMax)
            {
                transform.position += Vector3.up * velocity.y * Time.deltaTime;
                //transform.position += Vector3.up * velocity.y * Time.deltaTime * 3f;
            }
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
        ActiveUIPanelEventCallbacks();
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