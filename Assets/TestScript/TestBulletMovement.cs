using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBulletMovement : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.localPosition += transform.forward * 5f * Time.deltaTime;
    }
}
