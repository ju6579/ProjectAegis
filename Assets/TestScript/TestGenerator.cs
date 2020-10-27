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
        StartCoroutine(_test());
    }

    private IEnumerator _test()
    {
        while (this.enabled)
        {
            ProjectionManager.GetInstance().InstantiateToWorld(testPrefab, transform.localPosition, transform.rotation);
            yield return rate;
        }
        yield return null;
    }
}
