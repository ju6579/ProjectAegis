using System.Collections.Generic;
using UnityEngine;

using Pawn;

public class ProjectionManager : Singleton<ProjectionManager>
{
    #region Projection Manager Data
    public Vector3 TableWorldPosition => _tableSpace.transform.position;
    public Transform WorldTransform => _worldSpace.transform;
    public Transform TableTransform => _tableSpace.transform;
    public Material ProjectionMaterial => _playerProjectionMaterial;
    #endregion

    [SerializeField]
    private Material _playerProjectionMaterial = null;

    [SerializeField]
    private Material _enemyProjectionMaterial = null;

    [SerializeField]
    private GameObject _worldSpace = null;

    [SerializeField]
    private GameObject _tableSpace = null;

    [SerializeField]
    private Material _enemyBulletMaterial = null;

    [SerializeField]
    private Material _playerBulletMaterial = null;

    private float _worldScaleRatio = 1f;

    #region Public Method
    public KeyValuePair<Transform,Transform> InstantiateProduct(GameObject ship)
    {
        return InstantiateToWorld(ship, Vector3.back * 10f, Quaternion.identity, false);
    }

    public GameObject InstantiateEnemy(GameObject enemy)
    {
        return InstantiateToWorld(enemy, 
                             Vector3.forward * 10f, 
                             Quaternion.Euler(0, 180, 0),
                             true).Key.gameObject;
    }

    public GameObject InstantiateEnemyUnit(GameObject unit, EnemyController ec)
    {
        GameObject instance = InstantiateToWorld(unit, 
                                           ec.transform.localPosition, 
                                           ec.transform.localRotation,
                                           true).Key.gameObject;

        instance.GetComponent<EnemyUnitController>().InitiallizeUnit(ec);
        
        return instance;
    }

    public void InstantiateBullet(GameObject bullet, Vector3 worldPosition, Quaternion rotation, bool isShootByPlayer, int bulletDamage, Transform target)
    {
        Vector3 localPosition = _worldSpace.transform.InverseTransformPoint(worldPosition);
        KeyValuePair<Transform, Transform> instance = InstantiateToWorld(bullet, localPosition, rotation, !isShootByPlayer);

        BulletMovement bulletComponent = instance.Key.GetComponent<BulletMovement>();
        
        if (isShootByPlayer)
        {
            bulletComponent.SetBulletProperty(GlobalGameManager.GetInstance().PlayerBulletTargetLayer,
                                        _playerBulletMaterial,
                                        isShootByPlayer,
                                        bulletDamage,
                                        target);
        }
        else
        {
            bulletComponent.SetBulletProperty(GlobalGameManager.GetInstance().EnemyBulletTargetLayer,
                                        _enemyBulletMaterial,
                                        isShootByPlayer,
                                        bulletDamage,
                                        target);
        }
    }

    public KeyValuePair<Transform, Transform> InstantiateWeapon(GameObject weapon)
    {
        return InstantiateToWorld(weapon, Vector3.up * 10f, Quaternion.identity, true);
    }

    ////////////////////////////////// Not Use
    public KeyValuePair<Transform, Transform> InstantiatePlayerBaseShip(GameObject playerBaseShip)
    {
        return InstantiateToWorld(playerBaseShip, _tableSpace.transform.position, _tableSpace.transform.rotation, false);
    }
    ////////////////////////////////// Not Use
    #endregion

    #region MonoBehaviour Callbacks
    protected override void Awake()
    {
        _worldScaleRatio = 1f / _worldSpace.transform.localScale.x;
        base.Awake();
    }
    #endregion

    #region Private Method Area
    private GameObject InstantiateByObjectPool(GameObject original, Transform parent)
    {
        GameObject result = GlobalObjectManager.GetObject(original);
        result.transform.SetParent(parent);

        return result;
    }

    /// <summary>
    /// Instantiate Prefab to World and Table, and Link Each Other for Project Mesh.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="targetPosition"></param>
    /// <param name="targetRotation"></param>
    /// <param name="isEnemy"></param>
    /// <returns> Return Key Value Pair of World Transform and Table Projected Transform </returns>
    private KeyValuePair<Transform, Transform> InstantiateToWorld(GameObject origin,
                                                        Vector3 targetPosition,
                                                        Quaternion targetRotation,
                                                        bool isEnemy)
    {
        Transform worldTr = InstantiateByObjectPool(origin, _worldSpace.transform).transform;
        worldTr.localPosition = targetPosition;
        worldTr.rotation = targetRotation;
        //worldTr.localScale *= _worldScaleRatio;


        PawnBaseController worldPawn = worldTr.GetComponent<PawnBaseController>();

        Transform tableTr = null;

        // If Instantiate First, Add Component Position Tracker, or not Reuse Tracker
        if (worldPawn.ProjectedTarget == null)
        {
            PawnBaseController prefabPawn = origin.GetComponent<PawnBaseController>();

            tableTr = InstantiateByObjectPool(prefabPawn.TargetMeshAnchor, _tableSpace.transform).transform;
            tableTr.localPosition = worldTr.localPosition;
            tableTr.rotation = worldTr.rotation;
            tableTr.localScale = worldTr.localScale;

            ProjectPositionTracker tracker = tableTr.GetComponent<ProjectPositionTracker>();
            if (tracker == null)
                tracker = (ProjectPositionTracker)tableTr.gameObject.AddComponent(typeof(ProjectPositionTracker));

            tracker.SetTargetTransform(worldTr,
                                  worldPawn.TargetMeshAnchor.transform,
                                  isEnemy ? _enemyProjectionMaterial : _playerProjectionMaterial);

            tracker.ProjectedType = worldPawn.PawnActionType;
            if (tracker.ProjectedType == PawnType.SpaceShip)
                tracker.SetTargetShipContoller(worldTr.gameObject.GetComponent<ShipController>());

            worldPawn.ProjectedTarget = tracker;
        }
        else
        {
            tableTr = worldPawn.ProjectedTarget.transform;

            tableTr.GetComponent<ProjectPositionTracker>()
                .ReplaceMaterial(isEnemy ? _enemyProjectionMaterial : _playerProjectionMaterial);
        }

        return new KeyValuePair<Transform, Transform>(worldTr, tableTr);
    }
    #endregion
}
