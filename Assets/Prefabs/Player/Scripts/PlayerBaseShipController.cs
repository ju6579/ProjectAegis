﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseShipController : Singleton<PlayerBaseShipController>
{
    public List<GameObject> Sockets = new List<GameObject>();

    [SerializeField]
    private GameObject _playerShipPrefab = null;

    private GameObject _playerShipWorldObject;

#pragma warning disable IDE0052 // 읽지 않은 private 멤버 제거
    private GameObject _playerShipProjectedObject;
#pragma warning restore IDE0052 // 읽지 않은 private 멤버 제거

    protected override void Awake()
    {
        Singleton<ProjectionManager>.ListenSingletonLoaded(() =>
        {
            KeyValuePair<Transform, Transform> instances
                = ProjectionManager.GetInstance().InstantiatePlayerBaseShip(_playerShipPrefab);

            _playerShipWorldObject = instances.Key.gameObject;
            _playerShipProjectedObject = instances.Value.gameObject;

            GameObject anchor = _playerShipWorldObject.GetComponent<PawnBaseController>().TargetMeshAnchor;
            for (int i = 0; i < anchor.transform.childCount; i++)
            {
                Transform tr = anchor.transform.GetChild(i);
                Debug.Log(tr.name);
                if (tr.CompareTag("Socket"))
                    Sockets.Add(tr.gameObject);
            }

            _playerShipWorldObject.transform.position = ProjectionManager.GetInstance().TableWorldPosition;
        });
        base.Awake();
    }
}