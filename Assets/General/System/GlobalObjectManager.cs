using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalObjectManager : MonoBehaviour
{
    private static GlobalObjectManager _instance = null;
    public static GlobalObjectManager GetInstance() { return _instance; }

    private Queue<GameObject> _shipAnchor = new Queue<GameObject>();

    public GameObject GetShipAnchor() { return _shipAnchor.Count != 0 ? _shipAnchor.Dequeue() : null; }

    private void Awake()
    {
        if (_instance != null)
        {
            GlobalLogger.CallLogError(name, GErrorType.SingletonDuplicated);
        }
        _instance = this;

        GameObject.FindGameObjectsWithTag("ShipAnchor")
            .ToList<GameObject>().ForEach((GameObject go)=>_shipAnchor.Enqueue(go));
    }
}
