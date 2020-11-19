using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayerKindom.PlayerKindomTypes;
using Pawn;

namespace PlayerKindom
{
    public class PlayerKingdom : Singleton<PlayerKingdom>
    {
        #region Facility Handler
        public void ShipToCargo(ProductWrapper product) => _kingdomCargo.AddShipToCargo(product);
        public void ShipToField(ProductWrapper product) => _kingdomCargo.LaunchShip(product);

        public void WeaponToCargo(ProductWrapper product) => _kingdomCargo.AddWeaponToCargo(product);
        public ProductWrapper WeaponToField(ProductionTask productData, ShipController ship, GameObject socket)
            => _kingdomCargo.GetWeaponProduct(productData, ship, socket);

        public int WeaponCount(ProductionTask pTask) => _kingdomCargo.GetSpecificWeaponCount(pTask);

        public List<ProductWrapper> ShipCargoList => _kingdomCargo.ShipCargoList;

        public Vector3 NextWarpPoint => _warpPointManager.GetNextShipWarpPoint();
        #endregion

        #region Kingdom Handler
        public int CurrentAvailableTaskCount => _availableTaskCount;
        public void EndTask() => _availableTaskCount++;
        public void ProductDestoryed(ProductWrapper product) => _kingdomCargo.RemoveProduct(product);

        public List<ProductionTask> ProductList => _productionTaskCatalog;
        public List<ResearchTask> ResearchList => _researchTaskCatalog;

        public float EscapeTime => _escapeSpendTime;
        public void ExecuteEscape() => StartCoroutine(_ExecuteEscape());
        public SpendableResource CurrentResource => _kingdomResource;
        #endregion

        #region Inspector Field
        [SerializeField]
        private List<ProductionTask> _productionTaskCatalog = null;

        [SerializeField]
        private List<ResearchTask> _researchTaskCatalog = null;

        [SerializeField]
        private SpendableResource _kingdomResource = null;

        [SerializeField]
        private int _kingdomHuman = 1;

        [SerializeField]
        private int _availableTaskCount = 3;

        [SerializeField]
        private Transform _warpBaseTransform = null;

        [SerializeField]
        private int _warpPointCutCount = 12;

        [SerializeField, Range(0.01f, 0.2f)]
        private float _warpPointBoundary = 0.1f;

        [SerializeField]
        private float _escapeSpendTime = 3f;
        #endregion

        #region Private Field
        private List<ResearchTask> _availableResearch = new List<ResearchTask>();
        private List<ProductionTask> _availableProduction = new List<ProductionTask>();

        private PlayerKingdomCargo _kingdomCargo = new PlayerKingdomCargo();
        private WarpPointManager _warpPointManager;

        private float _harvestTimeStamp = 0f;
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
            _warpPointManager = new WarpPointManager(_warpBaseTransform,
                                                   _warpPointCutCount, 
                                                   _warpPointBoundary);
            base.Awake();
        }

        private void Start()
        {
            _productionTaskCatalog.ForEach((ProductionTask pt) => AddAvailableProduction(pt));
            _researchTaskCatalog.ForEach((ResearchTask rt) => AddAvailableResearch(rt));

            _harvestTimeStamp = Time.time;
        }

        
        private void Update()
        {
            if(Time.time - _harvestTimeStamp > 1f)
            {
                MapSystem.GetInstance().HarvestResourceAtCurrentTile(_kingdomResource);
                _harvestTimeStamp = Time.time;
            }
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

        private IEnumerator _ExecuteEscape()
        {
            WaitForSeconds escapeWait = new WaitForSeconds(_escapeSpendTime / 2f);
            yield return escapeWait;

            EnemyKingdom.GetInstance().DestroyCurrentEnemyOnEscape();
            _kingdomCargo.HandleFieldShipOnEscape();

            yield return escapeWait;
            
            yield return null;
        }
        #endregion

        #region Player Kingdom Facilities
        private class PlayerKingdomCargo
        {
            public List<ProductWrapper> ShipCargoList => _shipCargo;

            private List<ProductWrapper> _shipCargo = new List<ProductWrapper>();

            private Dictionary<ProductionTask, Queue<ProductWrapper>> _weaponCargo
                = new Dictionary<ProductionTask, Queue<ProductWrapper>>();

            private Dictionary<GameObject, ProductWrapper> _fieldShipHash = new Dictionary<GameObject, ProductWrapper>();

            public void RemoveProduct(ProductWrapper product)
            {
                if (product.Instance == null)
                {
                    Debug.Log("Product is Null");
                    return;
                }

                PawnType type = product.Instance.GetComponent<PawnBaseController>().PawnActionType;
                if(type == PawnType.SpaceShip)
                {
                    if (_shipCargo.Contains(product))
                        _shipCargo.Remove(product);

                    if (_fieldShipHash.ContainsKey(product.Instance))
                        _fieldShipHash.Remove(product.Instance);
                }
                else if(type == PawnType.Weapon)
                {
                    product.DisableProductInstance();
                }
                else
                {
                    GlobalLogger.CallLogError(product.ProductData.Product.name, GErrorType.InspectorValueException);
                }

                product.ClearProductWrapper();
            }

            public void AddShipToCargo(ProductWrapper product)
            {
                if (_fieldShipHash.ContainsKey(product.Instance))
                    _fieldShipHash.Remove(product.Instance);

                if (product.Instance.activeSelf)
                {
                    product.Instance.GetComponent<ShipController>().OnShipToCargo();
                    product.Instance.SetActive(false);
                }

                _shipCargo.Add(product);

                ListChangedObserveComponent<ProductWrapper, PlayerKingdom>
                    .BroadcastListChange(product, true);
            }

            public void LaunchShip(ProductWrapper product)
            {
                _shipCargo.Remove(product);
                _fieldShipHash.Add(product.Instance, product);

                product.Instance.transform.localPosition = Vector3.back * 5f;
                product.Instance.SetActive(true);

                ListChangedObserveComponent<ProductWrapper, PlayerKingdom>
                    .BroadcastListChange(product, false);

                product.Instance.GetComponent<ShipController>().WarpToPosition();
            }

            public void AddWeaponToCargo(ProductWrapper product)
            {
                if (!_weaponCargo.ContainsKey(product.ProductData))
                    _weaponCargo[product.ProductData] = new Queue<ProductWrapper>();

                product.DisableProductInstance();

                _weaponCargo[product.ProductData].Enqueue(product);
            }

            public ProductWrapper GetWeaponProduct(ProductionTask productData, ShipController ship, GameObject socket)
            {
                ProductWrapper product = null;

                if (GetSpecificWeaponCount(productData) > 0)
                {
                    ProductWrapper cache = _weaponCargo[productData].Dequeue();

                    product = cache;
                }

                return product;
            }

            public int GetSpecificWeaponCount(ProductionTask productData)
            {
                return _weaponCargo.ContainsKey(productData) ? _weaponCargo[productData].Count : 0;
            }

            public void HandleFieldShipOnEscape()
            {
                var fieldEnum = _fieldShipHash.GetEnumerator();

                while (fieldEnum.MoveNext())
                {
                    ShipController ship = fieldEnum.Current.Key.GetComponent<ShipController>();
                    ship.transform.localPosition = Vector3.back * 5f;
                    ship.WarpToPosition();
                }
            }
        }
        #endregion
    }

    namespace PlayerKindomTypes
    {
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
            public PawnType ProductType
                => Product.GetComponent<PawnBaseController>().PawnActionType;

            public Sprite TaskIcon = null;

            protected override void TaskAction()
            {
                base.TaskAction();

                PawnBaseController productPawn = Product.GetComponent<PawnBaseController>();
                if (productPawn == null) GlobalLogger.CallLogError(TaskName, GErrorType.InspectorValueException);

                switch (productPawn.PawnActionType)
                {
                    case PawnType.SpaceShip:
                        if (Product.GetComponent<ShipController>() == null)
                            GlobalLogger.CallLogError(TaskName, GErrorType.InspectorValueException);

                        PlayerKingdom.GetInstance().ShipToCargo(new ProductWrapper(this));
                        break;

                    case PawnType.Weapon:
                        if (Product.GetComponent<WeaponController>() == null)
                            GlobalLogger.CallLogError(TaskName, GErrorType.InspectorValueException);

                        PlayerKingdom.GetInstance().WeaponToCargo(new ProductWrapper(this));
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
        public class SpendableResource
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

            public void AddResourceByData(SpendableResource data)
            {
                this.Crystal += data.Crystal;
                this.Explosive += data.Explosive;
                this.Metal += data.Metal;
                this.Electronic += data.Electronic;
            }

            public bool IsSpendable(SpendableResource target)
            {
                return (this.Crystal >= target.Crystal) &&
                      (this.Explosive >= target.Explosive) &&
                      (this.Metal >= target.Metal) &&
                      (this.Electronic >= target.Electronic);
            }
        }

        public class ProductWrapper
        {
            public GameObject Instance;
            public ProductionTask ProductData;
            public PawnType ProductType;

            public ProductWrapper(ProductionTask productData)
            {
                ProductData = productData;
                ProductType = productData.Product.GetComponent<PawnBaseController>().PawnActionType;
                Instance = null;

                if(ProductType == PawnType.SpaceShip)
                {
                    Instance = ActiveProductInstance();
                    Instance.SetActive(false);
                }                
            }

            public GameObject ActiveProductInstance()
            {
                this.Instance = ProjectionManager.GetInstance().InstantiateProduct(ProductData.Product).Key.gameObject;
                return Instance;
            }

            public void DisableProductInstance()
            {
                if(Instance != null)
                {
                    GlobalObjectManager.ReturnToObjectPool(Instance);
                }

                Instance = null;
            }

            public void ClearProductWrapper()
            {
                Instance = null;
                ProductData = null;
            }
        }
        #endregion
    }
}