using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionPanelController : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _instructionSet = new List<GameObject>();

    private int _currentInstruction = 0;

    private void Start()
    {
        _instructionSet.ForEach((GameObject go) => go.SetActive(false));
        _instructionSet[0].SetActive(true);
        _currentInstruction = 0;
    }

    public void OnClickPrevButton()
    {
        _instructionSet[_currentInstruction].SetActive(false);
        _currentInstruction = Mathf.Clamp(_currentInstruction - 1, 0, _instructionSet.Count - 1);
        _instructionSet[_currentInstruction].SetActive(true);
    }

    public void OnClickNextButton()
    {
        _instructionSet[_currentInstruction].SetActive(false);
        _currentInstruction = Mathf.Clamp(_currentInstruction + 1, 0, _instructionSet.Count - 1);
        _instructionSet[_currentInstruction].SetActive(true);
    }

    public void OnClickTutorialEnd()
    {
        if (GlobalGameManager.GetInstance() != null)
            GlobalGameManager.GetInstance().EndMainGameScene();
    }
}
