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

    private Button _selectedSocketButton = null;
    private UISocketContentsProperty _selectedSocketProperty = null;

    private Button _selectedProductButton = null;
    private PlayerKingdom.ProductionTask _selectedTask = null;

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
                _objectUIContentsHash.Add(pTask, cache);

                cache.GetComponent<UIProductContentsProperty>().SetUIContentsData(pTask);

                Button cacheButton = cache.GetComponent<Button>();
                cacheButton.onClick.AddListener(() => OnClickProductionContents(pTask, cacheButton));

                cache.SetActive(false);
            }
        });

        Singleton<PlayerBaseShipController>.ListenSingletonLoaded(() =>
        {
            PlayerBaseShipController.GetInstance().Sockets.ForEach((GameObject socket) =>
            {
                GameObject cache = Instantiate(_socketUIContents, _socketScrollRect.content);
                UISocketContentsProperty contents = cache.GetComponent<UISocketContentsProperty>();
                Button cacheButton = cache.GetComponent<Button>();

                cacheButton.onClick.AddListener(() => OnClickSocketContents(contents, cacheButton));
                contents.SetSocket(socket);
            });
        });

        _socketAttachButton.GetComponent<Button>().onClick.AddListener(() => OnClickAttachButton());
    }

    protected override void OnListChanged(PlayerKingdom.ProductionTask changed, bool isAdd)
    {
        base.OnListChanged(changed, isAdd);
        if(PawnBaseController.CompareType(changed.Product, PawnBaseController.PawnType.Weapon))
            _objectUIContentsHash[changed].SetActive(isAdd);
    }

    private void OnClickProductionContents(PlayerKingdom.ProductionTask pTask, Button uiButton)
    {
        if (_selectedProductButton != null)
            _selectedProductButton.image.color = Color.white;
        uiButton.image.color = Color.black;

        _selectedProductButton = uiButton;
        _selectedTask = pTask;
    }

    private void OnClickSocketContents(UISocketContentsProperty socketProperty, Button uiButton)
    {
        if (_selectedSocketButton != null)
            _selectedSocketButton.image.color = Color.white;
        uiButton.image.color = Color.black;

        _selectedSocketButton = uiButton;
        _selectedSocketProperty = socketProperty;
    }

    private void OnClickAttachButton()
    {
        if(_selectedTask != null && _selectedSocketProperty != null)
        {
            _selectedSocketProperty.AttachSocket(_selectedTask);
        }
    }
}
