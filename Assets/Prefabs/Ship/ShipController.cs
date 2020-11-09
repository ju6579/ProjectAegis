using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayerKindom;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
    public ShipProperty ShipData => _shipProperty;
    public List<GameObject> SocketList => _sockets;

    public Collider[] Target => _searchedTarget;
    public int TargetCount => _searchedTarget.Length;

    [Serializable]
    public class ShipProperty
    {
        public float ShieldPoint;
        public float ArmorPoint;
        public float MaxMoveSpeed;
        public float Accelation;
        public float ArrivalTime;
    }

    [SerializeField]
    private ShipProperty _shipProperty = null;

    [SerializeField]
    private LayerMask _targetLayerMask = -1;

    [SerializeField]
    private float _searchDistance = 1000f;

    private List<GameObject> _sockets = new List<GameObject>();
    private Collider[] _searchedTarget = new Collider[0];

    private WaitForSeconds _searchRate = new WaitForSeconds(0.333f);

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

    private void ShipMovement()
    {

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

        transform.localPosition = _targetPosition - transform.forward * 0.1f;
        shipPhysics.AddForce(transform.forward * warpPower, ForceMode.Impulse);
    }
}