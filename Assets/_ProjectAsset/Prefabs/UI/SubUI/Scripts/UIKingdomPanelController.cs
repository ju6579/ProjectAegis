using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PlayerKindom;
using PlayerKindom.PlayerKindomTypes;

public class UIKingdomPanelController : MonoBehaviour
{
    [SerializeField]
    private Text _remainTimeText = null;

    [SerializeField]
    private UIMapPanelController _mapPanel = null;

    [SerializeField]
    private Text _currentTP = null;

    [SerializeField]
    private Text _currentKilledEnemy = null;

    private WaitForSeconds _uiUpdateRate = new WaitForSeconds(0.3f);

    private void Start()
    {
        StartCoroutine(_UpdateUI());
    }

    private IEnumerator _UpdateUI()
    {
        while (this != null)
        {
            UpdateKingdomInformation();
            yield return _uiUpdateRate;
        }
            
        yield return null;
    }

    private void UpdateKingdomInformation()
    {
        if (_mapPanel != null)
            _remainTimeText.text = ((int)_mapPanel.RemainTime).ToString();

        if(ResearchManager.GetInstance() != null)
            _currentTP.text = ResearchManager.GetInstance().TrainingPoint.ToString();
        
        _currentKilledEnemy.text = PlayerKingdom.GetInstance().KillNumber.ToString();
    }
}