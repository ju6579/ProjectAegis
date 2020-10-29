using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionManager : MonoBehaviour
{
    private static ProjectionManager _instance = null;
    public static ProjectionManager GetInstance() { return _instance; }

    [SerializeField]
    private Material projectionMaterial = null;

    [SerializeField]
    private GameObject worldSpace = null;

    [SerializeField]
    private GameObject tableSpace = null;

    private float _worldScaleRatio = 1f;

    #region Public Method
    private GameObject InstantiateToWorld(GameObject origin, Vector3 targetPosition, Quaternion targetRotation)
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

        return worldTr.gameObject;
    }

    public void InstantiateShip(GameObject ship)
    {
        InstantiateToWorld(ship, Vector3.back * 2f, Quaternion.identity);
    }
    #endregion

    #region MonoBehaviour Callbacks
    private void Awake()
    {
        if (_instance != null) GlobalLogger.CallLogError(name, GErrorType.SingletonDuplicated);
        _instance = this;

        _worldScaleRatio = 1f / worldSpace.transform.localScale.x;
    }
    #endregion
}
