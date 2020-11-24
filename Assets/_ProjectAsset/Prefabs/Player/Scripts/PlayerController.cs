using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Vive;
using HTC.UnityPlugin.VRModuleManagement;
using Pawn;

using PlayerKindom;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform _rightHand = null;

    [SerializeField]
    private Transform _leftHand = null;

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
        if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))
        {
            _rayCacheRight.origin = _rightHand.position;
            _rayCacheRight.direction = _rightHand.forward;

            if (Physics.Raycast(_rayCacheRight, out _rayInfoRight, 20f))
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

        if (ViveInput.GetPress(HandRole.RightHand, ControllerButton.Trigger))
        {
            if (_targetShipProjectorRight != null)
            {
                var deviceState = VRModule.GetDeviceState(_rightHandDeviceIndex);

                _targetShipProjectorRight.InputShipControl(deviceState.velocity.normalized);

                if(deviceState.velocity.z < 0 && deviceState.velocity.magnitude > 2.5f)
                {
                    Debug.Log("Recall Ship");
                    RecallShipToKingdom(_targetShipProjectorRight);
                }
            }
        }
        else
            _targetShipProjectorRight = null;
    }

    private void LeftHandAction()
    {
        if (ViveInput.GetPressDown(HandRole.LeftHand, ControllerButton.Trigger))
        {
            _rayCacheLeft.origin = _leftHand.position;
            _rayCacheLeft.direction = _leftHand.forward;

            if (Physics.Raycast(_rayCacheLeft, out _rayInfoLeft, 20f))
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

        if (ViveInput.GetPress(HandRole.LeftHand, ControllerButton.Trigger))
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

    private void RecallShipToKingdom(ProjectPositionTracker ppt)
    {
        PlayerKingdom.GetInstance().ShipToCargo(ppt.TargetShipController.ShipProduct);
    }
}
