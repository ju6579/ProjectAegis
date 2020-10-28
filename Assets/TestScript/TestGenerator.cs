using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject testPrefab = null;

    private WaitForSeconds rate = new WaitForSeconds(3f);

    private void Start()
    {
        ProjectionManager.GetInstance().InstantiateToWorld(testPrefab, transform.localPosition, transform.rotation);
    }
}
