using System;
using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Serializable]
    public class WeaponProperty
    {
        public GameObject BulletObject;
        public int AttackCount;
        public float AttackDelay;
        public float ReloadDelay;
    }

    public WeaponProperty WeaponData => _weaponProperty;

    [SerializeField]
    private WeaponProperty _weaponProperty;

    [SerializeField]
    private GameObject _muzzle = null;

    private WaitForSeconds _attackWait;
    private WaitForSeconds _reloadWait;

    private bool _isAttack = false;

    private GameObject _attackTarget = null;

    private void Awake()
    {
        _attackWait = new WaitForSeconds(_weaponProperty.AttackDelay);
        _reloadWait = new WaitForSeconds(_weaponProperty.ReloadDelay);
    }

    private void Update()
    {
        if (_attackTarget != null)
        {
            if (!_isAttack)
            {
                _isAttack = true;
                StartCoroutine(_AttackTarget());
            }

            transform.LookAt(_attackTarget.transform);
        }
        else
            _attackTarget = EnemyKingdom.GetInstance().GetCurrentEnemy();
    }

    private void ShootTarget()
    {
        ProjectionManager.GetInstance().InstantiateBullet(_weaponProperty.BulletObject,
                                                _muzzle.transform.position,
                                                transform.rotation);
    }

    private IEnumerator _AttackTarget()
    {
        for(int i = 0; i < _weaponProperty.AttackCount; i++)
        {
            ShootTarget();
            yield return _attackWait;
        }
        yield return _reloadWait;
        _isAttack = false;
    }
}
