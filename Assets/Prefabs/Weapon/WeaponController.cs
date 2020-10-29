using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletObject = null;

    [SerializeField]
    private int attackCount = 3;

    [SerializeField]
    private float attackDelay = 0.25f;

    [SerializeField]
    private float reloadDelay = 1f;

    private WaitForSeconds _attackWait;
    private WaitForSeconds _reloadWait;

    private void Awake()
    {
        _attackWait = new WaitForSeconds(attackDelay);
        _reloadWait = new WaitForSeconds(reloadDelay);
    }

    private void ShootTarget()
    {
        Instantiate(bulletObject);
    }

    private IEnumerator _AttackTarget()
    {
        for(int i = 0; i < attackCount; i++)
        {
            ShootTarget();
            yield return _attackWait;
        }
        yield return _reloadWait;
    }
}
