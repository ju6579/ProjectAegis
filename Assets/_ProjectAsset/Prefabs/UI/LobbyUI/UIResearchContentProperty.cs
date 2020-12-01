using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayerKindom.PlayerKindomTypes;
public class UIResearchContentProperty : MonoBehaviour
{
    [SerializeField]
    private Image _productImage = null;

    [SerializeField]
    private Text _productName = null;

    public void SetResearchContentsProperties(ProductionTask pTask)
    {
        _productImage.sprite = pTask.TaskIcon;
        _productName.text = pTask.TaskName;
    }
}