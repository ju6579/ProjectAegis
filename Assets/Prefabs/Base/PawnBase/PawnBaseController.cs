using UnityEngine;

using PlayerKindom;
using PlayerKindom.PlayerKindomTypes;
using System;
using System.Collections;

namespace Pawn
{
    public class PawnBaseController : MonoBehaviour
    {
        public static bool CompareType(GameObject pawn, PawnType type)
        {
            PawnBaseController pbc = pawn.GetComponent<PawnBaseController>();
            if (pbc == null)
                return false;

            return pbc.PawnActionType == type;
        }

        public PawnType PawnActionType = PawnType.NotSet;
        public GameObject TargetMeshAnchor = null;
        public GameObject SocketAnchor = null;
        public ProductWrapper PawnData;

        public ProjectPositionTracker ProjectedTarget = null;
        public bool bIsAttack = false;

        [SerializeField]
        private PawnProperty _pawnProperty = new PawnProperty();

        public void ApplyDamage(BulletMovement bullet)
        {
            int damage = bullet.Damage;
            damage = _pawnProperty.ShieldPoint - damage;

            if (damage < 0)
                _pawnProperty.ArmorPoint += damage;
            else
                _pawnProperty.ShieldPoint = damage;

            if (bullet.StoppingPower > Mathf.Epsilon && !bIsAttack)
            {
                bIsAttack = true;
                StartCoroutine(_RestoreAttack(bullet.StoppingPower));
            }

            if (_pawnProperty.ArmorPoint < 0)
            {
                if (PawnActionType == PawnType.SpaceShip || PawnActionType == PawnType.Weapon)
                    PlayerKingdom.GetInstance().ProductDestoryed(PawnData);

                GlobalObjectManager.ReturnToObjectPool(gameObject);
            }
        }

        private IEnumerator _RestoreAttack(float stoppingPower)
        {
            yield return new WaitForSeconds(stoppingPower);
            bIsAttack = false;
            yield return null;
        }
    }


    [Serializable]
    public class PawnProperty
    {
        public int ShieldPoint;
        public int ArmorPoint;
        public float AttackRestoreTime;
    }

    public enum PawnType
    {
        SpaceShip,
        Weapon,
        Enemy,
        Unit,
        NotSet
    }

    [Serializable]
    public class SpaceShipProperty
    {
        public float ShieldPoint;
        public float ArmorPoint;
        public float MaxMoveSpeed;
        public float Accelation;
        public float ArrivalTime;
    }

    [Serializable]
    public class WeaponProperty
    {
        public GameObject BulletObject;
        public int AttackCount;
        public float AttackDelay;
        public float ReloadDelay;
    }
}