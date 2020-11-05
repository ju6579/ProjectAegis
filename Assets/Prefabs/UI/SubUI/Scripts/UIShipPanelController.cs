using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShipPanelController : ListChangedObserveComponent<PlayerKingdom.ProductWrapper, PlayerKingdom>
{
    [SerializeField]
    private ScrollRect _shipCargoScrollView = null;

    [SerializeField]
    private GameObject _shipScrollDataContents = null;

    protected override void Awake()
    {
        base.Awake();

    }

    protected override void OnListChanged(PlayerKingdom.ProductWrapper changed, bool isAdd)
    {
        base.OnListChanged(changed, isAdd);

        if(isAdd)
        {
            CreateShipDataContentsOnScroll(changed);
        }
        else
        {
            GameObject cache = _objectUIContentsHash[changed];
            _objectUIContentsHash.Remove(changed);
            Destroy(cache);
        }
    }

    protected override void LoadList()
    {
        PlayerKingdom.GetInstance().CargoList.ForEach((PlayerKingdom.ProductWrapper product) =>
        {
            CreateShipDataContentsOnScroll(product);
        });
    }

    private void CreateShipDataContentsOnScroll(PlayerKingdom.ProductWrapper product)
    {
        if(!PawnBaseController.CompareType(product.Instance, PawnBaseController.PawnType.SpaceShip))
        {
            GlobalLogger.CallLogError(product.ProductData.TaskName, GErrorType.InspectorValueException);
            return;
        }

        GameObject cache = Instantiate(_shipScrollDataContents, _shipCargoScrollView.content);
        Button buttonCache = cache.GetComponent<Button>();

        cache.GetComponent<UIShipDataContentsProperty>().SetUIContentsData(product);

        buttonCache.onClick.AddListener(() => OnClickShipDataContents());
        _objectUIContentsHash.Add(product, cache);
    }

    private void OnClickShipDataContents()
    {

    }
}