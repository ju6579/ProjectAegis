using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlayerCameraController : Singleton<PlayerCameraController>
{
    public WaitForSeconds FadeIn()
    {
        StopAllCoroutines();

        StartCoroutine(_FadeIn());
        return _fadeInOutTotalWait;
    }

    public WaitUntil FadeOut()
    {
        StopAllCoroutines();

        _isFadeOutFinished = false;
        StartCoroutine(_FadeOut());

        return new WaitUntil(() => _isFadeOutFinished);
    }

    [SerializeField]
    private Image _fadeInOutImage = null;

    [SerializeField]
    private float _fadeInOutTime = 3f;

    [SerializeField]
    private bool _isBlackoutStart = true;

    [SerializeField]
    private Text _initializeText = null;

    private WaitForEndOfFrame _frameWait = new WaitForEndOfFrame();
    private WaitForSeconds _fadeInOutTotalWait = null;
    private int _cameraEffectLoopTime = 0;
    private bool _isFadeOutFinished = false;

    protected override void Awake()
    {
        _initializeText.gameObject.SetActive(false);

        _fadeInOutTotalWait = new WaitForSeconds(_fadeInOutTime);
        base.Awake();
    }

    private void Start()
    {
        if (_isBlackoutStart) FadeIn();
    }

    private IEnumerator _FadeIn()
    {
        _initializeText.gameObject.SetActive(true);

        Color colorCache = Color.black;
        colorCache.a = 1;

        _fadeInOutImage.color = colorCache;
        yield return _fadeInOutTotalWait;

        while (colorCache.a > 0)
        {
            colorCache.a -= Time.deltaTime / _fadeInOutTime;
            _fadeInOutImage.color = colorCache;

            yield return _frameWait;
        }

        _initializeText.gameObject.SetActive(false);
        yield return null;
    }

    private IEnumerator _FadeOut()
    {
        _initializeText.gameObject.SetActive(true);

        Color colorCache = Color.black;
        colorCache.a = 0f;

        _fadeInOutImage.color = colorCache;

        while(colorCache.a <= 1)
        {
            colorCache.a += Time.deltaTime / _fadeInOutTime;
            _fadeInOutImage.color = colorCache;

            yield return _frameWait;
        }

        _isFadeOutFinished = true;
        yield return null;
    }
}