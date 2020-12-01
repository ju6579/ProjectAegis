using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Pawn;

public class ProjectPositionTracker : MonoBehaviour
{
    public PawnType ProjectedType = PawnType.NotSet;
    public ShipController TargetShipController => _projectedShipController;

    public void SetTargetShipContoller(ShipController ship) => _projectedShipController = ship;
    public void InputShipControl(Vector3 inputVector) => _projectedShipController.MoveShipByDirection(inputVector);

    public Transform _targetRootTransform = null;
    public Transform _targetAnchor = null;

    private Dictionary<Transform, Transform> _projectionHash = new Dictionary<Transform, Transform>();
    private ShipController _projectedShipController = null;
    private List<MeshRenderer> _meshRenderers = new List<MeshRenderer>();
    private Material _originalMaterial = null;

    public void SetTargetTransform(Transform root, Transform targetAnchor, Material targetMaterial)
    {
        _projectionHash.Clear();

        _targetRootTransform = root;
        _targetAnchor = targetAnchor;

        List<Transform> followTargets = targetAnchor.GetComponentsInChildren<Transform>().ToList<Transform>();
        followTargets.RemoveAt(0);

        List<Transform> followers = transform.GetComponentsInChildren<Transform>().ToList<Transform>();
        followers.RemoveAt(0);

        for (int i = 0; i < followTargets.Count; i++)
            _projectionHash.Add(followTargets[i], followers[i]);

        _originalMaterial = targetMaterial;

        if(_meshRenderers.Count <= 0)
            _meshRenderers = GetComponentsInChildren<MeshRenderer>().ToList<MeshRenderer>();

        ReplaceMaterial(targetMaterial);
    }

    public void ReplaceMaterial(Material material)
    {
        _meshRenderers.ForEach((MeshRenderer mr) => mr.material = material);
    }

    public void RevertMaterial()
    {
        _meshRenderers.ForEach((MeshRenderer mr) => mr.material = _originalMaterial);
    }

    public Transform GetProjectedTransform(Transform worldTr) => _projectionHash[worldTr];

    public void DetachRootTransform() => transform.SetParent(ProjectionManager.GetInstance().TableTransform);

    public void ReplaceRootTransform(Transform root)
    {
        ProjectPositionTracker ppt = root.GetComponentInParent<PawnBaseController>().ProjectedTarget;
        Transform projectedRoot = ppt.GetProjectedTransform(root);
        this.transform.SetParent(projectedRoot);
    }

    private void Update()
    {
        if (_targetRootTransform.gameObject.activeSelf)
        {
            transform.localPosition = _targetRootTransform.localPosition;
            transform.rotation = _targetRootTransform.rotation;

            var enumerator = _projectionHash.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var pair = enumerator.Current;
                pair.Value.localPosition = pair.Key.localPosition;
                pair.Value.localRotation = pair.Key.localRotation;
            }
        }
        else
            gameObject.SetActive(false);
    }
}