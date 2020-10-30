using System.Collections.Generic;
using UnityEngine;

public class EnemyKingdom : Singleton<EnemyKingdom>
{
    [SerializeField]
    private List<GameObject> enemyFactory = null;

    public Transform enemyGate = null;
    private GameObject _launchedEnemy = null;

    public GameObject GetCurrentEnemy() { return _launchedEnemy; }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (enemyGate != null)
            _launchedEnemy = ProjectionManager.GetInstance().InstantiateEnemy(enemyFactory[0]);
    }
}
