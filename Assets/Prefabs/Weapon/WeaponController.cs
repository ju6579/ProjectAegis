﻿using System;
using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public void SetAttachedShip(ShipController ship) => _attachedShip = ship;
    public void DetachWeaponToShip() => _attachedShip = null;

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

    private WaitForSeconds _attackWait;
    private WaitForSeconds _reloadWait;

    private void Awake()
    {
        _attackWait = new WaitForSeconds(_weaponProperty.AttackDelay);
        _reloadWait = new WaitForSeconds(_weaponProperty.ReloadDelay);
    }

    private void Update()
    {
        if(_attachedShip != null)
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
