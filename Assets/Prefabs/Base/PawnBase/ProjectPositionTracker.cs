using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectPositionTracker : MonoBehaviour
{
    public Transform TargetTransform = null;

    private void Update()
    {
        transform.localPosition = TargetTransform.localPosition;
        transform.rotation = TargetTransform.rotation;
    }
}
