using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UnityEngine;

public class PlayerKingdom : Singleton<PlayerKingdom>
{
    #region Facility Handler
    private PlayerKingdomCargo _kingdomCargo = new PlayerKingdomCargo();

    public void ShipToCargo(ProductWrapper product) => _kingdomCargo.AddShipToCargo(product);
    public void ShipToField(ProductWrapper product) => _kingdomCargo.LaunchShip(product);

    public void WeaponToCargo(ProductWrapper product) => _kingdomCargo.AddWeaponToCargo(product);
    public int WeaponCount(ProductionTask pTask) => _kingdomCargo.GetSpecificWeaponCount(pTask);
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
    private class PlayerKingdomCargo
    {
        private List<ProductWrapper> _shipCargo = new List<ProductWrapper>();

        private Dictionary<ProductionTask, Queue<ProductWrapper>> _weaponCargo
            = new Dictionary<ProductionTask, Queue<ProductWrapper>>();

        private Dictionary<ProductionTask, ProductWrapper> _spaceField = new Dictionary<ProductionTask, ProductWrapper>();

        public void AddShipToCargo(ProductWrapper product) 
        {
            if (_spaceField.ContainsKey(product.ProductData)) _spaceField.Remove(product.ProductData);
            _shipCargo.Add(product); 
        }

        public void LaunchShip(ProductWrapper product)
        {
            _shipCargo.Remove(product);
            _spaceField.Add(product.ProductData, product);

            product.Instance.GetComponent<ShipController>().WarpToPosition();
        }

        public void AddWeaponToCargo(ProductWrapper product) 
        {
            if (_spaceField.ContainsKey(product.ProductData)) _spaceField.Remove(product.ProductData);

            if (!_weaponCargo.ContainsKey(product.ProductData))
                _weaponCargo[product.ProductData] = new Queue<ProductWrapper>();

            _weaponCargo[product.ProductData].Enqueue(product);
        }

        public bool AddWeaponToShip(ProductionTask productData)
        {
            bool isSucceed = (GetSpecificWeaponCount(productData) > 0);

            if (isSucceed)
            {
                ProductWrapper cache = _weaponCargo[productData].Dequeue();
                _spaceField.Add(cache.ProductData, cache);

                // Some Code Add To Ship
                //
                //
            }

            return isSucceed;
        }

        public int GetSpecificWeaponCount(ProductionTask productData)
        {
            return _weaponCargo.ContainsKey(productData) ? _weaponCargo[productData].Count : 0;
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
                    PlayerKingdom.GetInstance().ShipToCargo(new ProductWrapper(ship, this));
                    break;

                case PawnBaseController.PawnType.Weapon:
                    GameObject weapon = ProjectionManager.GetInstance().InstantiateShip(Product).Key.gameObject;
                    if (weapon.GetComponent<WeaponController>() == null)
                        GlobalLogger.CallLogError(TaskName, GErrorType.InspectorValueException);
                    PlayerKingdom.GetInstance().WeaponToCargo(new ProductWrapper(weapon, this));
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
            return (this.Crystal >= target.Crystal) &&
                  (this.Explosive >= target.Explosive) &&
                  (this.Metal >= target.Metal) &&
                  (this.Electronic >= target.Electronic);
        }
    }

    public struct ProductWrapper
    {
        public GameObject Instance;
        public ProductionTask ProductData;

        public ProductWrapper(GameObject instance, ProductionTask productData)
        {
            Instance = instance;
            ProductData = productData;
        }
    }
    #endregion
}