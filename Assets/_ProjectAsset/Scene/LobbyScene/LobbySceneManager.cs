using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbySceneManager : MonoBehaviour
{
    [SerializeField]
    private string _playSceneName = null;

    [SerializeField]
    private string _tutorialSceneName = null;

    [SerializeField]
    private AudioSource _startSound = null;

    public void OnClickGameStartButton() => StartCoroutine(_StartMainScene());

    public void OnClickTutorialButton() => StartCoroutine(_StartTutorialScene());

    private IEnumerator _StartMainScene()
    {
        _startSound.Play();
        yield return PlayerCameraController.GetInstance().FadeOut();

        SceneManager.LoadScene(_playSceneName);
        yield return null;
    }

    private IEnumerator _StartTutorialScene()
    {
        _startSound.Play();
        yield return PlayerCameraController.GetInstance().FadeOut();

        SceneManager.LoadScene(_tutorialSceneName);
        yield return null;
    }
}
