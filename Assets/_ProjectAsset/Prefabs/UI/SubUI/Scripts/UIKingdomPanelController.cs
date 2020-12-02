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

    private void Start()
    {
        
    }

    private void Update()
    {
        UpdateKingdomInformation();
    }

    private void UpdateKingdomInformation()
    {
        if (_mapPanel != null)
            _remainTimeText.text = ((int)_mapPanel.RemainTime).ToString();

        _currentTP.text = ResearchManager.GetInstance().TrainingPoint.ToString();
        _currentKilledEnemy.text = PlayerKingdom.GetInstance().KillNumber.ToString();
    }
}