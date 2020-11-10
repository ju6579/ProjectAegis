using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pawn;

public class EnemyWeaponController : MonoBehaviour
{
    public void SetEnemyController(EnemyController ec) => _attachedEnemyController = ec;

    [SerializeField]
    private WeaponProperty _weaponProperty;

    [SerializeField]
    private GameObject _muzzle = null;

    private EnemyController _attachedEnemyController = null;

    private bool _isAttack = false;

    private WaitForSeconds _attackWait;
    private WaitForSeconds _reloadWait;

    private void Awake()
    {
        _attackWait = new WaitForSeconds(_weaponProperty.AttackDelay);
        _reloadWait = new WaitForSeconds(_weaponProperty.ReloadDelay);
    }

    private void Update()
    {
        if (_attachedEnemyController != null)
        {
            Transform nextTarget = _attachedEnemyController.GetTargetTransform(transform);

            if (nextTarget != null)
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

    private void ShootTarget()
    {
        ProjectionManager.GetInstance().InstantiateBullet(_weaponProperty.BulletObject,
                                                _muzzle.transform.position,
                                                transform.rotation,
                                                false);
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
