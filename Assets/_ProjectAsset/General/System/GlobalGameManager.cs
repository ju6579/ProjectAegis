using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalGameManager : Singleton<GlobalGameManager>
{
    public LayerMask PlayerBulletTargetLayer = 0;
    public LayerMask EnemyBulletTargetLayer = 0;
    public LayerMask PlayerShipLayer = 0;
    public LayerMask EnemyShipLayer = 0;

    [SerializeField]
    private List<Material> _nearGroundSkyList = new List<Material>();
    [SerializeField]
    private List<Material> _middleAtmosphereSkyList = new List<Material>();
    [SerializeField]
    private List<Material> _spaceSkyList = new List<Material>();

    public void ChangeSkyByProgress(float progress)
    {
        Material targetMaterial = null;

        if (progress < 0.333f)
        {
            targetMaterial = _nearGroundSkyList[Random.Range(0, _nearGroundSkyList.Count - 1)];
        }
        else if (progress >= 0.333f && progress < 0.666f)
        {
            targetMaterial = _middleAtmosphereSkyList[Random.Range(0, _middleAtmosphereSkyList.Count - 1)];
        }
        else
        {
            targetMaterial = _spaceSkyList[Random.Range(0, _spaceSkyList.Count - 1)];
        }

        RenderSettings.skybox = targetMaterial;
    }

    public void EndMainGameScene()
    {
        StartCoroutine(_EndMainGameScene());
    }

    private IEnumerator _EndMainGameScene()
    {
        PlayerCameraController.GetInstance().EndScene(5f);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("LobbyScene");
    }
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