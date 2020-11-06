using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomLibrary : MonoBehaviour
{
    public static void OnClickAttachWeapon()
    {
        if(PlayerUIController.SelectedWeaponTask != null && PlayerUIController.SelectedUISocketContents)
            PlayerUIController.SelectedUISocketContents.AttachSocket(PlayerUIController.SelectedWeaponTask);
    }

    public static void OnClickLaunchShip()
    {
        if (PlayerUIController.SelectedShip.Instance != null)
            PlayerKingdom.GetInstance().ShipToField(PlayerUIController.SelectedShip);
        PlayerUIController.SelectedShip = null;
    }
}