using UnityEngine;
using UnityEngine.UI;

using PlayerKindom;
using PlayerKindom.PlayerKindomTypes;

public class UISocketContentsProperty : MonoBehaviour
{
    public Button UIButton => _contentsButton;
    public void SetSocket(GameObject socket) => _targetSocket = socket;

    [SerializeField]
    private Text _socketWeaponName = null;

    [SerializeField]
    private Image _socketWeaponImage = null;

    private readonly string _socketDefaultName = "Empty Socket";
    private ProductWrapper _socketedWeapon = null;
    private GameObject _targetSocket = null;

    private Button _contentsButton = null;

    private void Awake()
    {
        ClearSocket();
        _contentsButton = GetComponent<Button>();
    }

    public void AttachSocket(ProductionTask productData, ShipController ship)
    {
        if (_socketedWeapon != null)
            return;

        ProductWrapper product =
            PlayerKingdom.GetInstance().WeaponToSocket(productData, _targetSocket);

        if (product == null)
            return;

        product.Instance.GetComponent<WeaponController>().SetAttachedShip(ship);

        _socketedWeapon = product;

        _socketWeaponName.text = product.ProductData.TaskName;
        _socketWeaponImage.sprite = product.ProductData.TaskIcon;
    }

    public void ClearSocket()
    {
        _socketWeaponName.text = _socketDefaultName;
        _socketWeaponImage.sprite = null;

        if (_socketedWeapon != null)
        {
            _socketedWeapon.Instance.GetComponent<WeaponController>().DetachWeaponToShip();

            PlayerKingdom.GetInstance().WeaponToCargo(_socketedWeapon);
            _socketedWeapon = null;
        }
    }

    private void OnDisable()
    {
        _contentsButton.image.color = Color.white;
    }
}
