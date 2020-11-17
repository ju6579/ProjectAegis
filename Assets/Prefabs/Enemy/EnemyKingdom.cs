using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MapTileProperties;

public class EnemyKingdom : Singleton<EnemyKingdom>
{
    public Vector3 NextWarpPoint => _enemyWarpPointManager.GetNextShipWarpPoint();

    public void RequestCreateUnit(GameObject prefab, EnemyController mother)
    {
        if(_launchedEnemyUnit.Count < 45 )
            _launchedEnemyUnit.Add(ProjectionManager.GetInstance().InstantiateEnemyUnit(prefab, mother));
    }

    public void DestroyCurrentEnemyOnEscape()
    {
        _launchedEnemyMotherShips.ForEach((GameObject mother) => {
            if (mother.activeSelf)
            {
                GlobalObjectManager.ReturnToObjectPool(mother);
            }
        });
        _launchedEnemyMotherShips.Clear();

        _launchedEnemyUnit.ForEach((GameObject unit) =>
        {
            if (unit.activeSelf)
            {
                GlobalObjectManager.ReturnToObjectPool(unit);
            }
        });
        _launchedEnemyUnit.Clear();
    }

    [SerializeField]
    private Transform _enemyGate = null;

    [SerializeField]
    private float _enemySpawnTimeOffset = 1f;

    [SerializeField]
    private int _warpPointCutCount = 12;

    [SerializeField, Range(0.1f, 1f)]
    private float _warpPointBoundary = 0.2f;

    [SerializeField]
    private List<GameObject> _enemyFactory = null;

    [SerializeField]
    private float _enemySpawnTime = 20f;

    private WarpPointManager _enemyWarpPointManager = null;

    private List<GameObject> _launchedEnemyMotherShips = new List<GameObject>();
    private List<GameObject> _launchedEnemyUnit = new List<GameObject>();

    private float _enemySpawnTimeStamp = 0f;
    
    protected override void Awake()
    {
        _enemyWarpPointManager = new WarpPointManager(_enemyGate,
                                                    _warpPointCutCount,
                                                    _warpPointBoundary);

        base.Awake();
    }

    private void Start()
    {
        _enemySpawnTimeStamp = Time.time;
    }

    private void Update()
    {
        UpdateKingomByDifficulty(MapSystem.GetInstance().MapDifficulty);

        if (Time.time - _enemySpawnTimeStamp > _enemySpawnTime && _launchedEnemyMotherShips.Count < 8)
        {
            _launchedEnemyMotherShips.Add(ProjectionManager.GetInstance().InstantiateEnemy(_enemyFactory[0]));
            _enemySpawnTimeStamp = Time.time;
        }

        for (int i = 0; i < _launchedEnemyUnit.Count; i++)
        {
            if (!_launchedEnemyUnit[i].activeSelf)
            {
                _launchedEnemyUnit.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < _launchedEnemyMotherShips.Count; i++)
        {
            if (!_launchedEnemyMotherShips[i].activeSelf)
            {
                _launchedEnemyMotherShips.RemoveAt(i);
                i--;
            }
        }
    }

    private void UpdateKingomByDifficulty(float difficulty)
    {
        
    }
}