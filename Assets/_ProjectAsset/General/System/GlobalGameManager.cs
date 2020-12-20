using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mewlist;

public class GlobalGameManager : Singleton<GlobalGameManager>
{
    public LayerMask PlayerBulletTargetLayer = 0;
    public LayerMask EnemyBulletTargetLayer = 0;
    public LayerMask PlayerShipLayer = 0;
    public LayerMask EnemyShipLayer = 0;

    [SerializeField]
    private List<SkyBoxSettings> _nearGroundSkyList = new List<SkyBoxSettings>();
    [SerializeField]
    private List<SkyBoxSettings> _middleAtmosphereSkyList = new List<SkyBoxSettings>();
    [SerializeField]
    private List<SkyBoxSettings> _spaceSkyList = new List<SkyBoxSettings>();

    [SerializeField]
    private AudioSource _backgroundAudio = null;

    private List<MassiveCloudsProfile> _cloudProfilesCache = new List<MassiveCloudsProfile>();

    public void SwitchMusic(float progress)
    {
        SkyBoxSettings sky;

        if (progress < 0.333f)
        {
            sky = _nearGroundSkyList[Random.Range(0, _nearGroundSkyList.Count - 1)];
        }
        else if (progress >= 0.333f && progress < 0.666f)
        {
            sky = _middleAtmosphereSkyList[Random.Range(0, _middleAtmosphereSkyList.Count - 1)];
        }
        else
        {
            sky = _spaceSkyList[Random.Range(0, _spaceSkyList.Count - 1)];
        }

        StartCoroutine(_SwitchMusic(sky.BGM));
    }

    public void ChangeSkyByProgress(float progress)
    {
        
        SkyBoxSettings sky;
        _cloudProfilesCache.Clear();

        if (progress < 0.333f)
        {
            sky = _nearGroundSkyList[Random.Range(0, _nearGroundSkyList.Count - 1)];
        }
        else if (progress >= 0.333f && progress < 0.666f)
        {
            sky = _middleAtmosphereSkyList[Random.Range(0, _middleAtmosphereSkyList.Count - 1)];
        }
        else
        {
            sky = _spaceSkyList[Random.Range(0, _spaceSkyList.Count - 1)];
        }

        RenderSettings.skybox = sky.SkyBox;

        MassiveClouds cloudsEffect = null;
        if (Camera.main != null)
            cloudsEffect = Camera.main.GetComponent<MassiveClouds>();

        if (cloudsEffect != null)
        {
            if (sky.IsDrawCloud)
            {
                cloudsEffect.enabled = true;
                _cloudProfilesCache.Add(sky.Layer);
                cloudsEffect.SetProfiles(_cloudProfilesCache);
            }
                
            else
            {
                cloudsEffect.enabled = false;
            }
        }
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

    private IEnumerator _SwitchMusic(AudioClip clip)
    {
        WaitForSeconds switchRate = new WaitForSeconds(0.05f);
        float volumeRate = 0.3f / 20f;

        for (int i = 0; i < 20; i++) 
        {
            _backgroundAudio.volume = Mathf.Clamp(_backgroundAudio.volume - volumeRate, 0f, 0.3f);
            yield return switchRate;
        }
        _backgroundAudio.Stop();
        
        _backgroundAudio.clip = clip;
        _backgroundAudio.loop = true;
        _backgroundAudio.Play();

        for (int i = 0; i < 20; i++)
        {
            _backgroundAudio.volume = Mathf.Clamp(_backgroundAudio.volume + volumeRate, 0f, 0.3f);
            yield return switchRate;
        }

        yield return null;
    }

    [System.Serializable]
    public struct SkyBoxSettings
    {
        public bool IsDrawCloud;
        public Material SkyBox;
        public MassiveCloudsProfile Layer;
        public AudioClip BGM;
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