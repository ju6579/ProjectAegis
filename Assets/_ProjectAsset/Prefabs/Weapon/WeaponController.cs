﻿using System;
using System.Collections;
using UnityEngine;

using Pawn;

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

    public WeaponProperty WeaponData => _weaponProperty;

    [SerializeField]
    private WeaponProperty _weaponProperty;

    [SerializeField]
    private GameObject _muzzle = null;

    private bool _isAttack = false;

    public ShipController _attachedShip = null;
    public Transform _attachedSocketTransform = null;

    private WaitForSeconds _attackWait;
    private WaitForSeconds _reloadWait;
    private WaitForSeconds _randomWait;

    private void Awake()
    {
        _attackWait = new WaitForSeconds(_weaponProperty.AttackDelay);
        _reloadWait = new WaitForSeconds(_weaponProperty.ReloadDelay);
        _randomWait = new WaitForSeconds(UnityEngine.Random.Range(0f, 0.05f));
    }

    private void OnEnable()
    {
        _isAttack = false;
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
                                                true,
                                                _weaponProperty.BulletDamage);
    }

    private IEnumerator _AttackTarget()
    {
        for(int i = 0; i < _weaponProperty.AttackCount; i++)
        {
            ShootTarget();
            yield return _attackWait;
            yield return _randomWait;
        }
        yield return _reloadWait;
        _isAttack = false;
    }
}
