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

    public void SetWeaponOnSocket(ProductWrapper weapon, GameObject socket)
    {
        if(weapon.Instance == null)
        {
            _attachedWeaponHash.Add(socket, weapon);
            _sockets.Remove(socket);
        }
    }

    public void DetachWeaponOnSocket(GameObject socket)
    {
        ProductWrapper weapon = _attachedWeaponHash[socket];

        PlayerKingdom.GetInstance().WeaponToCargo(weapon);
        _attachedWeaponHash.Remove(socket);
    }

    public void OnShipDestroy()
    {
        var weaponSet = _attachedWeaponHash.GetEnumerator();

        while (weaponSet.MoveNext())
        {
            PlayerKingdom.GetInstance().ProductDestoryed(weaponSet.Current.Value);
        }
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
    private LayerMask _targetLayerMask = -1;

    [SerializeField]
    private float _searchDistance = 1000f;

    private List<GameObject> _sockets = new List<GameObject>();
    private Rigidbody _shipRigidBody = null;
    private Dictionary<GameObject, ProductWrapper> _attachedWeaponHash = new Dictionary<GameObject, ProductWrapper>();

    private WaitForSeconds _searchRate = new WaitForSeconds(0.333f);
    private Vector3 _currentInput = Vector3.zero;

    #region Experimental
    private Queue<Collider> _searchedQueue = new Queue<Collider>();
    private Collider[] _searchedTarget = new Collider[0];

    private Transform GetTargetByVolley()
    {
        Transform target = null;
        Collider cache;

        if(_searchedQueue.Count > 0)
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
    private float warpPower = 100f;

    private Rigidbody shipPhysics = null;
    private Vector3 _targetPosition;
    private float _currentSpeed = 0f;

    private void Awake()
    {
        _shipRigidBody = GetComponent<Rigidbody>();
        GameObject anchor = GetComponent<PawnBaseController>().SocketAnchor;
        for (int i = 0; i < anchor.transform.childCount; i++)
        {
            Transform tr = anchor.transform.GetChild(i);

            if (tr.CompareTag("Socket"))
                _sockets.Add(tr.gameObject);
        }

        shipPhysics = GetComponent<Rigidbody>();

        _targetPosition = transform.position;
        warpPower = warpPower * shipPhysics.mass;
    }

    private void OnEnable()
    {
        var weaponSet = _attachedWeaponHash.GetEnumerator();

        while (weaponSet.MoveNext())
        {
            weaponSet.Current.Value.ActiveProductInstance().GetComponent<WeaponController>()
                .SetAttachedShip(this, weaponSet.Current.Key.transform);
        }

        StartCoroutine(_SearchAttackTarget());
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

    private IEnumerator _SearchAttackTarget()
    {
        while(this != null)
        {
            _searchedTarget = Physics.OverlapSphere(transform.position, _searchDistance, _targetLayerMask);

            _searchedQueue.Clear();
            _searchedTarget.ToList().ForEach((Collider co) => _searchedQueue.Enqueue(co));

            yield return _searchRate;
        }

        yield return null;
    }

    private IEnumerator _WarpToPosition()
    {
        yield return new WaitForSeconds(_shipProperty.ArrivalTime + UnityEngine.Random.Range(0.5f, 1.5f));
        _targetPosition = PlayerKingdom.GetInstance().NextWarpPoint;

        transform.localPosition = _targetPosition;
        shipPhysics.AddForce(transform.forward * warpPower, ForceMode.Impulse);
    }
}