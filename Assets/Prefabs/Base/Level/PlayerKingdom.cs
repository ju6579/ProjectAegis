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
    public int CurrentAvailableTaskCount => _availableTaskCount;
    public void EndTask() => _availableTaskCount++;
    public void ProductDestoryed(ProductWrapper product) => _kingdomCargo.RemoveProduct(product);

    public List<ProductionTask> ProductList => _productionTaskCatalog;
    public List<ResearchTask> ResearchList => _researchTaskCatalog;
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

    [SerializeField]
    private int _availableTaskCount = 3;
    #endregion

    #region Private Field
    private List<ResearchTask> _availableResearch = new List<ResearchTask>();
    private void AddAvailableResearch(ResearchTask rTask) 
    {
        _availableResearch.Add(rTask);
        // Broadcast To UI
        TaskListCallbacks<ResearchTask>.BroadcastAvailableTaskChanged?.Invoke(rTask, true);
    }

    private List<ProductionTask> _availableProduction = new List<ProductionTask>();
    private void AddAvailableProduction(ProductionTask pTask)
    {
        _availableProduction.Add(pTask);
        // Broadcast To UI
        TaskListCallbacks<ProductionTask>.BroadcastAvailableTaskChanged?.Invoke(pTask, true);
    }
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
    }

    private void Start()
    {
        _productionTaskCatalog.ForEach((ProductionTask pt) => AddAvailableProduction(pt));
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

        public void RemoveProduct(ProductWrapper product)
        {
            if (product.ProductData == null)
                return;

            PawnBaseController.PawnType type = product.ProductData.Product
                .GetComponent<PawnBaseController>().PawnActionType;

            if (_spaceField.ContainsKey(product.ProductData)) _spaceField.Remove(product.ProductData);

            if(type == PawnBaseController.PawnType.SpaceShip)
            {
                int shipIndex = _shipCargo.IndexOf(product);
                if (shipIndex >= 0)
                    _shipCargo.RemoveAt(shipIndex);
            }
        }

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
            ProductWrapper product;

            switch (pawn.PawnActionType)
            {
                case PawnBaseController.PawnType.SpaceShip:
                    GameObject ship = ProjectionManager.GetInstance().InstantiateShip(Product).Key.gameObject;
                    if (ship.GetComponent<ShipController>() == null)
                        GlobalLogger.CallLogError(TaskName, GErrorType.InspectorValueException);

                    product = new ProductWrapper(ship, this);
                    pawn.PawnData = product;
                    PlayerKingdom.GetInstance().ShipToCargo(product);
                    break;

                case PawnBaseController.PawnType.Weapon:
                    GameObject weapon = ProjectionManager.GetInstance().InstantiateShip(Product).Key.gameObject;
                    if (weapon.GetComponent<WeaponController>() == null)
                        GlobalLogger.CallLogError(TaskName, GErrorType.InspectorValueException);

                    product = new ProductWrapper(weapon, this);
                    pawn.PawnData = product;
                    PlayerKingdom.GetInstance().WeaponToCargo(product);
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