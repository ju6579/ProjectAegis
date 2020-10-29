using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ShipController : MonoBehaviour
{
    [SerializeField]
    private float maxMoveSpeed = 20f;

    [SerializeField]
    private float accelation = 1f;

    [SerializeField]
    private float arrivalTime = 1f;

    [SerializeField]
    private float warpPower = 1f;

    private Rigidbody shipPhysics = null;
    private Vector3 _targetPosition;
    private float _currentSpeed = 0f;

    private WaitForSeconds _arrivalWait;

    private void Awake()
    {
        shipPhysics = GetComponent<Rigidbody>();

        _targetPosition = transform.position;
        _arrivalWait = new WaitForSeconds(arrivalTime);
        warpPower = warpPower * shipPhysics.mass;
    }

    private void OnEnable()
    {
        StartCoroutine(_WarpToPosition());
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