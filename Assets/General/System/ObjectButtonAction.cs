using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectButtonAction : MonoBehaviour
{
    protected enum ButtonType
    {
        CallSpaceShip
    }

    [SerializeField]
    private ButtonType buttonActionType = ButtonType.CallSpaceShip;

    public void OnButtonInteract(GameObject ship)
    {
        switch (buttonActionType)
        {
            case ButtonType.CallSpaceShip:
                CallSpaceShip(ship);
                break;

            default:
                break;
        }
    }

    private void CallSpaceShip(GameObject ship)
    {
        ProjectionManager.GetInstance().InstantiateShip(ship);
    }
}
