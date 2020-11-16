using Pawn;
using System.Collections;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    public int Damage => _bulletDamage;
    public bool IsShootByPlayer = false;
    public float StoppingPower => _bulletStoppingPower;
    public void SetTargetLayer(LayerMask targetLayer) => _targetLayer = targetLayer;

    private enum BulletType
    {
        Metal,
        ShortLaser,
        LongLaser,
        Missile,
        NotSet
    }

    [SerializeField]
    private BulletType _bulletType = BulletType.NotSet;

    [SerializeField]
    private float _bulletSpeed = 1f;

    [SerializeField]
    private int _bulletDamage = 1;

    [SerializeField]
    private float _bulletStoppingPower = 0f;

    private LayerMask _targetLayer = 0;
    private RaycastHit _hitInfo;
    private Ray _ray;

    private WaitForSeconds _metalLifeTime = new WaitForSeconds(1f);
    private WaitForSeconds _laserLifeTime = new WaitForSeconds(0.3f);
    private WaitForEndOfFrame _frameWait = new WaitForEndOfFrame();

    private LineRenderer _lineRenderer = null;
    private bool _isChecked = false;

    private void OnEnable()
    {
        _isChecked = false;
        StartCoroutine(_Lifetime());
    }

    private void Start()
    {
        if (_bulletType == BulletType.LongLaser)
            _lineRenderer = GetComponent<LineRenderer>();

        _ray = new Ray(transform.position, transform.forward);
    }

    private void Update()
    {
        switch (_bulletType)
        {
            case BulletType.Metal:
                MetalMovement();
                break;
            case BulletType.LongLaser:
                LaserMovement();
                break;
        }
    }

    private void MetalMovement()
    {
        transform.localPosition += transform.forward * Time.deltaTime * _bulletSpeed;

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
            case BulletType.Metal:
                yield return _metalLifeTime;
                break;
            case BulletType.LongLaser:
                yield return _laserLifeTime;
                break;
        }

        GlobalObjectManager.ReturnToObjectPool(gameObject);
        yield return null;
    }
}
