using Pawn;
using System.Collections;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public int Damage => _bulletDamage;
    
    public float StoppingPower => _bulletStoppingPower;

    public void SetBulletProperty(LayerMask targetLayer, Material material, bool isShootByPlayer, int damage, Transform target)
    {
        _targetLayer = targetLayer;
        _bulletDamage = damage;
        _missileTarget = target;

        switch (_bulletType)
        {
            case BulletType.Projectile:
                _targetMeshRenderer.material = material;
                break;

            case BulletType.Laser:
                _lineRenderer.material = material;
                break;

            case BulletType.Missile:
                _targetMeshRenderer.material = material;
                break;
        }
    } 

    [SerializeField]
    private BulletType _bulletType = BulletType.NotSet;

    [SerializeField]
    private float _bulletSpeed = 1f;
    
    [SerializeField]
    private float _bulletStoppingPower = 0f;

    [SerializeField]
    private MeshRenderer _targetMeshRenderer = null;

    [SerializeField]
    private LineRenderer _lineRenderer = null;

    [SerializeField]
    private float _preProcessTimeAmount = 1f;

    private Transform _missileTarget = null;

    private LayerMask _targetLayer = -1;
    private RaycastHit _hitInfo;
    private Ray _ray;

    private WaitForSeconds _metalLifeTime = new WaitForSeconds(1f);
    private WaitForSeconds _laserLifeTime = new WaitForSeconds(0.3f);
    private WaitForSeconds _missileLifeTime = new WaitForSeconds(5.0f);
    private WaitForEndOfFrame _frameWait = new WaitForEndOfFrame();
    
    private bool _isChecked = false;
    private int _bulletDamage = 1;
    private bool _IsShootByPlayer = false;
    private float _missileTimeStamp = 0f;

    private Vector3 _randomUpDirection = Vector3.up;

    private void OnEnable()
    {
        _missileTimeStamp = _preProcessTimeAmount;
        _randomUpDirection = Random.rotation * Vector3.up;

        _isChecked = false;
        StartCoroutine(_Lifetime());
        _missileTimeStamp = Time.time;
    }

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        if (_bulletType != BulletType.Laser)
            _lineRenderer.enabled = false; 

        _ray = new Ray(transform.position, transform.forward);
    }

    private void Update()
    {
        switch (_bulletType)
        {
            case BulletType.Laser:
                break;


            case BulletType.Projectile:
                transform.localPosition += transform.forward * Time.deltaTime * _bulletSpeed;
                break;


            case BulletType.Missile:
                if (_missileTarget == null)
                {
                    _missileTarget = null;
                    GlobalObjectManager.ReturnToObjectPool(gameObject);
                }

                if(_missileTarget.gameObject.activeInHierarchy)
                    transform.LookAt(_missileTarget);

                transform.localPosition += (transform.forward + _randomUpDirection * _missileTimeStamp).normalized
                                       * Time.deltaTime * _bulletSpeed;

                _missileTimeStamp -= Time.deltaTime;
                _missileTimeStamp = Mathf.Clamp(_missileTimeStamp, 0, _preProcessTimeAmount);
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (_bulletType)
        {
            case BulletType.Projectile:
                ProjectileHitCheck();
                break;

            case BulletType.Laser:
                LaserHitCheck();
                break;

            case BulletType.Missile:
                MissileHitCheck();
                break;
        }
    }

    private void ProjectileHitCheck()
    {
        _ray.origin = transform.position;
        _ray.direction = transform.forward;

        if (Physics.Raycast(_ray, out _hitInfo, _bulletSpeed * 20f, _targetLayer))
        {
            PawnBaseController pbc = _hitInfo.collider.gameObject.GetComponentInParent<PawnBaseController>();
            pbc.ApplyDamage(this);

            GlobalObjectManager.ReturnToObjectPool(gameObject);
        }
    }

    private void LaserHitCheck()
    {
        if (!_isChecked)
        {
            _ray.origin = transform.position;
            _ray.direction = transform.forward;

            Vector3 _hitPoint = transform.position + transform.forward * 10000f;

            if (Physics.Raycast(_ray, out _hitInfo, 10000f, _targetLayer))
            {
                PawnBaseController pbc = _hitInfo.collider.gameObject.GetComponentInParent<PawnBaseController>();
                pbc.ApplyDamage(this);

                _hitPoint = _hitInfo.point;
            }

            StartCoroutine(_LaserDraw(transform.position, _hitPoint));
        }

        _isChecked = true;
    }

    private void MissileHitCheck()
    {
        _ray.origin = transform.position;
        _ray.direction = transform.forward;

        if (Physics.Raycast(_ray, out _hitInfo, _bulletSpeed * 20f, _targetLayer))
        {
            PawnBaseController pbc = _hitInfo.collider.gameObject.GetComponentInParent<PawnBaseController>();
            pbc.ApplyDamage(this);

            AudioSourceManager.GetInstance().RequestPlayAudioByType(SFXType.MissileHit);
            GlobalEffectManager.GetInstance().PlayEffectByType(VFXType.MissileExplosion, transform.position);

            GlobalObjectManager.ReturnToObjectPool(gameObject);
        }
    }

    private IEnumerator _LaserDraw(Vector3 startPosition, Vector3 endPosition)
    {
        _lineRenderer.SetPosition(0, startPosition);
        _lineRenderer.SetPosition(1, endPosition);

        yield return null;
    }

    private IEnumerator _Lifetime()
    {
        switch(_bulletType)
        {
            case BulletType.Projectile:
                yield return _metalLifeTime;
                break;
            case BulletType.Laser:
                yield return _laserLifeTime;
                break;
            case BulletType.Missile:
                yield return _missileLifeTime;
                break;
        }

        _missileTarget = null;
        GlobalObjectManager.ReturnToObjectPool(gameObject);
        yield return null;
    }
}

public enum BulletType
{
    Projectile,
    Laser,
    Missile,
    NotSet
}