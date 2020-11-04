using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
    [Serializable]
    private struct ShipProperty
    {
        public float MaxMoveSpeed;
        public float Accelation;
        public float ArrivalTime;
    }

    [SerializeField]
    private ShipProperty _shipProperty;

    [SerializeField]
    private List<GameObject> _sockets;
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
        shipPhysics = GetComponent<Rigidbody>();

        _targetPosition = transform.position;
        _arrivalWait = new WaitForSeconds(_shipProperty.ArrivalTime);
        warpPower = warpPower * shipPhysics.mass;
    }

    private void OnEnable()
    {
        
    }

    private void Update()
    {

    }

    private void ShipMovement()
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