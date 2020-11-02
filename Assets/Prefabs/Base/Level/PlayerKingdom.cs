using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerKingdom : Singleton<PlayerKingdom>
{
    #region Public Field
    public void RequestTaskToKingdom(PlayerTask playerTask)
    {
        TaskWrapper taskWrapper = new TaskWrapper(playerTask);
        StartCoroutine(taskWrapper._ExecuteTask(_taskHash));
    }

    public void RequestShipWarpToKingdom(GameObject go)
    {
        if(_shipCargo.Count != 0)
        {
            foreach(KeyValuePair<GameObject, List<GameObject>> kvp in _shipCargo)
            {
                kvp.Value.ForEach((GameObject cache) => cache.GetComponent<ShipController>().WarpToPosition());
            }
        }
    }
    #endregion

    #region Inspector Field
    public List<MaintenanceTask> MaintenanceTasks;

    public List<ResearchTask> ResearchTasks;

    public List<ProductionTask> ProductionTasks;

    public List<BuildTask> BuildTasks;
    #endregion

    #region MonoBehaviour Field

    #endregion

    #region Private Field
    // Except of Maintenance Task
    private HashSet<TaskWrapper> _taskHash = new HashSet<TaskWrapper>();

    private Dictionary<GameObject, List<GameObject>> _shipCargo 
        = new Dictionary<GameObject, List<GameObject>>();
    public void SetShipToCargo(GameObject prefab, GameObject shiptInstance) 
    {
        if (!_shipCargo.ContainsKey(prefab)) _shipCargo[prefab] = new List<GameObject>();
        _shipCargo[prefab].Add(shiptInstance);
    }

    private Dictionary<GameObject, List<GameObject>> _weaponCargo 
        = new Dictionary<GameObject, List<GameObject>>();
    public void SetWeaponToCargo(GameObject prefab, GameObject weaponInstance)
    {
        if (_weaponCargo.ContainsKey(prefab)) _weaponCargo[prefab] = new List<GameObject>();
        _weaponCargo[prefab].Add(weaponInstance);
    }
    #endregion

    #region Custom Type Class Area
    private struct TaskWrapper
    {
        public string TaskName;
        public float CurrentProgressRatio => (Time.time - _taskStartTime) / _taskData.TaskExecuteTime;

        private float _taskStartTime;
        private PlayerTask _taskData;

        public TaskWrapper(PlayerTask taskType)
        {
            TaskName = taskType.TaskName;
            _taskData = taskType;
            _taskStartTime = Time.time;
        }

        public IEnumerator _ExecuteTask(HashSet<TaskWrapper> taskMonitor)
        {
            taskMonitor.Add(this);

            yield return new WaitForSeconds(_taskData.TaskExecuteTime);
            _taskData.TaskAction();

            taskMonitor.Remove(this);
        }
    }

    public class PlayerTask
    {
        public string TaskName = "";
        public virtual void TaskAction() { }
        public float TaskExecuteTime = 0f;
        public Texture2D TaskIcon = null;
    }

    #region Task Types
    [Serializable]
    public class MaintenanceTask : PlayerTask
    {

    }

    [Serializable]
    public class ResearchTask : PlayerTask
    {
        public override void TaskAction()
        {
            base.TaskAction();
        }
    }

    [Serializable]
    public class ProductionTask : PlayerTask
    {
        [SerializeField]
        private GameObject TaskTarget;

        public override void TaskAction()
        {
            base.TaskAction();
            PawnBaseController pawnProperties = TaskTarget.GetComponent<PawnBaseController>();
            switch (pawnProperties.PawnActionType)
            {
                case PawnBaseController.PawnType.SpaceShip:
                    GameObject ship = ProjectionManager.GetInstance().InstantiateShip(TaskTarget).Key.gameObject;
                    PlayerKingdom.GetInstance().SetShipToCargo(TaskTarget, ship);
                    break;

                case PawnBaseController.PawnType.Weapon:
                    GameObject weapon = ProjectionManager.GetInstance().InstantiateShip(TaskTarget).Key.gameObject;
                    PlayerKingdom.GetInstance().SetWeaponToCargo(TaskTarget, weapon);
                    break;

                default:
                    GlobalLogger.CallLogError(TaskTarget.name, GErrorType.InspectorValueException);
                    break;
            }
        }
    }

    [Serializable]
    public class BuildTask : PlayerTask
    {
        public override void TaskAction()
        {
            base.TaskAction();
        }
    }
    #endregion

    #endregion
}