using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCCameraController : BaseCameraController
{
    public string InteractTag = "";
    public string SocketTag = "";

    public float CameraRotateSpeed = 90f;
    public float HorizontalRotateRange = 45f;
    public float VerticalRotateRange = 45f;

    private float _verticalRotate = 0f;
    private float _horizontalRotate = 0f;

    private WaitForEndOfFrame _waitRate = new WaitForEndOfFrame();

    private void OnEnable()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        _verticalRotate = euler.x;
        _horizontalRotate = euler.y;
    }

    //
    public GameObject TestShip = null;
    public GameObject TestWeapon = null;
    //
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit rayInfo = RaycastOnMiddlePoint(5f);
            
            if(rayInfo.collider != null)
            {
                if(rayInfo.collider.tag == InteractTag)
                {
                    ObjectButtonAction oba = rayInfo.collider.GetComponent<ObjectButtonAction>();
                    if (oba != null) oba.OnButtonInteract(TestShip);
                }
                else if(rayInfo.collider.tag == SocketTag)
                {
                    
                }
            }
        }
    }

    private void LateUpdate()
    {
        _verticalRotate -= Input.GetAxis("Mouse Y") * Time.deltaTime * CameraRotateSpeed;
        _horizontalRotate += Input.GetAxis("Mouse X") * Time.deltaTime * CameraRotateSpeed;

        _verticalRotate = Mathf.Clamp(_verticalRotate, -VerticalRotateRange, VerticalRotateRange);
        _horizontalRotate = Mathf.Clamp(_horizontalRotate, -HorizontalRotateRange, HorizontalRotateRange);

        Vector3 targetRotationEuler = transform.rotation.eulerAngles;
        targetRotationEuler.x = _verticalRotate;
        targetRotationEuler.y = _horizontalRotate;

        transform.rotation = Quaternion.Euler(targetRotationEuler);
    }
}
