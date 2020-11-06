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

    public static PlayerKingdom.ProductionTask SelectedWeaponTask = null;
    public static PlayerKingdom.ProductionTask SelectedShipTask = null;

    public static PlayerKingdom.ProductWrapper SelectedShip = null;
    public static UISocketContentsProperty SelectedUISocketContents = null;

    public static Button SelectedSocketButton = null;
    public static Button SelectedWeaponTaskButton = null;
    public static Button SelectedShipTaskButton = null;

    public static void ClearSelect()
    {
        SelectedWeaponTask = null;
        SelectedShipTask = null;
        SelectedShip = null;

        SelectedUISocketContents = null;

        SelectedSocketButton = null;
        SelectedWeaponTaskButton = null;
        SelectedShipTask = null;
    }

    [SerializeField]
    private GameObject _mainCanvas = null;

    [SerializeField]
    private GameObject _screenSaver = null;

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

    private bool _isMainPanelActive = false;
    
    #region Public Method Area
    public void ActiveSpecificPanel(int actionNumber)
    {
        MainMenuButtonType actionType = (MainMenuButtonType)actionNumber;
        DisableAllSubPanel();
        DisableScreenSaver();
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
        ClearSelect();
        base.Awake();
        ActiveUIPanelEventCallbacks += ActiveMainPanel;
        DisableUIPanelEventCallbacks += DisableMainPanel;
    }

    private void Start()
    {
        DisableUIPanelEventCallbacks();
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
        DisableAllSubPanel();
        ActiveScreenSaver();

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
        ClearSelect();

        _mapUIPanel.SetActive(false);
        _teamUIPanel.SetActive(false);
        _shipUIPanel.SetActive(false);
        _cargoUIPanel.SetActive(false);
        _upgradeUIPanel.SetActive(false);
        _productUIPanel.SetActive(false);
    }

    private void ActiveScreenSaver() { _screenSaver.SetActive(true); }
    private void DisableScreenSaver() { _screenSaver.SetActive(false); }
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
