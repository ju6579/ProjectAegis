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
}