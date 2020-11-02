using System;
using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Serializable]
    private struct WeaponProperty
    {
        public GameObject BulletObject;
        public int AttackCount;
        public float AttackDelay;
        public float ReloadDelay;
    }

    [SerializeField]
    private WeaponProperty weaponProperty;

    [SerializeField]
    private GameObject muzzle = null;

    private WaitForSeconds _attackWait;
    private WaitForSeconds _reloadWait;

    private bool _isAttack = false;

    private GameObject _attackTarget = null;

    private void Awake()
    {
        _attackWait = new WaitForSeconds(weaponProperty.AttackDelay);
        _reloadWait = new WaitForSeconds(weaponProperty.ReloadDelay);
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
        ProjectionManager.GetInstance().InstantiateBullet(weaponProperty.BulletObject,
                                                muzzle.transform.position,
                                                transform.rotation);
    }

    private IEnumerator _AttackTarget()
    {
        for(int i = 0; i < weaponProperty.AttackCount; i++)
        {
            ShootTarget();
            yield return _attackWait;
        }
        yield return _reloadWait;
        _isAttack = false;
    }
}
