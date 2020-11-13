using System;
using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public void SetAttachedShip(ShipController ship, Transform socket) 
    {
        _attachedShip = ship;
        _attachedSocketTransform = socket;
    }

    public void DetachWeaponToShip() 
    {
        _attachedSocketTransform = null;
        _attachedShip = null;

        GlobalObjectManager.ReturnToObjectPool(gameObject);
    }

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

    private bool _isAttack = false;

    private ShipController _attachedShip = null;
    private Transform _attachedSocketTransform = null;

    private WaitForSeconds _attackWait;
    private WaitForSeconds _reloadWait;

    private void Awake()
    {
        _attackWait = new WaitForSeconds(_weaponProperty.AttackDelay);
        _reloadWait = new WaitForSeconds(_weaponProperty.ReloadDelay);
    }

    private void Update()
    {
        transform.position = _attachedSocketTransform.position;

        if(_attachedShip != null && !_isAttack)
        {
            Transform nextTarget = _attachedShip.GetEnemyTarget(transform);

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
                                                true);
    }

    private IEnumerator _AttackTarget()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 0.5f));
        for(int i = 0; i < _weaponProperty.AttackCount; i++)
        {
            ShootTarget();
            yield return _attackWait;
        }
        yield return _reloadWait;
        _isAttack = false;
    }
}
