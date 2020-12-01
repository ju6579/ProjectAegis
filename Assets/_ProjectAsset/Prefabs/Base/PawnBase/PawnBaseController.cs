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

        public void PlayHideToShowEffect() => StartCoroutine(_HideToShowDissolveEffect());

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
                if (PawnActionType == PawnType.SpaceShip)
                {
                    ShipController ship = GetComponent<ShipController>();

                    ship.OnShipDestroy();
                    PlayerKingdom.GetInstance().ProductDestoryed(ship.ShipProduct);
                }

                AudioSourceManager.GetInstance().RequestPlayAudioByType(SFXType.ShipExplosion);
                GlobalEffectManager.GetInstance().PlayEffectByTypeAndScale(VFXType.ShipExplosion, transform.position, _explosionSize);

                _dissolveRenderer.SetPropertyBlock(null);

                GlobalObjectManager.ReturnToObjectPool(gameObject);
            }

            if (_dissolveRenderer != null)
            {
                float hurtAmount = 1f - (float)_pawnProperty.ArmorPoint / (float)_pawnPropertyOrigin.ArmorPoint;
                Debug.Log(hurtAmount);
                _materialPropertyHandler.SetFloat("_HurtAmount", hurtAmount);
                _dissolveRenderer.SetPropertyBlock(_materialPropertyHandler);
            }
        }

        public PawnType PawnActionType = PawnType.NotSet;
        public GameObject TargetMeshAnchor = null;
        public GameObject SocketAnchor = null;

        public ProjectPositionTracker ProjectedTarget = null;
        public bool bIsAttack = false;

        [SerializeField]
        private PawnProperty _pawnProperty = new PawnProperty();

        [SerializeField]
        private MeshRenderer _dissolveRenderer = null;

        [SerializeField]
        private Vector3 _explosionSize = Vector3.zero;

        private PawnProperty _pawnPropertyOrigin = new PawnProperty();
        private MaterialPropertyBlock _materialPropertyHandler = null;

        private void Awake()
        {
            _pawnPropertyOrigin.CopyProperty(_pawnProperty);

            if(_dissolveRenderer != null)
                _materialPropertyHandler = new MaterialPropertyBlock();
        }

        private void OnEnable()
        {
            _pawnProperty.CopyProperty(_pawnPropertyOrigin);

            if (_dissolveRenderer != null)
            {
                _materialPropertyHandler.Clear();
                _materialPropertyHandler.SetFloat("_HurtAmount", 0);
                _dissolveRenderer.SetPropertyBlock(_materialPropertyHandler);
            }
                
            if (ProjectedTarget != null)
                ProjectedTarget.gameObject.SetActive(true);
        }

        private IEnumerator _RestoreAttack(float stoppingPower)
        {
            yield return new WaitForSeconds(stoppingPower);
            bIsAttack = false;
            yield return null;
        }

        private static readonly float DISSOLVEMAXVALUE = 1f;
        private static readonly float DISSOLVEMINVALUE = -1f;
        private static readonly float DISSOLVESPENDTIME = 2F;
        private static readonly float DISSOLVETIMESPEED = 0.01f;
        private WaitForSeconds _dissolveSpendWait = new WaitForSeconds(DISSOLVETIMESPEED);
        private IEnumerator _HideToShowDissolveEffect()
        {
            _materialPropertyHandler.SetFloat("_DissolvePivot", DISSOLVEMINVALUE);
            _dissolveRenderer.SetPropertyBlock(_materialPropertyHandler);

            float dissolveRate = 1f / (DISSOLVESPENDTIME / DISSOLVETIMESPEED) * 2;

            for (int i = 0; i < DISSOLVESPENDTIME / DISSOLVETIMESPEED; i++)
            {
                _materialPropertyHandler.SetFloat("_DissolvePivot", DISSOLVEMINVALUE + i * dissolveRate);
                _dissolveRenderer.SetPropertyBlock(_materialPropertyHandler);

                yield return _dissolveSpendWait;
            }

            yield return null;
        }

        private IEnumerator _ShowToHideDissolveEffet()
        {
            yield return null;
        }
    }


    [Serializable]
    public class PawnProperty
    {
        public int ShieldPoint;
        public int ArmorPoint;
        public float AttackRestoreTime;

        public void CopyProperty(PawnProperty pawn)
        {
            this.ShieldPoint = pawn.ShieldPoint;
            this.ArmorPoint = pawn.ArmorPoint;
            this.AttackRestoreTime = pawn.AttackRestoreTime;
        }
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
        public BulletType BulletActionType;
        public GameObject BulletObject;
        public int BulletDamage;
        public int AttackCount;
        public float AttackDelay;
        public float ReloadDelay;
    }
}