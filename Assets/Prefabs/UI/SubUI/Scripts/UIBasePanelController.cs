using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBasePanelController : ListChangedObserveComponent<PlayerKingdom.ProductionTask, PlayerKingdom>
{
    [SerializeField]
    private ScrollRect _socketScrollRect = null;

    [SerializeField]
    private ScrollRect _weaponScrollRect = null;

    [SerializeField]
    private GameObject _socketUIContents = null;

    [SerializeField]
    private GameObject _productUIContents = null;

    [SerializeField]
    private GameObject _socketAttachButton = null;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void LoadList()
    {
        PlayerKingdom.GetInstance().ProductList.ForEach((PlayerKingdom.ProductionTask pTask) =>
        {
            if (PawnBaseController.CompareType(pTask.Product, PawnBaseController.PawnType.Weapon))
            {
                GameObject cache = Instantiate(_productUIContents, _weaponScrollRect.content);
                _ObjectUIContentsHash.Add(pTask, cache);

                cache.GetComponent<UIProductContentsProperty>().SetUIContentsData(pTask);

                cache.SetActive(false);
            }
        });

        Singleton<PlayerBaseShipController>.ListenSingletonLoaded(() =>
        {
            PlayerBaseShipController.GetInstance().Sockets.ForEach((GameObject socket) =>
            {
                GameObject cache = Instantiate(_socketUIContents, _socketScrollRect.content);
                UISocketContentsProperty contents = cache.GetComponent<UISocketContentsProperty>();

                contents.SetSocket(socket);
            });
        });
    }

    protected override void OnListChanged(PlayerKingdom.ProductionTask changed, bool isAdd)
    {
        base.OnListChanged(changed, isAdd);
        if(PawnBaseController.CompareType(changed.Product, PawnBaseController.PawnType.Weapon))
            _ObjectUIContentsHash[changed].SetActive(isAdd);
    }
}
