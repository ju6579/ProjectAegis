using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pawn;

[RequireComponent(typeof(Rigidbody), typeof(PawnBaseController))]
public class EnemyUnitController : MonoBehaviour
{
    public void SetMotherShip(EnemyController motherShip)
    {
        _motherShip = motherShip;

        _sockets.ForEach((GameObject go) =>
        {
            GameObject weapon = ProjectionManager.GetInstance().InstantiateWeapon(_unitWeapon).Key.gameObject;

            weapon.GetComponent<EnemyWeaponController>().SetEnemyController(motherShip, go.transform, _searchDistance);
            _attachedWeaponList.Add(weapon);
        });
    }

    [SerializeField]
    private SpaceShipProperty _unitProperty = null;

    [SerializeField]
    private GameObject _unitWeapon = null;

    [SerializeField]
    private float _searchDistance = 500f;

    private EnemyController _motherShip = null;

    private Rigidbody _unitPhysics = null;

    private Transform _targetTransform = null;
    private Quaternion _randomForwardAngle;

    private float ActionStartTimeStamp = 0f;

    private List<GameObject> _sockets = new List<GameObject>();
    private List<GameObject> _attachedWeaponList = new List<GameObject>();

    private void Awake()
    {
        _unitPhysics = GetComponent<Rigidbody>();
        _randomForwardAngle = Quaternion.Euler(Vector3.back * Random.Range(0f, 360f));

        GameObject anchor = GetComponent<PawnBaseController>().SocketAnchor;
        for (int i = 0; i < anchor.transform.childCount; i++)
        {
            Transform tr = anchor.transform.GetChild(i);

            if (tr.CompareTag("Socket"))
                _sockets.Add(tr.gameObject);
        }
    }

    private void OnEnable()
    {
        ActionStartTimeStamp = Time.time;
    }

    private void OnDisable()
    {
        if (_attachedWeaponList.Count > 0)
        {
            _attachedWeaponList.ForEach((GameObject go) => GlobalObjectManager.ReturnToObjectPool(go));
            _attachedWeaponList.Clear();
        }
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

        if(target != null)
        {
            Vector3 direction;

            if(Vector3.Distance(transform.position, target.position) > 200f)
            {
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
