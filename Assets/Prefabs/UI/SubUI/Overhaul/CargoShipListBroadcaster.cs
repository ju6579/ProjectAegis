using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CargoShipListBroadcaster : ListChangedObserveComponent<PlayerKingdom.ProductWrapper, PlayerKingdom>
{
    [SerializeField]
    private GameObject _shipScrollContents = null;

    private static List<ScrollRect> _scrollContentsBroadcaster = new List<ScrollRect>();
    private static Dictionary<PlayerKingdom.ProductWrapper, List<GameObject>> _objectUIContentsHash
        = new Dictionary<PlayerKingdom.ProductWrapper, List<GameObject>>();

    public static void ListenCargoShipListChanged(ScrollRect targetScrollRect) => _scrollContentsBroadcaster.Add(targetScrollRect);

    protected override void LoadList()
    {
        PlayerKingdom.GetInstance().ShipCargoList.ForEach((PlayerKingdom.ProductWrapper product) =>
        {
            AddContentsToAllScrollView(product);
        });
    }

    protected override void OnListChanged(PlayerKingdom.ProductWrapper changed, bool isAdd)
    {
        base.OnListChanged(changed, isAdd);
        if(PawnBaseController.CompareType(changed.ProductData.Product, PawnBaseController.PawnType.SpaceShip))
        {
            if (isAdd)
            {
                if (!_objectUIContentsHash.ContainsKey(changed))
                    AddContentsToAllScrollView(changed);
                _objectUIContentsHash[changed].ForEach((GameObject go) => go.SetActive(true));
            }
            else
            {
                _objectUIContentsHash[changed].ForEach((GameObject go) => go.SetActive(false));
            }
        }
    }

    private void AddContentsToAllScrollView(PlayerKingdom.ProductWrapper product)
    {
        _scrollContentsBroadcaster.ForEach((ScrollRect sr) =>
        {
            GameObject cache = Instantiate(_shipScrollContents, sr.content);
            cache.GetComponent<UIShipDataContentsProperty>().SetUIContentsData(product);

            if (!_objectUIContentsHash.ContainsKey(product))
                _objectUIContentsHash[product] = new List<GameObject>();
            _objectUIContentsHash[product].Add(cache);

            cache.SetActive(false);
        });
    }

    protected override void Awake()
    {
        base.Awake();
    }
}
