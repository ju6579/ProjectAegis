using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayerKindom.PlayerKindomTypes;

public class ResearchManager : Singleton<ResearchManager>
{
    public int TrainingPoint => _currentTP;
    public void OnEnemyKilled(int point) => _currentTP += point;

    public List<ProductionTask> AvailableShipList => _availableShipList;
    public List<ProductionTask> AvailableWeaponList => _availableWeaponList;

    public List<ProductionTask> AllShipList => _shipProductList;
    public List<ProductionTask> AllWeaponList => _weaponProductList;

    public bool IsProductUnlocked(ProductionTask pTask)
    {
        return _availableShipList.Contains(pTask) || _availableWeaponList.Contains(pTask);
    }

    [SerializeField]
    private List<ProductionTask> _shipProductList = new List<ProductionTask>();

    [SerializeField]
    private List<ProductionTask> _weaponProductList = new List<ProductionTask>();

    [SerializeField]
    private List<int> _availableShipTaskID = new List<int>();

    [SerializeField]
    private List<int> _availableWeaponTaskID = new List<int>();

    private Dictionary<int, ProductionTask> _shipProductHash = new Dictionary<int, ProductionTask>();
    private Dictionary<int, ProductionTask> _weaponProductHash = new Dictionary<int, ProductionTask>();

    // Replace to Private
    public List<ProductionTask> _availableShipList = new List<ProductionTask>();
    public List<ProductionTask> _availableWeaponList = new List<ProductionTask>();
    // Replace to Private

    private int _currentTP = 0;

    protected override void OnDestroy()
    {
        
    }

    protected override void Awake()
    {
        _shipProductList.ForEach((ProductionTask pt) => _shipProductHash.Add(pt.ProductionID, pt));
        _weaponProductList.ForEach((ProductionTask pt) => _weaponProductHash.Add(pt.ProductionID, pt));

        _availableShipTaskID.ForEach((int id) => _availableShipList.Add(_shipProductHash[id]));
        _availableWeaponTaskID.ForEach((int id) => _availableWeaponList.Add(_weaponProductHash[id]));

        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}
