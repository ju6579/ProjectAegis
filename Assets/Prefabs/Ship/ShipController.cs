using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayerKindom;
using Pawn;

[RequireComponent(typeof(Rigidbody), typeof(PawnBaseController))]
public class ShipController : MonoBehaviour
{
    public SpaceShipProperty ShipData => _shipProperty;
    public List<GameObject> SocketList => _sockets;
    public void MoveShipByDirection(Vector3 inputVector) => _currentInput = inputVector;

    [SerializeField]
    private SpaceShipProperty _shipProperty = null;

    [SerializeField]
    private LayerMask _targetLayerMask = -1;

    [SerializeField]
    private float _searchDistance = 1000f;

    private List<GameObject> _sockets = new List<GameObject>();
    private Collider[] _searchedTarget = new Collider[0];
    private Rigidbody _shipRigidBody = null;

    private WaitForSeconds _searchRate = new WaitForSeconds(0.333f);
    private Vector3 _currentInput = Vector3.zero;

    public void SetWeaponOnSocket(GameObject weapon)
    {
        if(_sockets.Count != 0)
        {
            weapon.transform.SetParent(_sockets[0].transform);
            weapon.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            weapon.transform.rotation = Quaternion.identity;

            _sockets.RemoveAt(0);
        }
    }

    public Transform GetEnemyTarget(Transform weapon)
    {
        if (_searchedTarget.Length > 0)
        {
            Transform nearest = null;
            float minDistance = float.MaxValue;
            float cache;

            for(int i = 0; i < _searchedTarget.Length; i++)
            {
                if (_searchedTarget[i] == null) continue;

                cache = Vector3.Distance(_searchedTarget[i].transform.position, transform.position);
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

    private float warpPower = 100f;

    private Rigidbody shipPhysics = null;
    private Vector3 _targetPosition;
    private float _currentSpeed = 0f;
    private WaitForSeconds _arrivalWait;

    public void WarpToPosition()
    {
        StartCoroutine(_WarpToPosition());
    }

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
        _arrivalWait = new WaitForSeconds(_shipProperty.ArrivalTime);
        warpPower = warpPower * shipPhysics.mass;
    }

    private void Start()
    {
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
            _shipRigidBody.velocity = _currentInput * _shipProperty.MaxMoveSpeed;
            _currentInput = Vector3.zero;
        }
    }

    private IEnumerator _SearchAttackTarget()
    {
        while(this != null)
        {
            _searchedTarget = Physics.OverlapSphere(transform.position, _searchDistance, _targetLayerMask);
            yield return _searchRate;
        }

        yield return null;
    }

    private IEnumerator _WarpToPosition()
    {
        yield return _arrivalWait;
        _targetPosition = PlayerKingdom.GetInstance().NextWarpPoint;

        transform.localPosition = _targetPosition;
        shipPhysics.AddForce(transform.forward * warpPower, ForceMode.Impulse);
    }
}