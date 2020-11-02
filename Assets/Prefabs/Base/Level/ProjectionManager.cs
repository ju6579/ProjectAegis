using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectionManager : Singleton<ProjectionManager>
{
    [SerializeField]
    private Material projectionMaterial = null;

    [SerializeField]
    private GameObject worldSpace = null;

    [SerializeField]
    private GameObject tableSpace = null;

    private float _worldScaleRatio = 1f;

    #region Public Method
    // KeyValuePair<World Transform, Table Transform>
    private KeyValuePair<Transform,Transform> InstantiateToWorld(GameObject origin, 
                                                        Vector3 targetPosition, 
                                                        Quaternion targetRotation)
    {
        Transform worldTr = Instantiate(origin, worldSpace.transform).transform;
        worldTr.localPosition = targetPosition;
        worldTr.rotation = targetRotation;
        worldTr.localScale *= _worldScaleRatio;

        PawnBaseController pbc = worldTr.GetComponent<PawnBaseController>();

        Transform tableTr = Instantiate(pbc.TargetMeshAnchor, tableSpace.transform).transform;
        tableTr.localScale = worldTr.localScale;

        ProjectPositionTracker tracker = (ProjectPositionTracker)tableTr.gameObject
            .AddComponent(typeof(ProjectPositionTracker));

        tracker.SetTargetTransform(worldTr, pbc.TargetMeshAnchor.transform, projectionMaterial);

        return new KeyValuePair<Transform, Transform>(worldTr, tableTr);
    }

    public KeyValuePair<Transform,Transform> InstantiateShip(GameObject ship)
    {
        return InstantiateToWorld(ship, Vector3.back * 5f, Quaternion.identity);
    }

    public GameObject InstantiateEnemy(GameObject enemy)
    {
        return InstantiateToWorld(enemy, 
                             Vector3.back * 5f, 
                             Quaternion.Euler(0, 180, 0)).Key.gameObject;
    }

    public void InstantiateBullet(GameObject bullet, Vector3 worldPosition, Quaternion rotation)
    {
        Vector3 localPosition = worldSpace.transform.InverseTransformPoint(worldPosition);
        InstantiateToWorld(bullet, localPosition, rotation);
    }

    public void InstantiateWeapon(GameObject weapon, Collider socketCollider)
    {
        Vector3 spawnPosition = socketCollider.gameObject.transform.position;
        spawnPosition = tableSpace.transform.InverseTransformPoint(spawnPosition);
        spawnPosition.y += weapon.transform.localScale.y * _worldScaleRatio;

        KeyValuePair<Transform, Transform> trs = InstantiateToWorld(weapon, 
                                                            spawnPosition, 
                                                            Quaternion.identity);

        Transform worldObject = socketCollider.gameObject.GetComponentInParent<ProjectPositionTracker>()
            .GetProjectionTarget(socketCollider.gameObject.transform);

        trs.Key.SetParent(worldObject);
        trs.Value.SetParent(socketCollider.gameObject.transform);
    }

    #endregion

    #region MonoBehaviour Callbacks
    protected override void Awake()
    {
        base.Awake();
        _worldScaleRatio = 1f / worldSpace.transform.localScale.x;
    }
    #endregion
}
