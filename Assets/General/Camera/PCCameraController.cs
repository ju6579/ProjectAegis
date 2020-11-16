using UnityEngine;

using Pawn;

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
    private ProjectPositionTracker _targetShipProjector = null;
    //
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit rayInfo = RaycastOnMiddlePoint(10f);
            if (rayInfo.collider != null)
            {
                Debug.Log(rayInfo.collider.tag);
                if (rayInfo.collider.CompareTag("Pawn"))
                {
                    Debug.Log("temp3");

                    ProjectPositionTracker ppt 
                        = rayInfo.collider.gameObject.GetComponentInParent<ProjectPositionTracker>();
                    
                    if(ppt.ProjectedType == PawnType.SpaceShip)
                    {
                        _targetShipProjector = ppt;
                    }
                }
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if(_targetShipProjector != null)
            {
                Vector3 inputVector = Vector3.zero;
                inputVector += Input.GetAxis("Vertical") * Vector3.forward;
                inputVector += Input.GetAxis("Horizontal") * Vector3.right;
                inputVector += Input.GetAxis("Height") * Vector3.up;

                _targetShipProjector.InputShipControl(inputVector.normalized);
            }    
        }
        else
            _targetShipProjector = null;
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
