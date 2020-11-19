using Pawn;
using System.Collections;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public int Damage => _bulletDamage;
    
    public float StoppingPower => _bulletStoppingPower;

    public void SetBulletProperty(LayerMask targetLayer, Material _material, bool isShootByPlayer, int damage)
    {
        _targetLayer = targetLayer;
        _bulletDamage = damage;

        switch (_bulletType)
        {
            case BulletType.Projectile:
                _targetMeshRenderer.material = _material;
                break;

            case BulletType.Laser:
                _lineRenderer.material = _material;
                break;

            case BulletType.Missile:
                _targetMeshRenderer.material = _material;
                break;
        }
    } 

    private enum BulletType
    {
        Projectile,
        Laser,
        Missile,
        NotSet
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

    private LayerMask _targetLayer = 0;
    private RaycastHit _hitInfo;
    private Ray _ray;

    private WaitForSeconds _metalLifeTime = new WaitForSeconds(1f);
    private WaitForSeconds _laserLifeTime = new WaitForSeconds(0.3f);
    private WaitForEndOfFrame _frameWait = new WaitForEndOfFrame();
    
    private bool _isChecked = false;
    private int _bulletDamage = 1;
    private bool _IsShootByPlayer = false;

    private void OnEnable()
    {
        _isChecked = false;
        StartCoroutine(_Lifetime());
    }

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        if (!(_bulletType == BulletType.Laser))
            _lineRenderer.enabled = false; 
        _ray = new Ray(transform.position, transform.forward);
    }

    private void Update()
    {
        if(_bulletType == BulletType.Projectile)
            transform.localPosition += transform.forward * Time.deltaTime * _bulletSpeed;
    }

    private void MetalMovement()
    {
        _ray.origin = transform.position;
        _ray.direction = transform.forward;

        if (Physics.Raycast(_ray, out _hitInfo, _bulletSpeed * 50f, _targetLayer))
        {
            PawnBaseController pbc = _hitInfo.collider.gameObject.GetComponentInParent<PawnBaseController>();
            pbc.ApplyDamage(this);

            GlobalObjectManager.ReturnToObjectPool(gameObject);
        }
    }

    private void LaserMovement()
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

    private void FixedUpdate()
    {
        switch (_bulletType)
        {
            case BulletType.Projectile:
                MetalMovement();
                break;
            case BulletType.Laser:
                LaserMovement();
                break;
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
        }

        GlobalObjectManager.ReturnToObjectPool(gameObject);
        yield return null;
    }
}
