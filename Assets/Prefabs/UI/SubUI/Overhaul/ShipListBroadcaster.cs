using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipListBroadcaster : ListChangedObserveComponent<PlayerKingdom.ProductionTask, PlayerKingdom>
{
    [SerializeField]
    private GameObject _shipScrollContents = null;

    private static List<ScrollRect> _scrollContentsBroadcaster = new List<ScrollRect>();
    private static Dictionary<PlayerKingdom.ProductionTask, List<GameObject>> _objectUIContentsHash
        = new Dictionary<PlayerKingdom.ProductionTask, List<GameObject>>();

    public static void ListenShipListChange(ScrollRect targetScrollRect) => _scrollContentsBroadcaster.Add(targetScrollRect);

    protected override void LoadList()
    {
        PlayerKingdom.GetInstance().ProductList.ForEach((PlayerKingdom.ProductionTask pTask) =>
        {
            if (PawnBaseController.CompareType(pTask.Product, PawnBaseController.PawnType.SpaceShip))
            {
                _scrollContentsBroadcaster.ForEach((ScrollRect sr) =>
                {
                    GameObject cache = Instantiate(_shipScrollContents, sr.content);
                    cache.GetComponent<UIProductContentsProperty>().SetUIContentsData(pTask);

                    if (!_objectUIContentsHash.ContainsKey(pTask))
                        _objectUIContentsHash[pTask] = new List<GameObject>();
                    _objectUIContentsHash[pTask].Add(cache);

                    cache.SetActive(false);
                });
            }
        });
    }

    protected override void OnListChanged(PlayerKingdom.ProductionTask changed, bool isAdd)
    {
        base.OnListChanged(changed, isAdd);
        if (PawnBaseController.CompareType(changed.Product, PawnBaseController.PawnType.SpaceShip))
            _objectUIContentsHash[changed].ForEach((GameObject go) => go.SetActive(true));
    }

    protected override void Awake()
    {
        base.Awake();
    }
}
