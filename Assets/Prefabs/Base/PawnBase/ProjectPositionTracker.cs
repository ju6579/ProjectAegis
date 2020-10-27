using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectPositionTracker : MonoBehaviour
{
    public Transform targetTransform;

    private void Update()
    {
        transform.localPosition = targetTransform.localPosition;
        transform.rotation = targetTransform.rotation;
    }
}
