using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayerKindom;
using PlayerKindom.PlayerKindomTypes;
using Pawn;
using System.Linq;

[RequireComponent(typeof(Rigidbody), typeof(PawnBaseController))]
public class ShipController : MonoBehaviour
{
    public SpaceShipProperty ShipData => _shipProperty;
    public List<GameObject> SocketList => _sockets;
    public void MoveShipByDirection(Vector3 inputVector) => _currentInput = inputVector;
    public ProductWrapper ShipProduct = null;

    public void SetWeaponOnSocket(ProductionTask weapon, GameObject socket)
    {
        _attachedWeaponHash.Add(socket, weapon);
        _sockets.Remove(socket);
    }

    public void DetachWeaponOnSocket(GameObject socket)
    {
        ProductionTask weapon = _attachedWeaponHash[socket];

        PlayerKingdom.GetInstance().WeaponToCargo(weapon);

        _attachedWeaponHash.Remove(socket);
    }

    public void OnShipToCargo()
    {
        var weaponSet = _attachedWeaponHash.GetEnumerator();

        while (weaponSet.MoveNext())
        {
            PlayerKingdom.GetInstance().WeaponToCargo(weaponSet.Current.Value);
        }

        ReturnWeaponToPool();
    }

    public void OnShipDestroy()
    {
        ReturnWeaponToPool();
    }

    private bool _flag = false;
    public Transform GetEnemyTarget(Transform weapon)
    {
        _flag = !_flag;
        return _flag ? GetTargetByNearest(weapon) : GetTargetByVolley();
    }

    public void WarpToPosition()
    {
        StartCoroutine(_WarpToPosition());
    }

    [SerializeField]
    private SpaceShipProperty _shipProperty = null;

    [SerializeField]
    private float _searchDistance = 1000f;

    [SerializeField]
    private MeshRenderer _dissolveMeshRenderer = null;

    private List<GameObject> _sockets = new List<GameObject>();
    private Dictionary<GameObject, ProductionTask> _attachedWeaponHash = new Dictionary<GameObject, ProductionTask>();
    private List<GameObject> _attachedWeaponInstance = new List<GameObject>();

    private Rigidbody _shipRigidBody = null;

    private WaitForSeconds _searchRate = new WaitForSeconds(0.333f);

    private Material _instancedMaterial = null;
    private MaterialPropertyBlock _materialPropertyHandler;
    private PawnBaseController _pawnController = null;

    private Vector3 _currentInput = Vector3.zero;
    private Vector3 _targetPosition;
    private LayerMask _targetLayerMask = -1;

    private float warpPower = 100f;
    private float _currentSpeed = 0f;

    private void Awake()
    {
        _shipRigidBody = GetComponent<Rigidbody>();
        _pawnController = GetComponent<PawnBaseController>();

        _materialPropertyHandler = new MaterialPropertyBlock();

        _targetPosition = transform.position;
        warpPower = warpPower * _shipRigidBody.mass;

        GameObject anchor = _pawnController.SocketAnchor;
        for (int i = 0; i < anchor.transform.childCount; i++)
        {
            Transform tr = anchor.transform.GetChild(i);

            if (tr.CompareTag("Socket"))
                _sockets.Add(tr.gameObject);
        }
    }

    private void OnEnable()
    {
        var weaponSet = _attachedWeaponHash.GetEnumerator();

        GameObject cache = null;

        while (weaponSet.MoveNext())
        {
            cache = GlobalObjectManager.GetObject(weaponSet.Current.Value.Product);
            cache.GetComponent<WeaponController>().SetAttachedShip(this, weaponSet.Current.Key.transform);

            _attachedWeaponInstance.Add(cache);
        }

        StartCoroutine(_SearchAttackTarget());

        _targetLayerMask = GlobalGameManager.GetInstance().EnemyShipLayer;
    }

    private void Update()
    {
        ShipMovement();
    }

    private void ShipMovement()
    {
        if(_currentInput != Vector3.zero)
        {
            _shipRigidBody.AddForce(_currentInput * _shipProperty.MaxMoveSpeed * _shipRigidBody.mass);
            _currentInput = Vector3.zero;
        }
    }

    private void ReturnWeaponToPool()
    {
        _attachedWeaponInstance.ForEach((GameObject instance) =>
        {
            GlobalObjectManager.ReturnToObjectPool(instance);
        });
        
        _attachedWeaponInstance.Clear();
    }

    private IEnumerator _SearchAttackTarget()
    {
        while(this != null)
        {
            _searchedTarget = Physics.OverlapSphere(transform.position, 
                                               _searchDistance, 
                                               _targetLayerMask);

            _searchedQueue.Clear();
            _searchedTarget.ToList().ForEach((Collider co) => _searchedQueue.Enqueue(co));

            yield return _searchRate;
        }

        yield return null;
    }

    private IEnumerator _WarpToPosition()
    {
        yield return new WaitForSeconds(_shipProperty.ArrivalTime);

        _targetPosition = PlayerKingdom.GetInstance().NextWarpPoint;

        transform.localPosition = _targetPosition;
        _shipRigidBody.AddForce(transform.forward * warpPower, ForceMode.Impulse);

        _pawnController.PlayHideToShowEffect();
    }





    #region Experimental
    private Queue<Collider> _searchedQueue = new Queue<Collider>();
    private Collider[] _searchedTarget = new Collider[0];

    private Transform GetTargetByVolley()
    {
        Transform target = null;
        Collider cache;

        if (_searchedQueue.Count > 0)
        {
            cache = _searchedQueue.Dequeue();
            if (cache != null)
                target = cache.transform;

            _searchedQueue.Enqueue(cache);
        }

        return target;
    }

    private Transform GetTargetByNearest(Transform weapon)
    {
        if (_searchedTarget.Length > 0)
        {
            Transform nearest = null;
            float minDistance = float.MaxValue;
            float cache;

            for (int i = 0; i < _searchedTarget.Length; i++)
            {
                if (_searchedTarget[i] == null) continue;

                cache = Vector3.Distance(_searchedTarget[i].transform.position, weapon.position);
                if (minDistance > cache)
                {
                    minDistance = cache;
                    nearest = _searchedTarget[i].transform;
                }
            }

            return nearest;
        }

        return null;
    }

    #endregion

}