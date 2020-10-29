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
    private float bulletSpeed = 1f;

    [SerializeField]
    private float bulletDamage = 1f;


}
