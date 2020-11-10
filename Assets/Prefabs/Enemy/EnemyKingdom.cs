using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKingdom : Singleton<EnemyKingdom>
{
    public Vector3 NextWarpPoint => _enemyWarpPointManager.GetNextShipWarpPoint();

    [SerializeField]
    private Transform _enemyGate = null;

    [SerializeField]
    private int _warpPointCutCount = 12;

    [SerializeField, Range(0.1f, 0.5f)]
    private float _warpPointBoundary = 0.2f;

    [SerializeField]
    private List<GameObject> _enemyFactory = null;

    private WarpPointManager _enemyWarpPointManager = null;
    private GameObject _launchedEnemyMotherShip = null;

    protected override void Awake()
    {
        _enemyWarpPointManager = new WarpPointManager(_enemyGate,
                                                    _warpPointCutCount,
                                                    _warpPointBoundary);
        base.Awake();
    }

    private void Start()
    {
        StartCoroutine(_SpawnEnemy());
    }

    private IEnumerator _SpawnEnemy()
    {
        WaitForSeconds enemySpawnRate = new WaitForSeconds(10f);

        while(this != null)
        {
            _launchedEnemyMotherShip = ProjectionManager.GetInstance().InstantiateEnemy(_enemyFactory[0]);

            yield return enemySpawnRate;
        }

        yield return null;
    }
}
