using HTC.UnityPlugin.Vive;
using HTC.UnityPlugin.VRModuleManagement;
using Pawn;
using PlayerKindom;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Dpad Menu UI Input
    [SerializeField]
    private ControllerButton _mainMenuUIChangeButton1 = ControllerButton.DPadUp;
    [SerializeField]
    private MainMenuButtonType _button1MappingUI = MainMenuButtonType.Map;

    [SerializeField]
    private ControllerButton _mainMenuUIChangeButton2 = ControllerButton.DPadRight;
    [SerializeField]
    private MainMenuButtonType _button2MappingUI = MainMenuButtonType.Product;

    [SerializeField]
    private ControllerButton _mainMenuUIChangeButton3 = ControllerButton.DPadDown;
    [SerializeField]
    private MainMenuButtonType _button3MappingUI = MainMenuButtonType.Ship;

    [SerializeField]
    private ControllerButton _mainMenuUIChangeButton4 = ControllerButton.DPadLeft;
    [SerializeField]
    private MainMenuButtonType _button4MappingUI = MainMenuButtonType.Kingdom;

    #endregion

    [SerializeField]
    private ControllerButton _projectionControlInputButton = ControllerButton.Trigger;

    [SerializeField]
    private ControllerButton _activeMainUIInputButton = ControllerButton.Menu;

    [SerializeField]
    private ControllerButton _moveMainUIInputButton = ControllerButton.Grip;

    [SerializeField]
    private Transform _rightHand = null;

    [SerializeField]
    private Transform _leftHand = null;

    [SerializeField]
    private Transform _leftPointerPoseTracker = null;

    [SerializeField]
    private Transform _rightPointerPoseTracker = null;

    [SerializeField]
    private LayerMask _shipProjectionLayer = -1;

    private RaycastHit _rayInfoLeft = new RaycastHit();
    private Ray _rayCacheLeft = new Ray();

    private RaycastHit _rayInfoRight = new RaycastHit();
    private Ray _rayCacheRight = new Ray();

    private ProjectPositionTracker _targetShipProjectorLeft = null;
    private ProjectPositionTracker _targetShipProjectorRight = null;

    private uint _rightHandDeviceIndex = 0;
    private uint _leftHandDeviceIndex = 0;

    private readonly uint DeviceNotFound = VRModule.INVALID_DEVICE_INDEX;

    private void Update()
    {
        _rightHandDeviceIndex = ViveRole.GetDeviceIndex(HandRole.RightHand);
        _leftHandDeviceIndex = ViveRole.GetDeviceIndex(HandRole.LeftHand);

        if (_rightHandDeviceIndex != DeviceNotFound) RightHandAction();
        if (_leftHandDeviceIndex != DeviceNotFound) LeftHandAction();
    }

    private void RightHandAction()
    {
        InputRightHandProjectionControl(_projectionControlInputButton);
        ActiveMainUIPanelInput(HandRole.RightHand, _activeMainUIInputButton);
        MoveMainUIPanelInput(HandRole.RightHand, _moveMainUIInputButton, _rightHandDeviceIndex);
        ActiveSpecificMenuInput(HandRole.RightHand);
    }

    private void LeftHandAction()
    {
        InputLeftHandProjectionControl(_projectionControlInputButton);
    }

    private void ActiveMainUIPanelInput(HandRole hand, ControllerButton button)
    {
        if(ViveInput.GetPressDown(hand, button))
        {
            PlayerUIController.GetInstance().MainUIOnOffAction();
        }
    }

    private void MoveMainUIPanelInput(HandRole hand, ControllerButton button, uint deviceIndex)
    {
        if(ViveInput.GetPress(hand, button))
        {
            var deviceState = VRModule.GetDeviceState(deviceIndex);

            PlayerUIController.GetInstance().MoveMainUIPanel(deviceState.velocity);
        }
    }

    private void RecallShipToKingdom(ProjectPositionTracker ppt)
    {
        PlayerKingdom.GetInstance().ShipToCargo(ppt.TargetShipController.ShipProduct);
    }

    private void ActiveSpecificMenuInput(HandRole hand)
    {
        if (ViveInput.GetPressDown(hand, _mainMenuUIChangeButton1))
            PlayerUIController.GetInstance().ActiveSpecificPanel(_button1MappingUI);

        if (ViveInput.GetPressDown(hand, _mainMenuUIChangeButton2))
            PlayerUIController.GetInstance().ActiveSpecificPanel(_button2MappingUI);

        if (ViveInput.GetPressDown(hand, _mainMenuUIChangeButton3))
            PlayerUIController.GetInstance().ActiveSpecificPanel(_button3MappingUI);

        if (ViveInput.GetPressDown(hand, _mainMenuUIChangeButton4))
            PlayerUIController.GetInstance().ActiveSpecificPanel(_button4MappingUI);
    }

    #region Projection Control Input Function
    private void InputRightHandProjectionControl(ControllerButton button)
    {
        if (ViveInput.GetPressDown(HandRole.RightHand, button))
        {
            _rayCacheRight.origin = _rightPointerPoseTracker.position;
            _rayCacheRight.direction = _rightPointerPoseTracker.forward;

            if (Physics.Raycast(_rayCacheRight, out _rayInfoRight, 20f, _shipProjectionLayer))
            {
                if (_rayInfoRight.collider != null)
                {
                    if (_rayInfoRight.collider.CompareTag("Pawn"))
                    {
                        ProjectPositionTracker ppt
                            = _rayInfoRight.collider.gameObject.GetComponentInParent<ProjectPositionTracker>();

                        if (ppt.ProjectedType == PawnType.SpaceShip)
                        {
                            _targetShipProjectorRight = ppt;
                        }
                    }
                }
            }
        }

        if (ViveInput.GetPress(HandRole.RightHand, button))
        {
            if (_targetShipProjectorRight != null)
            {
                var deviceState = VRModule.GetDeviceState(_rightHandDeviceIndex);

                _targetShipProjectorRight.InputShipControl(deviceState.velocity.normalized);

                if (deviceState.velocity.z < 0 && deviceState.velocity.magnitude > 2.5f)
                {
                    Debug.Log("Recall Ship");
                    RecallShipToKingdom(_targetShipProjectorRight);
                }
            }
        }
        else
            _targetShipProjectorRight = null;
    }

    private void InputLeftHandProjectionControl(ControllerButton button)
    {
        if (ViveInput.GetPressDown(HandRole.LeftHand, button))
        {
            _rayCacheLeft.origin = _leftPointerPoseTracker.position;
            _rayCacheLeft.direction = _leftPointerPoseTracker.forward;

            if (Physics.Raycast(_rayCacheLeft, out _rayInfoLeft, 20f, _shipProjectionLayer))
            {
                if (_rayInfoLeft.collider != null)
                {
                    if (_rayInfoLeft.collider.CompareTag("Pawn"))
                    {
                        ProjectPositionTracker ppt
                            = _rayInfoLeft.collider.gameObject.GetComponentInParent<ProjectPositionTracker>();

                        if (ppt.ProjectedType == PawnType.SpaceShip)
                        {
                            _targetShipProjectorLeft = ppt;
                        }
                    }
                }
            }
        }

        if (ViveInput.GetPress(HandRole.LeftHand, button))
        {
            if (_targetShipProjectorLeft != null)
            {
                var deviceState = VRModule.GetDeviceState(_leftHandDeviceIndex);

                _targetShipProjectorLeft.InputShipControl(deviceState.velocity.normalized);

                if (deviceState.velocity.z < 0 && deviceState.velocity.magnitude > 2.5f)
                {
                    Debug.Log("Recall Ship");
                    RecallShipToKingdom(_targetShipProjectorLeft);
                }
            }
        }
        else
            _targetShipProjectorLeft = null;
    }
    #endregion
}
