using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private float arrivalTime = 3f;

    [SerializeField]
    private float warpPower = 1f;

    private WaitForSeconds _arrivalWait;
    private Vector3 _targetPosition = Vector3.zero;
    private Rigidbody _enemyPhysics = null;

    private void Awake()
    {
        _arrivalWait = new WaitForSeconds(arrivalTime);
        _enemyPhysics = GetComponent<Rigidbody>();
        warpPower = warpPower * _enemyPhysics.mass;
    }

    private void Start()
    {
        _targetPosition = EnemyKingdom.GetInstance().enemyGate.localPosition;
        StartCoroutine(_WarpToPosition());
    }

    private IEnumerator _WarpToPosition()
    {
        yield return _arrivalWait;

        transform.localPosition = _targetPosition + transform.forward * 0.1f;
        _enemyPhysics.AddForce(transform.forward * warpPower, ForceMode.Impulse);
    }
}
