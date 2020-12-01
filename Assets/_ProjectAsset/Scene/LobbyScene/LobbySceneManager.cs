using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbySceneManager : MonoBehaviour
{
    [SerializeField]
    private string _playSceneName = null;

    [SerializeField]
    private AudioSource _startSound = null;

    public void OnClickGameStartButton() => StartCoroutine(_StartMainScene());

    private IEnumerator _StartMainScene()
    {
        _startSound.Play();
        yield return PlayerCameraController.GetInstance().FadeOut();

        SceneManager.LoadScene(_playSceneName);
        yield return null;
    }
}
