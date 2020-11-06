using UnityEngine;

public class PawnBaseController : MonoBehaviour
{
    public static bool CompareType(GameObject pawn, PawnType type)
    {
        PawnBaseController pbc = pawn.GetComponent<PawnBaseController>();
        if (pbc == null) 
            return false;

        return pbc.PawnActionType == type;
    }

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
    public GameObject SocketAnchor = null;
    public PlayerKingdom.ProductWrapper PawnData;

    public ProjectPositionTracker ProjectedTarget = null;

    private void OnDestroy()
    {
        if (PawnActionType == PawnType.SpaceShip || PawnActionType == PawnType.Weapon)
            PlayerKingdom.GetInstance().ProductDestoryed(PawnData);
    }
}