using System.Collections;
using System.Collections.Generic;
using System.Text;
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

    public WaitForSeconds FadeInByTime(float time)
    {
        StopAllCoroutines();

        StartCoroutine(_FadeInByTime(time));
        return new WaitForSeconds(time);
    }

    public WaitForSeconds FadeOutByTime(float time)
    {
        StopAllCoroutines();

        _isFadeOutFinished = false;
        StartCoroutine(_FadeOutByTime(time));

        return new WaitForSeconds(time);
    }

    public WaitForSeconds EndScene(float time)
    {
        StopAllCoroutines();

        StartCoroutine(_EndScene());

        return new WaitForSeconds(time);
    }

    [SerializeField]
    private Image _fadeInOutImage = null;

    [SerializeField]
    private float _fadeInOutTime = 3f;

    [SerializeField]
    private bool _isBlackoutStart = true;

    [SerializeField]
    private Text _initializeText = null;

    [SerializeField]
    private Text _operationText = null;

    private WaitForEndOfFrame _frameWait = new WaitForEndOfFrame();
    private WaitForSeconds _fadeInOutTotalWait = null;
    private int _cameraEffectLoopTime = 0;
    private bool _isFadeOutFinished = false;

    protected override void Awake()
    {
        _initializeText.gameObject.SetActive(false);

        if(_operationText != null)
            _operationText.gameObject.SetActive(false);

        _fadeInOutTotalWait = new WaitForSeconds(_fadeInOutTime);
        base.Awake();
    }

    private void Start()
    {
        if (_isBlackoutStart) FadeIn();
    }

    private IEnumerator _FadeInByTime(float time)
    {
        _initializeText.gameObject.SetActive(true);

        Color colorCache = Color.black;
        colorCache.a = 1;

        _fadeInOutImage.color = colorCache;

        while (colorCache.a > 0)
        {
            colorCache.a -= Time.deltaTime / time;
            _fadeInOutImage.color = colorCache;

            yield return _frameWait;
        }

        _initializeText.gameObject.SetActive(false);
        yield return null;
    }

    private IEnumerator _FadeOutByTime(float time)
    {
        _initializeText.gameObject.SetActive(true);

        Color colorCache = Color.black;
        colorCache.a = 0f;

        _fadeInOutImage.color = colorCache;

        DrawOperationLog(3f);

        while (colorCache.a <= 1)
        {
            colorCache.a += Time.deltaTime / time;
            _fadeInOutImage.color = colorCache;

            yield return _frameWait;
        }
        
        _isFadeOutFinished = true;
        yield return null;
    }

    private IEnumerator _FadeIn()
    {
        _initializeText.gameObject.SetActive(true);

        Color colorCache = Color.black;
        colorCache.a = 1;

        _fadeInOutImage.color = colorCache;

        DrawOperationLog(3f);

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

    private IEnumerator _EndScene()
    {
        StartCoroutine(_FadeOut());

        _operationText.gameObject.SetActive(true);
        _operationText.text = "";

        int trainingPoint = ResearchManager.GetInstance().TrainingPoint;

        string breifingText = "<Training End>\n-Commit Result To System\n-Gathered Training Point: " + trainingPoint.ToString();

        Debug.Log(breifingText);

        // Print Breifing Text
        var textPivot = breifingText.GetEnumerator();
        while (textPivot.MoveNext())
        {
            _operationText.text += textPivot.Current;
            yield return _typingEffectWait;
        }

        yield return null;
    }

    private void DrawOperationLog(float time)
    {
        if (_operationText != null)
        {
            // Clear For Write new Text
            _operationText.text = "";
            _stringBuilder.Clear();
            _operationText.gameObject.SetActive(true);

            _operationText.StartCoroutine(_BreifingTypeWriterEffect(time));
        }
    }

    private WaitForSeconds _typingEffectWait = new WaitForSeconds(0.05f);
    private List<string> _mapName = new List<string>() { "Operation Area ", "Planet B", "Nebular "};
    private StringBuilder _stringBuilder = new StringBuilder();
    private int _mapCount = 0;
    private IEnumerator _BreifingTypeWriterEffect(float time)
    {
        // Making Brefing Text
        _stringBuilder.Append(_mapName[_mapCount]);
        _stringBuilder.Append(Random.Range(14, 256));
        _stringBuilder.Append("\n");

        if (_mapCount == 2)
        {
            _stringBuilder.Append("Destroy Enemy Base Station\n");
            EnemyKingdom.GetInstance().RequestCreateBoss();
        }  
        else
            _stringBuilder.Append("Withstand Enemy Assualt\n");

        _stringBuilder.Append("Operation Time : ");
        _stringBuilder.Append(System.DateTime.Now.ToString("HH:mm:ss"));

        string breifingText = _stringBuilder.ToString();

        // Print Breifing Text
        var textPivot = breifingText.GetEnumerator();
        while (textPivot.MoveNext())
        {
            _operationText.text += textPivot.Current;
            yield return _typingEffectWait;
        }

        yield return new WaitForSeconds(time);
        _operationText.gameObject.SetActive(false);
        _mapCount++;
        yield return null;
    }
}