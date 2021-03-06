﻿using UnityEngine;

public class ObjectButtonAction : MonoBehaviour
{
    public enum ButtonType
    {
        ProductionSpaceShip,
        WarpSpaceShip
    }

    [SerializeField]
    private ButtonType _buttonActionType = ButtonType.ProductionSpaceShip;

    public void OnButtonInteract(GameObject ship)
    {
        Debug.Log(name + " Click");
        switch (_buttonActionType)
        {
            case ButtonType.ProductionSpaceShip:
                //PlayerKingdom.GetInstance().RequestTaskToKingdom();
                break;

            case ButtonType.WarpSpaceShip:
                //PlayerKingdom.GetInstance().RequestShipWarpToKingdom(ship);
                break;

            default:
                break;
        }
    }
}
