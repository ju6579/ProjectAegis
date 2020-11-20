using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalObjectManager : Singleton<GlobalObjectManager>
{
    private static int _objectPoolStartSize = 5;

    // Prefab, instance
    private static Dictionary<GameObject, Queue<GameObject>> _globalObjectPool 
        = new Dictionary<GameObject, Queue<GameObject>>();

    // instance, Prefab
    private static Dictionary<GameObject, GameObject> _instanceCachePool 
        = new Dictionary<GameObject, GameObject>();

    private static Dictionary<GameObject, int> _globalObjectPoolSizeHash = new Dictionary<GameObject, int>();

    public static GameObject GetObject(GameObject prefab)
    {
        if (!_globalObjectPool.ContainsKey(prefab)) 
        {
            _globalObjectPool[prefab] = new Queue<GameObject>();

            ExtendObjectPool(prefab, _objectPoolStartSize);
            _globalObjectPoolSizeHash[prefab] = _objectPoolStartSize;
        }

        if(_globalObjectPool[prefab].Count == 0)
        {
            ExtendObjectPool(prefab, 5);
            _globalObjectPoolSizeHash[prefab] += 5;
        }

        GameObject instance = _globalObjectPool[prefab].Dequeue();
        _instanceCachePool.Add(instance, prefab);

        instance.SetActive(true);

        return instance;
    }

    public static void ReturnToObjectPool(GameObject instance)
    {
        instance.SetActive(false);

        _globalObjectPool[_instanceCachePool[instance]].Enqueue(instance);
        _instanceCachePool.Remove(instance);
    }

    private static void ExtendObjectPool(GameObject prefab, int targetSize)
    {
        GameObject instance = null;
        for (int i = 0; i < targetSize; i++)
        {
            instance = Instantiate(prefab);
            instance.SetActive(false);

            _globalObjectPool[prefab].Enqueue(instance);
        }
    }

 
}