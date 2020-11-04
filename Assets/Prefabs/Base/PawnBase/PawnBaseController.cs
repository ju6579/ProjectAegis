using UnityEngine;

public class PawnBaseController : MonoBehaviour
{
    public enum PawnType
    {
        SpaceShip,
        Weapon,
        Enemy,
        Unit,
        NotSet
    }

    public PawnType PawnActionType = PawnType.NotSet;
    public GameObject TargetMeshAnchor = null;
    public PlayerKingdom.ProductWrapper PawnData;

    private void OnDestroy()
    {
        if (PawnActionType == PawnType.SpaceShip || PawnActionType == PawnType.Weapon)
            PlayerKingdom.GetInstance().ProductDestoryed(PawnData);
    }
}