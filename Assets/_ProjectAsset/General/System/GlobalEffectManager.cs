using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pawn;

public class GlobalEffectManager : Singleton<GlobalEffectManager>
{
    public void PlayEffectByType(VFXType vtype, Vector3 targetPosition) => StartCoroutine(_PlayEffectByLifeTime(vtype, targetPosition));
    public void PlayEffectByTypeAndScale(VFXType vtype, Vector3 targetPosition, Vector3 scale)
        => StartCoroutine(_PlayEffectByLifeTimeAndScale(vtype, targetPosition, scale));

    [SerializeField]
    private List<ParticleObject> _particleEffectList = new List<ParticleObject>();

    [SerializeField]
    private int _poolExtendSize = 5;

    private Dictionary<VFXType, Queue<ParticleSystem>> _particleEffectPool = new Dictionary<VFXType, Queue<ParticleSystem>>();
    private Dictionary<VFXType, ParticleObject> _particleTypeHash = new Dictionary<VFXType, ParticleObject>();

    private Dictionary<VFXType, WaitForSeconds> _particleEffectWait = new Dictionary<VFXType, WaitForSeconds>();
    private WaitForEndOfFrame _frameWait = new WaitForEndOfFrame();

    protected override void Awake()
    {
        _particleEffectList.ForEach((ParticleObject particle) =>
        {
            _particleEffectPool.Add(particle.EffectType, new Queue<ParticleSystem>());

            _particleTypeHash.Add(particle.EffectType, particle);
            ExtendPoolSize(particle.EffectType);

            _particleEffectWait.Add(particle.EffectType, new WaitForSeconds(particle.EffectSystem.main.duration));
        });

        base.Awake();
    }

    private void ExtendPoolSize(VFXType vtype)
    {
        GameObject cache = null;
        ParticleSystem ps = null;

        for (int i = 0; i < _poolExtendSize; i++)
        {
            cache = Instantiate(_particleTypeHash[vtype].EffectHolder);
            ps = cache.GetComponent<ParticleSystem>();

            _particleEffectPool[vtype].Enqueue(ps);

            cache.SetActive(false);
        }
    }

    private ParticleSystem GetEffectByPool(VFXType vtype)
    {
        if (_particleEffectPool[vtype].Count <= 0)
            ExtendPoolSize(vtype);

        ParticleSystem effect = _particleEffectPool[vtype].Dequeue();
        effect.gameObject.SetActive(true);

        return effect;
    }

    private void ReturnEffectToPool(VFXType vtype, ParticleSystem target)
    {
        target.gameObject.SetActive(false);
        _particleEffectPool[vtype].Enqueue(target); 
    } 

    private IEnumerator _PlayEffectByLifeTime(VFXType vtype, Vector3 position)
    {
        ParticleSystem effect = GetEffectByPool(vtype);
        effect.gameObject.transform.position = position;
        effect.Play();

        yield return _particleEffectWait[vtype];

        effect.Stop();
        ReturnEffectToPool(vtype, effect);
    }

    private IEnumerator _PlayEffectByLifeTimeAndScale(VFXType vtype, Vector3 position, Vector3 scale)
    {
        ParticleSystem effect = GetEffectByPool(vtype);
        effect.gameObject.transform.position = position;
        effect.gameObject.transform.localScale = scale;
        effect.Play();

        yield return _particleEffectWait[vtype];

        effect.Stop();
        ReturnEffectToPool(vtype, effect);
    }

    private IEnumerator _PlayEffectByHolder(VFXType vtype, GameObject target, PawnBaseController pawn) 
    {
        if (pawn != null)
            target = pawn.gameObject;

        ParticleSystem effect = GetEffectByPool(vtype);
        effect.Play();

        while (target.activeInHierarchy)
        {
            effect.gameObject.transform.position = target.transform.position;

            yield return _frameWait;
        }

        effect.Stop();
        ReturnEffectToPool(vtype, effect);

        yield return null;
    }

    [Serializable]
    public struct ParticleObject
    {
        public VFXType EffectType;
        public GameObject EffectHolder;
        public ParticleSystem EffectSystem;
    }
}

public enum VFXType
{
    ShipExplosion,
    MissileExplosion,
    ShipFire
}