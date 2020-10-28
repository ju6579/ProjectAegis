using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BaseCameraController : MonoBehaviour
{
    public Ray GetCameraCenterRay() { return _cameraComponent.ViewportPointToRay(_halfRatioVector); }

    private void Awake()
    {
        _cameraComponent = GetComponent<Camera>();
    }

    protected Camera _cameraComponent = null;

    protected static readonly Vector3 _halfRatioVector = new Vector3(0.5f, 0.5f, 0);

    protected Collider GetColliderOnMiddlePoint(float range, LayerMask targetLayer)
    {
        RaycastHit rayInfo = new RaycastHit();

        return Physics.Raycast(GetCameraCenterRay(), out rayInfo, range, targetLayer) ? 
            rayInfo.collider : null;
    }

    protected RaycastHit RaycastOnMiddlePoint(float range, LayerMask targetLayer)
    {
        RaycastHit rayInfo = new RaycastHit();
        Physics.Raycast(GetCameraCenterRay(), out rayInfo, range, targetLayer);

        return rayInfo;
    }
}
