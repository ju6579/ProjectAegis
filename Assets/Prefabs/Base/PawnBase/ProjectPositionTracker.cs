using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectPositionTracker : MonoBehaviour
{
    public void SetTargetTransform(Transform root, Transform targetAnchor, Material projectionMaterial)
    {
        _targetTransform = root;
        _targetAnchor = targetAnchor;

        _followTargets = _targetAnchor.GetComponentsInChildren<Transform>().ToList<Transform>();
        _followTargets.RemoveAt(0);

        _followers = transform.GetComponentsInChildren<Transform>().ToList<Transform>();
        _followers.RemoveAt(0);

        List<MeshRenderer> meshRenderers = GetComponentsInChildren<MeshRenderer>().ToList<MeshRenderer>();
        meshRenderers.ForEach((MeshRenderer mr) => mr.material = projectionMaterial);

    }

    private Transform _targetTransform = null;
    private Transform _targetAnchor = null;

    private List<Transform> _followers = null;
    private List<Transform> _followTargets = null;

    private void Update()
    {
        transform.localPosition = _targetTransform.localPosition;
        transform.rotation = _targetTransform.rotation;

        for(int i = 0; i < _followTargets.Count; i++)
        {
            _followers[i].localPosition = _followTargets[i].localPosition;
            _followers[i].rotation = _followTargets[i].rotation;
        }
    }
}