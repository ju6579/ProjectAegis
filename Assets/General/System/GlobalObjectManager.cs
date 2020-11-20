using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalObjectManager : Singleton<GlobalObjectManager>
{
    [SerializeField]
    private List<GameObject> _effectPrefab = new List<GameObject>();

    [SerializeField]
    private int _effectPoolExtendSize = 5;

    #region Global Prefab Object Pool
    private static int _objectPoolStartSize = 5;

    // Prefab, instance
    private static Dictionary<GameObject, Queue<GameObject>> _globalObjectPool
        = new Dictionary<GameObject, Queue<GameObject>>();

    // instance, Prefab
    private static Dictionary<GameObject, GameObject> _instanceCachePool
        = new Dictionary<GameObject, GameObject>();

    private static Dictionary<GameObject, int> _globalObjectPoolSizeHash = new Dictionary<GameObject, int>();

    #region MonoBehaviour Callbacks
    protected override void Awake()
    {
        InitializeEffectPool();
        base.Awake();
    }
    #endregion

    public static GameObject GetObject(GameObject prefab)
    {
        if (!_globalObjectPool.ContainsKey(prefab))
        {
            _globalObjectPool[prefab] = new Queue<GameObject>();

            ExtendObjectPool(prefab, _objectPoolStartSize);
            _globalObjectPoolSizeHash[prefab] = _objectPoolStartSize;
        }

        if (_globalObjectPool[prefab].Count == 0)
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
    #endregion

    #region Effect Object Pool
    private Dictionary<CustomEffectType, Queue<GameObject>> _effectObjectPoolHash 
        = new Dictionary<CustomEffectType, Queue<GameObject>>();

    private Dictionary<CustomEffectType, GameObject> _effectPrefabHash = new Dictionary<CustomEffectType, GameObject>();

    public GameObject GetEffectByPool(CustomEffectType eType)
    {
        if (_effectObjectPoolHash[eType].Count <= 0) 
            ExtendEffectPool(eType, _effectPoolExtendSize);

        return _effectObjectPoolHash[eType].Dequeue();
    }

    public void ReturnEffectToPool(GameObject instance)
    {
        EffectController ec = instance.GetComponent<EffectController>();

        if (ec == null) GlobalLogger.CallLogError(instance.name, GErrorType.InspectorValueException);

        if (instance.activeSelf) instance.SetActive(false);

        _effectObjectPoolHash[ec.EffectType].Enqueue(instance);
    }

    private void ExtendEffectPool(CustomEffectType eType, int extendAmount)
    {
        for (int i = 0; i < extendAmount; i++)
            _effectObjectPoolHash[eType].Enqueue(Instantiate(_effectPrefabHash[eType]));
    }

    private void InitializeEffectPool()
    {
        _effectPrefab.ForEach((GameObject go) =>
        {
            EffectController ec = go.GetComponent<EffectController>();
            _effectObjectPoolHash[ec.EffectType] = new Queue<GameObject>();

            for (int i = 0; i < _effectPoolExtendSize; i++)
                _effectObjectPoolHash[ec.EffectType].Enqueue(Instantiate(go));

            _effectPrefabHash.Add(ec.EffectType, go);
        });
    }
    #endregion
}

public enum CustomEffectType
{
    NotSet,
    Explosion,
    TrailSmog,
    Burn
}