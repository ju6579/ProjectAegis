using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponListBroadcaster : ListChangedObserveComponent<PlayerKingdom.ProductionTask, PlayerKingdom>
{
    [SerializeField]
    private GameObject _weaponScrollContents = null;

    private static List<KeyValuePair<ScrollRect, IUIContentsCallbacks>> _scrollContentsBroadcaster
        = new List<KeyValuePair<ScrollRect, IUIContentsCallbacks>>();

    private static Dictionary<PlayerKingdom.ProductionTask, List<GameObject>> _objectUIContentsHash
        = new Dictionary<PlayerKingdom.ProductionTask, List<GameObject>>();

    public static void ListenWeaponListChanged(ScrollRect targetScrollRect, IUIContentsCallbacks callback)
    {
        _scrollContentsBroadcaster.Add(new KeyValuePair<ScrollRect, IUIContentsCallbacks>(targetScrollRect, callback));
    }

    protected override void LoadList()
    {
        PlayerKingdom.GetInstance().ProductList.ForEach((PlayerKingdom.ProductionTask pTask) =>
        {
            if(PawnBaseController.CompareType(pTask.Product, PawnBaseController.PawnType.Weapon))
            {
                _scrollContentsBroadcaster.ForEach((KeyValuePair<ScrollRect, IUIContentsCallbacks> keyPair) =>
                {
                    GameObject cache = Instantiate(_weaponScrollContents, keyPair.Key.content);
                    cache.GetComponent<UIProductContentsProperty>().SetUIContentsData(pTask);

                    Button buttonCache = cache.GetComponent<Button>();
                    buttonCache.onClick.AddListener(() 
                        => keyPair.Value.OnClickProductContents(buttonCache, pTask));

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
        if(PawnBaseController.CompareType(changed.Product, PawnBaseController.PawnType.Weapon))
            _objectUIContentsHash[changed].ForEach((GameObject go) => go.SetActive(true));
    }

    protected override void Awake()
    {
        base.Awake();
    }
}