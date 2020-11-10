using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pawn;

[RequireComponent(typeof(Rigidbody), typeof(PawnBaseController))]
public class EnemyUnitController : MonoBehaviour
{
    public void SetMotherShip(PawnBaseController motherPawn, EnemyController motherShip)
    {
        _motherPawn = motherPawn;
        _motherShip = motherShip;

        ProjectionManager.GetInstance().InstantiateWeaponOnSocket(_unitWeapon, _socket)
            .Key.GetComponent<EnemyWeaponController>().SetEnemyController(motherShip);
    }

    [SerializeField]
    private SpaceShipProperty _unitProperty = null;

    [SerializeField]
    private GameObject _socket = null;

    [SerializeField]
    private GameObject _unitWeapon = null;

    private EnemyController _motherShip = null;
    private PawnBaseController _motherPawn = null;

    private Rigidbody _unitPhysics = null;
    private PawnBaseController _unitPawn = null;

    private Transform _targetTransform = null;
    private Quaternion _randomForwardAngle;

    private float ActionStartTimeStamp = 0f;

    private void Awake()
    {
        _unitPawn = GetComponent<PawnBaseController>();
        _unitPhysics = GetComponent<Rigidbody>();
        _randomForwardAngle = Quaternion.Euler(Vector3.back * Random.Range(0f, 360f));
    }

    private void OnEnable()
    {
        ActionStartTimeStamp = Time.time;
    }

    private void Start()
    {

    }

    private void Update()
    {
        _targetTransform = _motherShip.GetTargetTransform(transform);

        if (_targetTransform != null && _motherShip != null)
            UnitMovement();
        else
            OnLoseControl();
    }

    private void UnitMovement()
    {
        Transform target = _motherShip.GetTargetTransform(transform);
        Vector3 direction;

        if (Time.time - ActionStartTimeStamp < 3f)
        {
            direction = Vector3.Slerp(transform.position, target.position, Time.deltaTime) - transform.position;
            direction = (_randomForwardAngle * direction).normalized * 2f;
        }
        else
            direction = (target.position - transform.position).normalized;

        _unitPhysics.AddForce(direction * _unitProperty.MaxMoveSpeed);
    }

    private void OnLoseControl()
    {
        Transform target = ProjectionManager.GetInstance().TableTransform;
        Vector3 direction;

        if (Time.time - ActionStartTimeStamp < 3f)
        {
            direction = Vector3.Slerp(transform.position, target.position, Time.deltaTime) - transform.position;
            direction = (_randomForwardAngle * direction).normalized * 2f;
        }
        else
            direction = (target.position - transform.position).normalized;

        _unitPhysics.AddForce(direction * _unitProperty.MaxMoveSpeed);
    }
}
