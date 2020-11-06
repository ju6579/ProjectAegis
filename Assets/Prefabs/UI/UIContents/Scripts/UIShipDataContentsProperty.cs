using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShipDataContentsProperty : MonoBehaviour
{
    [SerializeField]
    private Image _shipImage = null;

    [SerializeField]
    private Text _shipName = null;

    [SerializeField]
    private Text _shieldAmount = null;

    [SerializeField]
    private Text _armorAmount = null;

    [SerializeField]
    private Text _arrivalTime = null;

    private Button _contentsButton = null;

    private PlayerKingdom.ProductWrapper _shipSet;
    private ShipController.ShipProperty _shipProperty;
    private List<GameObject> _shipSocketUIContents = new List<GameObject>();

    private void Awake()
    {
        _contentsButton = GetComponent<Button>();
        _contentsButton.onClick.AddListener(() => OnClickShipData());
    }

    public void SetUIContentsData(PlayerKingdom.ProductWrapper product)
    {
        _shipSet = product;

        _shipName.text = product.ProductData.TaskName;
        _shipImage.sprite = product.ProductData.TaskIcon;

        if (!PawnBaseController.CompareType(_shipSet.Instance, PawnBaseController.PawnType.SpaceShip))
        {
            GlobalLogger.CallLogError(_shipSet.ProductData.TaskName, GErrorType.InspectorValueException);
            this.gameObject.SetActive(false);
            return;
        }

        _shipProperty = _shipSet.Instance.GetComponent<ShipController>().ShipData;

        PlayerUIController.GetInstance().StartCoroutine(_ObserveShipData());
    }

    private void UpdateUIContents()
    {
        _shieldAmount.text = _shipProperty.ShieldPoint.ToString();
        _armorAmount.text = _shipProperty.ArmorPoint.ToString();
        _arrivalTime.text = _shipProperty.ArrivalTime.ToString();
    }

    private void OnClickShipData()
    {
        _shipSocketUIContents.ForEach((GameObject go) => go.SetActive(true));
    }

    private IEnumerator _ObserveShipData()
    {
        WaitForEndOfFrame frameWait = new WaitForEndOfFrame();
        while(this != null)
        {
            UpdateUIContents();

            yield return frameWait;
        }
        yield return null;
    }

    private void OnDisable()
    {
        _contentsButton.image.color = Color.white;
    }
}
