using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class UISocketContentsProperty : MonoBehaviour
{
    public void SetSocket(GameObject socket) => _targetSocket = socket;

    [SerializeField]
    private Text _socketWeaponName = null;

    [SerializeField]
    private Image _socketWeaponImage = null;

    [SerializeField]
    private Button _socketDetachButton = null;

    private readonly string _socketDefaultName = "Empty Socket";
    private PlayerKingdom.ProductWrapper _socketedWeapon;
    private GameObject _targetSocket = null;

    private void Awake()
    {
        ClearSocket();
        _socketDetachButton.onClick.AddListener(() => ClearSocket());
    }

    public void AttachSocket(PlayerKingdom.ProductionTask productData)
    {
        PlayerKingdom.ProductWrapper product =
            PlayerKingdom.GetInstance().WeaponToSocket(productData, _targetSocket);
        if (product.Instance == null)
            return;

        _socketedWeapon = product;

        _socketWeaponName.text = product.ProductData.TaskName;
        _socketWeaponImage.sprite = product.ProductData.TaskIcon;
    }

    public void ClearSocket()
    {
        _socketWeaponName.text = _socketDefaultName;
        _socketWeaponImage.sprite = null;

        if (_socketedWeapon.Instance != null)
        {
            PlayerKingdom.GetInstance().WeaponToCargo(_socketedWeapon);
        }
    }
}
