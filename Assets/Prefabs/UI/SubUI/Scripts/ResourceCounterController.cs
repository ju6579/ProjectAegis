using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PlayerKindom;
using PlayerKindom.PlayerKindomTypes;

public class ResourceCounterController : MonoBehaviour
{
    [SerializeField]
    private Text _crystalValue = null;

    [SerializeField]
    private Text _explosiveValue = null;

    [SerializeField]
    private Text _metalValue = null;

    [SerializeField]
    private Text _electronicValue = null;

    private SpendableResource _targetResource = null;

    private void Start()
    {
        Singleton<PlayerKingdom>.ListenSingletonLoaded(() => _targetResource = PlayerKingdom.GetInstance().CurrentResource);
        
    }

    private void Update()
    {
        UpdateResouceText();
    }

    private void UpdateResouceText()
    {
        _crystalValue.text = _targetResource.Crystal.ToString();
        _explosiveValue.text = _targetResource.Explosive.ToString();
        _metalValue.text = _targetResource.Metal.ToString();
        _electronicValue.text = _targetResource.Electronic.ToString();
    }
}