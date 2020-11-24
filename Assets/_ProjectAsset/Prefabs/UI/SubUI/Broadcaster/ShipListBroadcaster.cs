﻿using System.Collections;
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

    private static List<KeyValuePair<ScrollRect, IUIContentsCallbacks>> _scrollContentsBroadcaster
        = new List<KeyValuePair<ScrollRect, IUIContentsCallbacks>>();

    private static Dictionary<ProductionTask, List<GameObject>> _objectUIContentsHash
        = new Dictionary<ProductionTask, List<GameObject>>();

    public static void ListenShipListChanged(ScrollRect targetScrollRect, IUIContentsCallbacks callback)
    {
        _scrollContentsBroadcaster.Add(new KeyValuePair<ScrollRect, IUIContentsCallbacks>(targetScrollRect, callback));
    }

    protected override void LoadList()
    {
        PlayerKingdom.GetInstance().ProductList.ForEach((ProductionTask pTask) =>
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
}
