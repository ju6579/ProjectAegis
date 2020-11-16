using System.Collections.Generic;
using UnityEngine;

using Pawn;

public class ProjectionManager : Singleton<ProjectionManager>
{
    public Vector3 TableWorldPosition => _tableSpace.transform.position;
    public Transform WorldTransform => _worldSpace.transform;
    public Transform TableTransform => _tableSpace.transform;
    public Material ProjectionMaterial => _projectionMaterial;

    [SerializeField]
    private Material _projectionMaterial = null;

    [SerializeField]
    private GameObject _worldSpace = null;

    [SerializeField]
    private GameObject _tableSpace = null;

    private float _worldScaleRatio = 1f;

    #region Public Method
    private GameObject InstantiateByObjectPool(GameObject original, Transform parent)
    {
        GameObject result = GlobalObjectManager.GetObject(original);
        result.transform.SetParent(parent);

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="targetPosition"></param>
    /// <param name="targetRotation"></param>
    /// <returns> Return Key Value Pair of World Transform and Table Projected Transform </returns>
    private KeyValuePair<Transform,Transform> InstantiateToWorld(GameObject origin, 
                                                        Vector3 targetPosition, 
                                                        Quaternion targetRotation)
    {
        Transform worldTr = InstantiateByObjectPool(origin, _worldSpace.transform).transform;
        worldTr.localPosition = targetPosition;
        worldTr.rotation = targetRotation;
        //worldTr.localScale *= _worldScaleRatio;

        
        PawnBaseController worldPawn = worldTr.GetComponent<PawnBaseController>();

        Transform tableTr = null;

        if (worldPawn.ProjectedTarget == null)
        {
            PawnBaseController prefabPawn = origin.GetComponent<PawnBaseController>();

            tableTr = InstantiateByObjectPool(prefabPawn.TargetMeshAnchor, _tableSpace.transform).transform;
            tableTr.localPosition = worldTr.localPosition;
            tableTr.rotation = worldTr.rotation;
            tableTr.localScale = worldTr.localScale;

            ProjectPositionTracker tracker = tableTr.GetComponent<ProjectPositionTracker>();
            if (tracker == null)
                tracker = (ProjectPositionTracker)tableTr.gameObject
                    .AddComponent(typeof(ProjectPositionTracker));

            tracker.SetTargetTransform(worldTr, worldPawn.TargetMeshAnchor.transform);
            tracker.ProjectedType = worldPawn.PawnActionType;
            if (tracker.ProjectedType == PawnType.SpaceShip)
                tracker.SetTargetShipContoller(worldTr.gameObject.GetComponent<ShipController>());

            worldPawn.ProjectedTarget = tracker;
        }
        else
            tableTr = worldPawn.ProjectedTarget.transform;

        return new KeyValuePair<Transform, Transform>(worldTr, tableTr);
    }

    public KeyValuePair<Transform,Transform> InstantiatePlayerBaseShip(GameObject playerBaseShip)
    {
        return InstantiateToWorld(playerBaseShip, _tableSpace.transform.position, _tableSpace.transform.rotation);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ship"></param>
    /// <returns> Return Key Value Pair of World Transform and Table Projected Transform </returns>
    public KeyValuePair<Transform,Transform> InstantiateProduct(GameObject ship)
    {
        return InstantiateToWorld(ship, Vector3.back * 5f, Quaternion.identity);
    }

    public GameObject InstantiateEnemy(GameObject enemy)
    {
        return InstantiateToWorld(enemy, 
                             Vector3.back * 5f, 
                             Quaternion.Euler(0, 180, 0)).Key.gameObject;
    }

    public GameObject InstantiateEnemyUnit(GameObject unit, EnemyController ec)
    {
        GameObject instance = InstantiateToWorld(unit, 
                                           ec.transform.localPosition, 
                                           ec.transform.localRotation).Key.gameObject;

        instance.GetComponent<EnemyUnitController>().SetMotherShip(ec);
        
        return instance;
    }

    public void InstantiateBullet(GameObject bullet, Vector3 worldPosition, Quaternion rotation, bool isShootByPlayer)
    {
        Vector3 localPosition = _worldSpace.transform.InverseTransformPoint(worldPosition);
        KeyValuePair<Transform, Transform> instance = InstantiateToWorld(bullet, localPosition, rotation);

        BulletMovement bulletComponent = instance.Key.GetComponent<BulletMovement>();
        bulletComponent.IsShootByPlayer = isShootByPlayer;
        bulletComponent.SetTargetLayer(isShootByPlayer ? CustomLibrary.GetInstance().PlayerBulletTargetLayer
                                                 : CustomLibrary.GetInstance().EnemyBulletTargetLayer);
    }

    public KeyValuePair<Transform, Transform> InstantiateWeapon(GameObject weapon)
    {
        return InstantiateToWorld(weapon, Vector3.back * 5f, Quaternion.identity);
    }
    #endregion

    #region MonoBehaviour Callbacks
    protected override void Awake()
    {
        _worldScaleRatio = 1f / _worldSpace.transform.localScale.x;
        base.Awake();
    }
    #endregion
}
