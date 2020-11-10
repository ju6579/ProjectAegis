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
        Missile
    }

    [SerializeField]
    private float _bulletSpeed = 1f;

    [SerializeField]
    private int _bulletDamage = 1;

    [SerializeField]
    private float _bulletStoppingPower = 0f;

    private LayerMask _targetLayer = 0;
    private RaycastHit _hitInfo;
    private Ray _ray;

    private void Start()
    {
        _ray = new Ray(transform.position, transform.forward);
        StartCoroutine(_Lifetime());
    }

    private void Update()
    {
        transform.localPosition += transform.forward * Time.deltaTime * _bulletSpeed;

        _ray.origin = transform.position;
        _ray.direction = transform.forward;
        if (Physics.Raycast(_ray, out _hitInfo, _bulletSpeed * 50f, _targetLayer)) 
        {
            PawnBaseController pbc = _hitInfo.collider.gameObject.GetComponentInParent<PawnBaseController>();
            pbc.ApplyDamage(this);

            PlayerKindom.PlayerKingdom.GetInstance().StartCoroutine(_KillWait());
            gameObject.SetActive(false);
        }
    }

    private IEnumerator _KillWait()
    {
        yield return new WaitForSeconds(Time.deltaTime);
        Destroy(this.gameObject);
    }

    private IEnumerator _Lifetime()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }
}
