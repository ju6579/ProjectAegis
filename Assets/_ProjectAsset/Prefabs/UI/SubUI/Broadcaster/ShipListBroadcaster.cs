using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PlayerKindom;
using PlayerKindom.PlayerKindomTypes;
using Pawn;

public class ShipListBroadcaster : ListChangedObserveComponent<ProductionTask, PlayerKingdom>
{
    [SerializeField]
    private GameObject _shipScrollContents = null;

    private List<KeyValuePair<ScrollRect, IUIContentsCallbacks>> _scrollContentsBroadcaster
        = new List<KeyValuePair<ScrollRect, IUIContentsCallbacks>>();

    private static Dictionary<ProductionTask, List<GameObject>> _objectUIContentsHash
        = new Dictionary<ProductionTask, List<GameObject>>();

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _scrollContentsBroadcaster.Clear();
        _objectUIContentsHash.Clear();
    }

    // Chain Delegate Function for Listen Contents Change
    public void ListenShipListChanged(ScrollRect targetScrollRect, IUIContentsCallbacks callback)
    {
        _scrollContentsBroadcaster.Add(new KeyValuePair<ScrollRect, IUIContentsCallbacks>(targetScrollRect, callback));
    }

    // Initialize and Setup List for UI
    protected override void LoadList()
    {
        if (ResearchManager.GetInstance() != null)
            LoadListByProvider(ResearchManager.GetInstance().AvailableShipList);
        else
            LoadListByProvider(PlayerKingdom.GetInstance().ProductList);
    }

    protected override void OnListChanged(ProductionTask changed, bool isAdd)
    {
        base.OnListChanged(changed, isAdd);
        if (PawnBaseController.CompareType(changed.Product, PawnType.SpaceShip))
            _objectUIContentsHash[changed].ForEach((GameObject go) => go.SetActive(true));
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        
    }

    private void LoadListByProvider(List<ProductionTask> productList)
    {
        productList.ForEach((ProductionTask pTask) =>
        {
            if (PawnBaseController.CompareType(pTask.Product, PawnType.SpaceShip))
            {
                _scrollContentsBroadcaster.ForEach((KeyValuePair<ScrollRect, IUIContentsCallbacks> keyPair) =>
                {
                    GameObject cache = Instantiate(_shipScrollContents, keyPair.Key.content);
                    cache.GetComponent<UIProductContentsProperty>().SetUIContentsData(pTask);

                    Button buttonCache = cache.GetComponent<Button>();
                    buttonCache.onClick.AddListener(()
                        => keyPair.Value.OnClickProductContents(buttonCache, pTask));

                    if (!_objectUIContentsHash.ContainsKey(pTask))
                        _objectUIContentsHash[pTask] = new List<GameObject>();
                    _objectUIContentsHash[pTask].Add(cache);

                    cache.transform.localPosition = Vector3.back * 0.01f;
                    cache.transform.localRotation = Quaternion.identity;

                    cache.SetActive(false);
                });
            }
        });
    }
}
