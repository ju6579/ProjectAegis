using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCCameraController : BaseCameraController
{
    public float CameraRotateSpeed = 90f;
    public float HorizontalRotateRange = 45f;
    public float VerticalRotateRange = 45f;

    public LayerMask InteractionLayer = -1;

    private float _verticalRotate = 0f;
    private float _horizontalRotate = 0f;

    private WaitForEndOfFrame _waitRate = new WaitForEndOfFrame();

    private void OnEnable()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        _verticalRotate = euler.x;
        _horizontalRotate = euler.y;

        Cursor.lockState = CursorLockMode.Locked;
    }

    //
    public GameObject TestPrefab = null;
    //
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit rayInfo = RaycastOnMiddlePoint(5f, InteractionLayer);

            if(rayInfo.collider != null)
            {
                ProjectionManager.GetInstance().InstantiateToTable(TestPrefab,
                                                         rayInfo.point,
                                                         Quaternion.identity);
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

    private IEnumerator _PickupObject(Transform picked)
    {
        while (Input.GetKey(KeyCode.Mouse0))
        {
            picked.position += Vector3.up * Time.deltaTime * Input.GetAxis("Height");
            picked.position += Vector3.right * Time.deltaTime * Input.GetAxis("Horizontal");
            picked.position += Vector3.forward * Time.deltaTime * Input.GetAxis("Vertical");
            yield return _waitRate;
        }
    }
}
