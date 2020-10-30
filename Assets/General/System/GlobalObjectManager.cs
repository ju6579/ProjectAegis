using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalObjectManager : Singleton<GlobalObjectManager>
{
    private Queue<GameObject> _shipAnchor = new Queue<GameObject>();

    public GameObject GetShipAnchor() { return _shipAnchor.Count != 0 ? _shipAnchor.Dequeue() : null; }

    protected override void Awake()
    {
        base.Awake();

        GameObject.FindGameObjectsWithTag("ShipAnchor")
            .ToList<GameObject>().ForEach((GameObject go)=>_shipAnchor.Enqueue(go));
    }
}
