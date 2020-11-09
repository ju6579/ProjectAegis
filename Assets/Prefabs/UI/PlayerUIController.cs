using System;
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
    private GameObject _mainCanvas = null;

    [SerializeField]
    private GameObject _mapUIPanel = null;

    [SerializeField]
    private GameObject _teamUIPanel = null;

    [SerializeField]
    private GameObject _shipUIPanel = null;

    [SerializeField]
    private GameObject _cargoUIPanel = null;

    [SerializeField]
    private GameObject _upgradeUIPanel = null;

    [SerializeField]
    private GameObject _productUIPanel = null;

    [SerializeField]
    private GameObject _dummyCanvas = null;

    private bool _isMainPanelActive = false;
    
    #region Public Method Area
    public void ActiveSpecificPanel(int actionNumber)
    {
        MainMenuButtonType actionType = (MainMenuButtonType)actionNumber;
        DisableAllSubPanel();
        switch (actionType)
        {
            case MainMenuButtonType.Map: _mapUIPanel.SetActive(true); break;
            case MainMenuButtonType.Team: _teamUIPanel.SetActive(true); break;
            case MainMenuButtonType.Ship: _shipUIPanel.SetActive(true); break;
            case MainMenuButtonType.Base: _cargoUIPanel.SetActive(true); break;
            case MainMenuButtonType.Upgrade: _upgradeUIPanel.SetActive(true); break;
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
    }
    #endregion

    #region Private Method Area
    private void ActiveMainPanel()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        _mainCanvas.SetActive(true);
        //DisableAllSubPanel();

        _isMainPanelActive = true;
    }

    private void DisableMainPanel()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        _mainCanvas.SetActive(false);

        _isMainPanelActive = false;
    }

    private void DisableAllSubPanel()
    {
        _mapUIPanel.SetActive(false);
        _teamUIPanel.SetActive(false);
        _shipUIPanel.SetActive(false);
        _cargoUIPanel.SetActive(false);
        _upgradeUIPanel.SetActive(false);
        _productUIPanel.SetActive(false);
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
