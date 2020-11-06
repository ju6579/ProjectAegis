using UnityEngine;
using UnityEngine.UI;

public class UISocketContentsProperty : MonoBehaviour
{
    public void SetSocket(GameObject socket) => _targetSocket = socket;

    [SerializeField]
    private Text _socketWeaponName = null;

    [SerializeField]
    private Image _socketWeaponImage = null;

    private readonly string _socketDefaultName = "Empty Socket";
    private PlayerKingdom.ProductWrapper _socketedWeapon = null;
    private GameObject _targetSocket = null;

    private Button _contentsButton = null;

    private void Awake()
    {
        ClearSocket();

        _contentsButton = GetComponent<Button>();
        _contentsButton.onClick.AddListener(() => OnClickContentsButton());
    }

    public void AttachSocket(PlayerKingdom.ProductionTask productData)
    {
        PlayerKingdom.ProductWrapper product =
            PlayerKingdom.GetInstance().WeaponToSocket(productData, _targetSocket);

        if (product == null)
            return;

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
            PlayerKingdom.GetInstance().WeaponToCargo(_socketedWeapon);
        }
    }

    private void OnClickContentsButton()
    {
        if (PlayerUIController.SelectedUISocketContents != null)
            PlayerUIController.SelectedSocketButton.image.color = Color.white;

        _contentsButton.image.color = Color.black;

        PlayerUIController.SelectedSocketButton = _contentsButton;
        PlayerUIController.SelectedUISocketContents = this;
    }

    private void OnDisable()
    {
        _contentsButton.image.color = Color.white;
    }
}
