using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGameManager : Singleton<GlobalGameManager>
{
    public LayerMask PlayerBulletTargetLayer = 0;
    public LayerMask EnemyBulletTargetLayer = 0;
    public LayerMask PlayerShipLayer = 0;
    public LayerMask EnemyShipLayer = 0;
}

public class WarpPointManager
{
    private Queue<Vector3> _shipWarpPointQueue = new Queue<Vector3>();
    private Queue<Vector3> _usedShipWarpPoint = new Queue<Vector3>();

    private Transform _baseTransform = null;
    private float _warpBoundary = 0.2f;

    // Generate Random Point By Cut Count
    public WarpPointManager(Transform baseTransform, int cutCount, float warpPointBoundary)
    {
        _baseTransform = baseTransform;

        List<Vector3> randomPositionSeed = new List<Vector3>();
        float degreeOffset = Mathf.PI * 2f / cutCount;

        for (int i = 0; i < cutCount; i++)
        {
            float degree = i * degreeOffset;
            Vector3 direction = new Vector3(Mathf.Cos(degree), Mathf.Sin(degree), 0);
            randomPositionSeed.Add(direction);
        }

        while (randomPositionSeed.Count != 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, randomPositionSeed.Count - 1);
            _shipWarpPointQueue.Enqueue(randomPositionSeed[randomIndex]);
            randomPositionSeed.RemoveAt(randomIndex);
        }

        _warpBoundary = warpPointBoundary;
    }

    public Vector3 GetNextShipWarpPoint()
    {
        if (_shipWarpPointQueue.Count <= 0)
            for (int i = 0; i < _usedShipWarpPoint.Count; i++)
                _shipWarpPointQueue.Enqueue(_usedShipWarpPoint.Dequeue());

        Vector3 pointCache = _shipWarpPointQueue.Dequeue();
        _usedShipWarpPoint.Enqueue(pointCache);

        pointCache = (_baseTransform != null ? _baseTransform.localPosition : Vector3.zero) 
                  + pointCache * _warpBoundary;

        return pointCache;
    }
}