using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKingdom : Singleton<PlayerKingdom>
{
    #region Facility Handler
    private PlayerKingdomCargo _kingdomCargo = new PlayerKingdomCargo();

    public void ShipToCargo(ProductWrapper product) => _kingdomCargo.AddShipToCargo(product);
    public void ShipToField(ProductWrapper product) => _kingdomCargo.LaunchShip(product);

    public void WeaponToCargo(ProductWrapper product) => _kingdomCargo.AddWeaponToCargo(product);
    public ProductWrapper WeaponToSocket(ProductionTask productData, GameObject socket)
        => _kingdomCargo.AddWeaponToSocket(productData, socket);

    public int WeaponCount(ProductionTask pTask) => _kingdomCargo.GetSpecificWeaponCount(pTask);

    public List<ProductWrapper> CargoList => _kingdomCargo.CargoList;
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
    private List<ProductionTask> _productionTaskCatalog = null;

    [SerializeField]
    private List<ResearchTask> _researchTaskCatalog = null;

    [SerializeField]
    private SpendableResource _kingdomResource = new SpendableResource();

    [SerializeField]
    private int _kingdomHuman = 1;

    [SerializeField]
    private int _availableTaskCount = 3;
    #endregion

    #region Private Field
    private List<ResearchTask> _availableResearch = new List<ResearchTask>();
    private List<ProductionTask> _availableProduction = new List<ProductionTask>();
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

    #region Private Method Area
    private void AddAvailableResearch(ResearchTask rTask)
    {
        _availableResearch.Add(rTask);

        ListChangedObserveComponent<ResearchTask, PlayerKingdom>.BroadcastListChange(rTask, true);
    }
    private void AddAvailableProduction(ProductionTask pTask)
    {
        _availableProduction.Add(pTask);

        ListChangedObserveComponent<ProductionTask, PlayerKingdom>.BroadcastListChange(pTask, true);
    }
    #endregion

    #region Player Kingdom Facilities
    private class PlayerKingdomCargo
    {
        public List<ProductWrapper> CargoList => _shipCargo;

        private List<ProductWrapper> _shipCargo = new List<ProductWrapper>();

        private Dictionary<ProductionTask, Queue<ProductWrapper>> _weaponCargo
            = new Dictionary<ProductionTask, Queue<ProductWrapper>>();

        private Dictionary<GameObject, ProductWrapper> _spaceField = new Dictionary<GameObject, ProductWrapper>();

        public void RemoveProduct(ProductWrapper product)
        {
            if (product.ProductData == null)
                return;

            PawnBaseController.PawnType type = product.ProductData.Product
                .GetComponent<PawnBaseController>().PawnActionType;

            if (_spaceField.ContainsKey(product.Instance)) 
                _spaceField.Remove(product.Instance);

            if(type == PawnBaseController.PawnType.SpaceShip)
            {
                int shipIndex = _shipCargo.IndexOf(product);
                if (shipIndex >= 0)
                    _shipCargo.RemoveAt(shipIndex);
            }
        }

        public void AddShipToCargo(ProductWrapper product) 
        {
            if (_spaceField.ContainsKey(product.Instance)) 
                _spaceField.Remove(product.Instance);

            _shipCargo.Add(product);

            ListChangedObserveComponent<ProductWrapper, PlayerKingdom>
                .BroadcastListChange(product, true);
        }

        public void LaunchShip(ProductWrapper product)
        {
            _shipCargo.Remove(product);
            _spaceField.Add(product.Instance, product);

            ListChangedObserveComponent<ProductWrapper, PlayerKingdom>
                .BroadcastListChange(product, false);

            product.Instance.GetComponent<ShipController>().WarpToPosition();
        }

        public void AddWeaponToCargo(ProductWrapper product) 
        {
            if (_spaceField.ContainsKey(product.Instance)) 
                _spaceField.Remove(product.Instance);

            if (!_weaponCargo.ContainsKey(product.ProductData))
                _weaponCargo[product.ProductData] = new Queue<ProductWrapper>();

            product.Instance.transform.SetParent(ProjectionManager.GetInstance().WorldTransform);
            product.Instance.transform.localPosition = Vector3.back * 5f;
            _weaponCargo[product.ProductData].Enqueue(product);
        }

        public ProductWrapper AddWeaponToSocket(ProductionTask productData, GameObject socket)
        {
            ProductWrapper product = new ProductWrapper();
            product.Instance = null;
            if (GetSpecificWeaponCount(productData) > 0)
            {
                ProductWrapper cache = _weaponCargo[productData].Dequeue();
                _spaceField.Add(cache.Instance, cache);

                cache.Instance.transform.SetParent(socket.transform);
                cache.Instance.transform.localPosition = Vector3.zero;

                ProjectionManager.PairOtherObject(cache.Instance, socket);

                product = cache;
            }

            return product;
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