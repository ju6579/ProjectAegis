using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionManager : MonoBehaviour
{
    private static ProjectionManager _instance = null;
    public static ProjectionManager GetInstance() { return _instance; }

    [SerializeField]
    private GameObject worldPlane = null;

    [SerializeField]
    private GameObject tablePlane = null;

    private float _worldScaleRatio = 1f;

    #region Public Method
    public void InstantiateToWorld(GameObject origin, Vector3 targetPosition, Quaternion targetRotation)
    {
        PawnBaseController pbc = origin.GetComponent<PawnBaseController>();
        
        if(pbc != null)
        {
            Transform worldTr;

            worldTr = Instantiate(origin, worldPlane.transform).transform;
            worldTr.localPosition = targetPosition;
            worldTr.rotation = targetRotation;
            worldTr.localScale *= _worldScaleRatio;

            Transform tableTr;

            tableTr = Instantiate(pbc.TargetMesh, tablePlane.transform).transform;
            tableTr.localScale = worldTr.localScale;

            tableTr.gameObject.AddComponent(typeof(ProjectPositionTracker));
            tableTr.gameObject.GetComponent<ProjectPositionTracker>().TargetTransform = worldTr;
        }
    }

    public void InstantiateToTable(GameObject origin, Vector3 targetWorldPosition, Quaternion targetRotation)
    {
        Vector3 targetLocalPosition = tablePlane.transform.InverseTransformPoint(targetWorldPosition);
        Debug.Log(targetLocalPosition);
        InstantiateToWorld(origin, targetLocalPosition, targetRotation);
    }
    #endregion

    #region MonoBehaviour Callbacks
    private void Awake()
    {
        if (_instance != null) GlobalLogger.CallLogError(name, GErrorType.SingletonDuplicated);
        _instance = this;

        _worldScaleRatio = 1f / worldPlane.transform.localScale.x;
    }
    #endregion
}
