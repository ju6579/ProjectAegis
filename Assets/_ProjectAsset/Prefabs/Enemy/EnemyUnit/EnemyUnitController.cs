using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pawn;

[RequireComponent(typeof(Rigidbody), typeof(PawnBaseController))]
public class EnemyUnitController : MonoBehaviour
{
    public void InitiallizeUnit(EnemyController motherShip)
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

    [SerializeField]
    private float _searchRate = 0.1f;

    [SerializeField]
    private float _rotateDistance = 200f;

    private List<GameObject> _sockets = new List<GameObject>();
    private List<GameObject> _attachedWeaponList = new List<GameObject>();

    private EnemyController _motherShip = null;
    private Rigidbody _unitPhysics = null;

    private Transform _targetTransform = null;
    private WaitForSeconds _searchWait = null;

    private Quaternion _randomRotation;
    private float _randomAngle = 0f;

    private LayerMask _targetLayerMask = -1;
    private Vector3 _nextMoveDirection = Vector3.zero;
    private float ActionStartTimeStamp = 0f;

    private void Awake()
    {
        _searchWait = new WaitForSeconds(_searchRate);

        _unitPhysics = GetComponent<Rigidbody>();
        
        _randomRotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
        _randomAngle = Random.Range(0f, 360f);

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
        _isSearchTargetRunning = false;
        _targetLayerMask = GlobalGameManager.GetInstance().PlayerShipLayer;
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
        _nextMoveDirection = Vector3.zero;


        if (Time.time - ActionStartTimeStamp < 3f)
            _nextMoveDirection += (_randomRotation * transform.up).normalized;


        if (_motherShip == null)
            _motherShip = EnemyKingdom.GetInstance().RequestNewMotherShip(this);
        else
        {
            if(!_motherShip.gameObject.activeInHierarchy)
                _motherShip = EnemyKingdom.GetInstance().RequestNewMotherShip(this);
            else
                _targetTransform = _motherShip.GetTargetTransform(transform);
        }
           


        if (_targetTransform != null)
            UnitMovement();
        else
            OnTargetNotFound();


        _unitPhysics.AddForce(_nextMoveDirection.normalized * _unitPhysics.mass * _unitProperty.MaxMoveSpeed);
    }

    private void UnitMovement()
    {
        RotateTargetTransform(_targetTransform, _rotateDistance);
    }

    private void OnTargetNotFound()
    {
        if(_motherShip.gameObject.activeSelf)
        {
            // Revolution Around Enemy Mother Ship
            RotateTargetTransform(_motherShip.transform, _rotateDistance);
        }
        else
        {
            // Stay Position And Find Target
            if (!_isSearchTargetRunning) StartCoroutine(_SearchTarget());
        }
    }

    private void RotateTargetTransform(Transform target, float targetDistance)
    {
        Vector3 targetDirection = target.position - transform.position;
        float distanceOffset = targetDirection.magnitude / targetDistance;

        if (distanceOffset < 1) _nextMoveDirection -= targetDirection.normalized;
        else if (distanceOffset > 1) _nextMoveDirection += targetDirection.normalized;

        Quaternion randomRightRotaion = Quaternion.AngleAxis(_randomAngle, transform.right);

        _nextMoveDirection += Vector3.Cross(-targetDirection.normalized, 
                                       randomRightRotaion * transform.forward)
                                       .normalized;
    }

    private bool _isSearchTargetRunning = false;
    private IEnumerator _SearchTarget()
    {
        _isSearchTargetRunning = true;

        Collider[] foundTarget = null;

        while(_targetTransform != null)
        {
            foundTarget = Physics.OverlapSphere(transform.position,
                                           _searchDistance,
                                           _targetLayerMask);

            if (foundTarget.Length > 0)
                _targetTransform = foundTarget[Random.Range(0, foundTarget.Length - 1)].transform;

            yield return _searchWait;
        }

        _isSearchTargetRunning = false;
        yield return null;
    }
}