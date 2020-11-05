using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectPositionTracker : MonoBehaviour
{
    public void SetTargetTransform(Transform root, Transform targetAnchor, Material projectionMaterial)
    {
        _targetTransform = root;
        _targetAnchor = targetAnchor;

        List<Transform> followTargets = _targetAnchor.GetComponentsInChildren<Transform>().ToList<Transform>();
        followTargets.RemoveAt(0);

        List<Transform> followers = transform.GetComponentsInChildren<Transform>().ToList<Transform>();
        followers.RemoveAt(0);

        for (int i = 0; i < followTargets.Count; i++)
            _projectionHash.Add(followTargets[i], followers[i]);

        List<MeshRenderer> meshRenderers = GetComponentsInChildren<MeshRenderer>().ToList<MeshRenderer>();
        meshRenderers.ForEach((MeshRenderer mr) => mr.material = projectionMaterial);
    }

    public Transform GetProjectionTarget(Transform projected) 
    {
        return _projectionHash[projected];
    }

    private Transform _targetTransform = null;
    private Transform _targetAnchor = null;

    private Dictionary<Transform, Transform> _projectionHash = new Dictionary<Transform, Transform>();

    private void Update()
    {
        if (_targetTransform != null)
        {
            transform.localPosition = _targetTransform.localPosition;
            transform.rotation = _targetTransform.rotation;

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