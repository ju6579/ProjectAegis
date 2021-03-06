﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PlayerKindom;
using PlayerKindom.PlayerKindomTypes;
using Pawn;

public class CargoShipListBroadcaster : ListChangedObserveComponent<ProductWrapper, PlayerKingdom>
{
    [SerializeField]
    private GameObject _shipScrollContents = null;

    private List<KeyValuePair<ScrollRect, IUIContentsCallbacks>> _scrollContentsBroadcaster 
        = new List<KeyValuePair<ScrollRect, IUIContentsCallbacks>>();

    private static Dictionary<ProductWrapper, List<GameObject>> _objectUIContentsHash
        = new Dictionary<ProductWrapper, List<GameObject>>();

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _scrollContentsBroadcaster.Clear();
        _objectUIContentsHash.Clear();
    }

    // Chain Delegate Function for Listen Contents Change
    public void ListenCargoShipListChanged(ScrollRect targetScrollRect, IUIContentsCallbacks callback)
    {
        _scrollContentsBroadcaster.Add(new KeyValuePair<ScrollRect, IUIContentsCallbacks>(targetScrollRect, callback));
    }

    // Initialize and Setup List for UI
    protected override void LoadList()
    {
        PlayerKingdom.GetInstance().ShipCargoList.ForEach((ProductWrapper product) =>
        {
            AddContentsToAllScrollView(product);
        });
    }

    protected override void OnListChanged(ProductWrapper changed, bool isAdd)
    {
        base.OnListChanged(changed, isAdd);
        if(PawnBaseController.CompareType(changed.ProductData.Product, PawnType.SpaceShip))
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

    private void AddContentsToAllScrollView(ProductWrapper product)
    {
        _scrollContentsBroadcaster.ForEach((KeyValuePair<ScrollRect, IUIContentsCallbacks> keyPair) =>
        {
            GameObject cache = Instantiate(_shipScrollContents, keyPair.Key.content);
            cache.GetComponent<UIShipDataContentsProperty>().SetUIContentsData(product);

            Button buttonCache = cache.GetComponent<Button>();
            buttonCache.onClick.AddListener(()
                => keyPair.Value.OnClickShipDataContents(buttonCache, product));

            if (!_objectUIContentsHash.ContainsKey(product))
                _objectUIContentsHash[product] = new List<GameObject>();
            _objectUIContentsHash[product].Add(cache);

            cache.transform.localPosition = Vector3.back * 0.01f;
            cache.transform.localRotation = Quaternion.identity;

            cache.SetActive(false);
        });
    }

    protected override void Awake()
    {
        base.Awake();
    }
}
