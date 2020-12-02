using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceManager : Singleton<AudioSourceManager>
{
    public void RequestPlayAudioByType(SFXType soundType)
    {
        switch (soundType)
        {
            case SFXType.Laser:
                _laserAudioHandler.RequestPlay();
                break;

            case SFXType.MissileHit:
                _missileHitAudioHandler.RequestPlay();
                break;

            case SFXType.QuadProjectile:
                _quadProjectileAudioHandler.RequestPlay();
                break;

            case SFXType.ShipExplosion:
                _shipExplosionAudioHandler.RequestPlay();
                break;

            case SFXType.TripleProjectile:
                _tripleProjectileAudioHandler.RequestPlay();
                break;
        }
    }

    public void RequestPlayAudioByBullet(BulletType bulletType, int bulletCount)
    {
        switch (bulletType)
        {
            case BulletType.Laser:
                RequestPlayAudioByType(SFXType.Laser);
                break;

            case BulletType.Missile:
                break;

            case BulletType.Projectile:
                if (bulletCount == 3)
                    RequestPlayAudioByType(SFXType.TripleProjectile);
                else if (bulletCount == 4)
                    RequestPlayAudioByType(SFXType.QuadProjectile);
                break;
        }
    }

    [SerializeField]
    private AudioClip _tripleProjectileClip;
    private AudioSourceHandler _tripleProjectileAudioHandler = null;

    [SerializeField]
    private AudioClip _quadProjectileClip;
    private AudioSourceHandler _quadProjectileAudioHandler = null;

    [SerializeField]
    private AudioClip _laserClip;
    private AudioSourceHandler _laserAudioHandler = null;

    [SerializeField]
    private AudioClip _missileHitClip;
    private AudioSourceHandler _missileHitAudioHandler = null;

    [SerializeField]
    private AudioClip _shipExplosionClip;
    private AudioSourceHandler _shipExplosionAudioHandler = null;

    protected override void Awake()
    {
        _tripleProjectileAudioHandler = new AudioSourceHandler(this, 3, _tripleProjectileClip, 0.20f, 0, 1);
        _quadProjectileAudioHandler = new AudioSourceHandler(this, 3, _quadProjectileClip, 0.35f, 0.3f, 1);
        _laserAudioHandler = new AudioSourceHandler(this, 3, _laserClip, 1f, 0, 1);
        _missileHitAudioHandler = new AudioSourceHandler(this, 2, _missileHitClip, 0.5f, 0.1f, 1);
        _shipExplosionAudioHandler = new AudioSourceHandler(this, 4, _shipExplosionClip, 1f, 0.6f, 1);

        base.Awake();
    }

    private class AudioSourceHandler
    {
        public void RequestPlay()
        {
            if(_sourceQueue.Count > 0)
            {
                AudioSource source = _sourceQueue.Dequeue();
                _parentComponent.StartCoroutine(_PlaySound(source));
            }
        }

        private Queue<AudioSource> _sourceQueue = new Queue<AudioSource>();
        private AudioClip _audioClip = null;
        private AudioSourceManager _parentComponent = null;
        private WaitForSeconds _playWait = null;

        public AudioSourceHandler(AudioSourceManager targetManager, 
                               int maxPlayCount,
                               AudioClip targetCilp,
                               float volume,
                               float reverb,
                               float pitch)
        {
            _audioClip = targetCilp;
            _parentComponent = targetManager;
            _playWait = new WaitForSeconds(targetCilp.length);

            AudioSource source = null;
            for (int i = 0; i < maxPlayCount; i++)
            {
                source = (AudioSource)targetManager.gameObject.AddComponent<AudioSource>();

                source.clip = targetCilp;
                source.volume = volume;
                source.reverbZoneMix = reverb;
                source.pitch = pitch;

                _sourceQueue.Enqueue(source);
            }
        }

        private IEnumerator _PlaySound(AudioSource source)
        {
            source.Play();
            yield return _playWait;
            _sourceQueue.Enqueue(source);
        }
    }
}

public enum SFXType
{
    TripleProjectile,
    QuadProjectile,
    Laser,
    MissileHit,
    ShipExplosion
}