using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pawn;

public class EnemyWeaponController : MonoBehaviour
{
    public void SetEnemyController(EnemyController ec, Transform socket, float weaponRange) 
    {
        _attachedEnemyController = ec;
        _attachedSocketTransform = socket;
        _weaponRange = weaponRange;
    }

    [SerializeField]
    private WeaponProperty _weaponProperty;

    [SerializeField]
    private GameObject _muzzle = null;

    private EnemyController _attachedEnemyController = null;
    private Transform _attachedSocketTransform = null;

    private bool _isAttack = false;

    private WaitForSeconds _attackWait;
    private WaitForSeconds _reloadWait;

    private float _weaponRange = 0f;

    private void Awake()
    {
        _attackWait = new WaitForSeconds(_weaponProperty.AttackDelay);
        _reloadWait = new WaitForSeconds(_weaponProperty.ReloadDelay);
    }

    private void OnEnable()
    {
        _isAttack = false;
    }

    private void Update()
    {
        if(_attachedSocketTransform != null)
        {
            transform.position = _attachedSocketTransform.position;

            if (_attachedEnemyController != null)
            {
                Transform nextTarget = _attachedEnemyController.GetTargetTransform(transform);

                if (nextTarget != null)
                {
                    if(Vector3.Distance(transform.position, nextTarget.position) < _weaponRange)
                    {
                        transform.LookAt(nextTarget);
                        if (!_isAttack)
                        {
                            _isAttack = true;
                            StartCoroutine(_AttackTarget());
                        }
                    }
                }
            }
        }
    }

    private void ShootTarget()
    {
        ProjectionManager.GetInstance().InstantiateBullet(_weaponProperty.BulletObject,
                                                _muzzle.transform.position,
                                                transform.rotation,
                                                false);
    }

    private void OnDisable()
    {
        _attachedEnemyController = null;
        _attachedSocketTransform = null;
    }

    private IEnumerator _AttackTarget()
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.5f));
        for (int i = 0; i < _weaponProperty.AttackCount; i++)
        {
            ShootTarget();
            yield return _attackWait;
        }
        yield return _reloadWait;
        _isAttack = false;
    }
}
