using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pawn;

[RequireComponent(typeof(Rigidbody), typeof(PawnBaseController))]
public class EnemyController : MonoBehaviour
{
    public PawnBaseController Pawn => _enemyPawn;

    public Transform GetTargetTransform(Transform callerTransform)
    {
        if (_searchedTarget.Length > 0)
        {
            if(_searchedTarget[0] != null)
                return _searchedTarget[0].transform;
        }

        return null;
    }

    [SerializeField]
    private SpaceShipProperty _enemyProperties= null;

    [SerializeField]
    private float _warpPower = 1f;

    [SerializeField]
    private LayerMask _targetLayerMask = -1;

    [SerializeField]
    private float _searchDistance = 1000f;

    [SerializeField]
    private List<GameObject> _unitFactory = null;

    [SerializeField]
    private List<GameObject> _enemyWeaponFactory = null;

    private List<GameObject> _sockets = new List<GameObject>();

    private Rigidbody _enemyPhysics = null;
    private PawnBaseController _enemyPawn = null;

    private WaitForSeconds _arrivalWait;
    private Vector3 _targetPosition = Vector3.zero;
    private Collider[] _searchedTarget = new Collider[0];
    private List<GameObject> _attachedWeaponList = new List<GameObject>();

    private WaitForSeconds _searchRate = new WaitForSeconds(0.5f);

    private void Awake()
    {
        _enemyPhysics = GetComponent<Rigidbody>();
        _enemyPawn = GetComponent<PawnBaseController>();

        _arrivalWait = new WaitForSeconds(_enemyProperties.ArrivalTime);
        _warpPower = _warpPower * _enemyPhysics.mass;

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
        _sockets.ForEach((GameObject go) =>
        {
            GameObject weapon = ProjectionManager.GetInstance().InstantiateWeapon(_enemyWeaponFactory[0]).Key.gameObject;

            weapon.GetComponent<EnemyWeaponController>().SetEnemyController(this, go.transform);
            _attachedWeaponList.Add(weapon);
        });

        _targetPosition = EnemyKingdom.GetInstance().NextWarpPoint;
        StartCoroutine(_WarpToPosition());
    }

    private void OnDisable()
    {
        if(_attachedWeaponList.Count > 0)
        {
            _attachedWeaponList.ForEach((GameObject go) => GlobalObjectManager.ReturnToObjectPool(go));
            _attachedWeaponList.Clear();
        }
    }

    private void Update()
    {
        EnemyMovement();
    }

    private IEnumerator _WarpToPosition()
    {
        yield return _arrivalWait;

        transform.localPosition = _targetPosition;
        _enemyPhysics.AddForce(transform.forward * _warpPower * 100f, ForceMode.Impulse);

        StartCoroutine(_SearchTarget());
        StartCoroutine(_CallShip());
    }

    private IEnumerator _SearchTarget()
    {
        while (this != null)
        {
            _searchedTarget = Physics.OverlapSphere(transform.position, _searchDistance, _targetLayerMask);
            
            yield return _searchRate;
        }

        yield return null;
    }

    private IEnumerator _CallShip()
    {
        WaitForSeconds callrate = new WaitForSeconds(10f);

        while (this != null)
        {
            EnemyKingdom.GetInstance().RequestCreateUnit(_unitFactory[0], this);

            yield return callrate;
        }
    }

    private void EnemyMovement()
    {
        if (!_enemyPawn.bIsAttack)
        {
            _enemyPhysics.AddForce(transform.forward * _enemyProperties.MaxMoveSpeed * _enemyPhysics.mass);
        }
    }
}
