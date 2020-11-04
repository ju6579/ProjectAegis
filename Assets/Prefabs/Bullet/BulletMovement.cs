using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
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
    private float _bulletDamage = 1f;

    private void Start()
    {
        StartCoroutine(_LifeTime());
    }

    private void Update()
    {
        transform.localPosition += transform.forward * Time.deltaTime * _bulletSpeed;
    }

    private IEnumerator _LifeTime()
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(this.gameObject);
    }
}
