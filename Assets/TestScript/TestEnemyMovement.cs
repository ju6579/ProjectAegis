using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyMovement : MonoBehaviour
{
    public static TestEnemyMovement Instance;

    [SerializeField]
    private float moveSpeed = 1f;

    private Vector3 _originalPosition = Vector3.zero;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _originalPosition = transform.position;
    }

    private void Update()
    {
        transform.position = _originalPosition + Mathf.Sin(Time.time) * Vector3.forward * moveSpeed;
    }
}
