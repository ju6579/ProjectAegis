using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectPositionTracker : MonoBehaviour
{
    private Transform _targetRootTransform = null;
    private Transform _targetAnchor = null;

    private Dictionary<Transform, Transform> _projectionHash = new Dictionary<Transform, Transform>();

    public void SetTargetTransform(Transform root, Transform targetAnchor, Material projectionMaterial)
    {
        _targetRootTransform = root;
        _targetAnchor = targetAnchor;

        List<Transform> followTargets = targetAnchor.GetComponentsInChildren<Transform>().ToList<Transform>();
        followTargets.RemoveAt(0);

        List<Transform> followers = transform.GetComponentsInChildren<Transform>().ToList<Transform>();
        followers.RemoveAt(0);

        for (int i = 0; i < followTargets.Count; i++)
            _projectionHash.Add(followTargets[i], followers[i]);

        List<MeshRenderer> meshRenderers = GetComponentsInChildren<MeshRenderer>().ToList<MeshRenderer>();
        meshRenderers.ForEach((MeshRenderer mr) => mr.material = projectionMaterial);
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
        if (_targetRootTransform != null)
        {
            transform.localPosition = _targetRootTransform.localPosition;
            transform.rotation = _targetRootTransform.rotation;

            var enumerator = _projectionHash.GetEnumerator();
            while(enumerator.MoveNext())
            {
                var pair = enumerator.Current;
                pair.Value.localPosition = pair.Key.localPosition;
                pair.Value.localRotation = pair.Key.localRotation;
            }
        }
        else
            Destroy(this.gameObject);
    }
}