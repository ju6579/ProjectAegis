using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
    public ShipProperty ShipData => _shipProperty;
    public List<GameObject> SocketList => _sockets;

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

    private List<GameObject> _sockets = new List<GameObject>();

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
#pragma warning disable IDE0044 // 읽기 전용 한정자 추가
#pragma warning disable IDE0051 // 사용되지 않는 private 멤버 제거
    private float _currentSpeed = 0f;
#pragma warning restore IDE0051 // 사용되지 않는 private 멤버 제거
#pragma warning restore IDE0044 // 읽기 전용 한정자 추가

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

#pragma warning disable IDE0051 // 사용되지 않는 private 멤버 제거
    private void ShipMovement()
#pragma warning restore IDE0051 // 사용되지 않는 private 멤버 제거
    {

    }

    private IEnumerator _WarpToPosition()
    {
        yield return _arrivalWait;
        GameObject go = GlobalObjectManager.GetInstance().GetShipAnchor();
        _targetPosition = go.transform.localPosition;

        transform.localPosition = _targetPosition - transform.forward * 0.1f;
        shipPhysics.AddForce(transform.forward * warpPower, ForceMode.Impulse);
    }
}