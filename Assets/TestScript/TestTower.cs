using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTower : MonoBehaviour
{
    [SerializeField]
    private GameObject Bullet = null;

    [SerializeField]
    private GameObject TowerBase = null;

    [SerializeField]
    private GameObject muzzle = null;

    private bool isAttack = false;

    private WaitForSeconds _fireRate = new WaitForSeconds(0.1f);
    private WaitForSeconds _reloadRate = new WaitForSeconds(1f);

    private void Update()
    {
        if(TestEnemyMovement.Instance != null)
        {
            TowerBase.transform.LookAt(TestEnemyMovement.Instance.transform);
            if (!isAttack)
            {
                StartCoroutine(_Fire());
                isAttack = true;
            }
        }
    }

    private IEnumerator _Fire()
    {
        while (this.enabled)
        {
            ProjectionManager.GetInstance().InstantiateToWorld(Bullet,
                ProjectionManager.GetInstance().GetWorldPlaneLocalPosition(muzzle.transform.position),
                TowerBase.transform.rotation);

            yield return _fireRate;

            ProjectionManager.GetInstance().InstantiateToWorld(Bullet,
                ProjectionManager.GetInstance().GetWorldPlaneLocalPosition(muzzle.transform.position),
                TowerBase.transform.rotation);

            yield return _fireRate;

            ProjectionManager.GetInstance().InstantiateToWorld(Bullet,
                ProjectionManager.GetInstance().GetWorldPlaneLocalPosition(muzzle.transform.position),
                TowerBase.transform.rotation);

            yield return _reloadRate;
        }
    }
}
