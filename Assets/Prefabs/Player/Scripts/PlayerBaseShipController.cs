using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseShipController : Singleton<PlayerBaseShipController>
{
    public List<GameObject> Sockets = new List<GameObject>();

    [SerializeField]
    private GameObject _playerShipPrefab = null;

    [SerializeField]
    private Transform _socketAnchor = null;

    private GameObject _playerShipWorldObject;

    private GameObject _playerShipProjectedObject;

    protected override void Awake()
    {
        Singleton<ProjectionManager>.ListenSingletonLoaded(() =>
        {
            KeyValuePair<Transform, Transform> instances
                = ProjectionManager.GetInstance().InstantiatePlayerBaseShip(_playerShipPrefab);

            _playerShipWorldObject = instances.Key.gameObject;
            _playerShipProjectedObject = instances.Value.gameObject;

            GameObject anchor = _playerShipWorldObject.GetComponent<PawnBaseController>().SocketAnchor;
            
            for (int i = 0; i < anchor.transform.childCount; i++)
            {
                Transform tr = anchor.transform.GetChild(i);
                if (tr.CompareTag("Socket"))
                    Sockets.Add(tr.gameObject);
            }

            _playerShipWorldObject.transform.position = ProjectionManager.GetInstance().TableWorldPosition;
        });
        base.Awake();
    }
}
