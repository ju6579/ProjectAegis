using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletObject = null;

    [SerializeField]
    private GameObject muzzle = null;

    [SerializeField]
    private int attackCount = 3;

    [SerializeField]
    private float attackDelay = 0.25f;

    [SerializeField]
    private float reloadDelay = 1f;

    private WaitForSeconds _attackWait;
    private WaitForSeconds _reloadWait;

    private bool _isAttack = false;

    private GameObject _attackTarget = null;

    private void Awake()
    {
        _attackWait = new WaitForSeconds(attackDelay);
        _reloadWait = new WaitForSeconds(reloadDelay);
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
        ProjectionManager.GetInstance().InstantiateBullet(bulletObject, muzzle.transform.position, transform.rotation);
    }

    private IEnumerator _AttackTarget()
    {
        for(int i = 0; i < attackCount; i++)
        {
            ShootTarget();
            yield return _attackWait;
        }
        yield return _reloadWait;
        _isAttack = false;
    }
}
