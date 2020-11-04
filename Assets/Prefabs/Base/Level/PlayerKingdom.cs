using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;

public class PlayerKingdom : Singleton<PlayerKingdom>
{
    #region Facility Handler
    private Facilities _facilityManager = new Facilities();

    public void ShipToCargo(GameObject instance) => _facilityManager.AddShipToCargo(instance);
    public void ShipToField(GameObject instance) => _facilityManager.LaunchShip(instance);

    public void WeaponToCargo(GameObject prefab, GameObject instance) => _facilityManager.AddWeaponToCargo(prefab, instance);
    public int WeaponCount(GameObject prefab) => _facilityManager.GetSpecificWeaponCount(prefab);
    #endregion

    #region Kingdom Handler
    public delegate void OnListChange<T>(T changedValue);

    public int CurrentAvailableTaskCount => _availableTaskCount;
    public void EndTask() => _availableTaskCount++;

    public static void ListenAvailableProduction(OnListChange<ProductionTask> listener) => _availableProductionListener += listener;
    public static void ListenAvailableResearch(OnListChange<ResearchTask> listener) => _availableResearchListener += listener;
    #endregion

    #region Inspector Field
    [SerializeField]
    private List<ProductionTask> _productionTaskCatalog;

    [SerializeField]
    private List<ResearchTask> _researchTaskCatalog;

    [SerializeField]
    private SpendableResource _kingdomResource;

    [SerializeField]
    private int _kingdomHuman = 1;
    #endregion

    #region Private Field
    private int _availableTaskCount = 2;

    private List<ResearchTask> _availableResearch = new List<ResearchTask>();
    private static OnListChange<ResearchTask> _availableResearchListener;
    private void AddAvailableResearch(ResearchTask rt) => _availableResearchListener(rt);

    private List<ProductionTask> _availableProduction = new List<ProductionTask>();
    private static OnListChange<ProductionTask> _availableProductionListener;
    private void AddAvailableProduct(ProductionTask pt) => _availableProductionListener(pt);
    #endregion

    #region Kingdom Command
    public bool RequestTaskToKingdom(PlayerTask pTask)
    {
        bool isAvailable = _availableTaskCount > 0 && _kingdomResource.IsSpendable(pTask.TaskCost);
        if (isAvailable)
        {
            _availableTaskCount--;
            StartCoroutine(pTask._Run());
        }
        return isAvailable;
    }

    public void RequestWarpToField(GameObject go)
    {

    }
    #endregion

    #region MonoBehaviour Callbacks
    protected override void Awake()
    {
        base.Awake();
        _availableProductionListener += (ProductionTask pt) => _availableProduction.Add(pt);
        _availableResearchListener += (ResearchTask rt) => _availableResearch.Add(rt);
    }

    private void Start()
    {
        _productionTaskCatalog.ForEach((ProductionTask pt) => AddAvailableProduct(pt));
        _researchTaskCatalog.ForEach((ResearchTask rt) => AddAvailableResearch(rt));
    }
    #endregion

    #region Player Kingdom Facilities
    private class Facilities
    {
        private ObservableCollection<GameObject> _shipCargo = new ObservableCollection<GameObject>();

        private Dictionary<GameObject, Queue<GameObject>> _weaponCargo
            = new Dictionary<GameObject, Queue<GameObject>>();

        public void AddShipToCargo(GameObject instance) { _shipCargo.Add(instance); }

        public void LaunchShip(GameObject instance)
        {
            _shipCargo.Remove(instance);
            instance.GetComponent<ShipController>().WarpToPosition();
        }

        public void AddWeaponToCargo(GameObject prefab, GameObject instance) 
        {
            if (!_weaponCargo.ContainsKey(prefab))
                _weaponCargo[prefab] = new Queue<GameObject>();

            _weaponCargo[prefab].Enqueue(instance);
        }

        public int GetSpecificWeaponCount(GameObject prefab)
        {
            return _weaponCargo.ContainsKey(prefab) ? _weaponCargo[prefab].Count : 0;
        }

        public GameObject MaintenanceTarget = null;
    }
    #endregion

    #region Player Task Type
    [Serializable]
    public class PlayerTask
    {
        public string TaskName = "";
        public string TaskInformation = "";
        public float TaskExecuteTime = 0;
        public SpendableResource TaskCost;

        public IEnumerator _Run()
        {
            yield return new WaitForSeconds(TaskExecuteTime);
            TaskAction();

            PlayerKingdom.GetInstance().EndTask();
        }

        protected virtual void TaskAction() { }
    }

    [Serializable]
    public class ProductionTask : PlayerTask
    {
        public GameObject Product = null;

        public Sprite TaskIcon = null;

        protected override void TaskAction()
        {
            base.TaskAction();

            PawnBaseController pawn = Product.GetComponent<PawnBaseController>();
            if (pawn == null) GlobalLogger.CallLogError(TaskName, GErrorType.InspectorValueException);

            switch (pawn.PawnActionType)
            {
                case PawnBaseController.PawnType.SpaceShip:
                    GameObject ship = ProjectionManager.GetInstance().InstantiateShip(Product).Key.gameObject;
                    if (ship.GetComponent<ShipController>() == null)
                        GlobalLogger.CallLogError(TaskName, GErrorType.InspectorValueException);
                    PlayerKingdom.GetInstance().ShipToCargo(ship);
                    break;

                case PawnBaseController.PawnType.Weapon:
                    GameObject weapon = ProjectionManager.GetInstance().InstantiateShip(Product).Key.gameObject;
                    if (weapon.GetComponent<WeaponController>() == null)
                        GlobalLogger.CallLogError(TaskName, GErrorType.InspectorValueException);
                    PlayerKingdom.GetInstance().WeaponToCargo(Product, weapon);
                    break;

                default:
                    GlobalLogger.CallLogError(TaskName, GErrorType.InspectorValueException);
                    break;
            }
        }
    }

    [Serializable]
    public class ResearchTask : PlayerTask
    {
        protected override void TaskAction()
        {
            base.TaskAction();
        }
    }
    #endregion

    #region CustomType
    [Serializable]
    public struct SpendableResource
    {
        public int Crystal;
        public int Explosive;
        public int Metal;
        public int Electronic;

        public SpendableResource(int crystal, int explosive, int metal, int electornic)
        {
            this.Crystal = crystal;
            this.Explosive = explosive;
            this.Metal = metal;
            this.Electronic = electornic;
        }

        public bool IsSpendable(SpendableResource target)
        {
            return (this.Crystal > target.Crystal) &&
                  (this.Explosive > target.Explosive) &&
                  (this.Metal > target.Metal) &&
                  (this.Electronic > target.Electronic);
        }
    }
    #endregion
}