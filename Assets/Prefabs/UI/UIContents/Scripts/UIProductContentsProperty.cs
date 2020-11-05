﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIProductContentsProperty : MonoBehaviour
{
    [SerializeField]
    private Image _productImage = null;

    [SerializeField]
    private Text _productName = null;

    [SerializeField]
    private Text _crystalCost = null;

    [SerializeField]
    private Text _explosiveCost = null;

    [SerializeField]
    private Text _metalCost = null;

    [SerializeField]
    private Text _electronicCost = null;

    [SerializeField]
    private Text _productCounter = null;

    private PlayerKingdom.ProductionTask _taskData = null;

    public void SetUIContentsData(PlayerKingdom.ProductionTask pTask)
    {
        if (_productImage == null &&
            _productName == null &&
            _crystalCost == null &&
            _explosiveCost == null &&
            _metalCost == null &&
            _electronicCost == null &&
            _productCounter == null)
            GlobalLogger.CallLogError(pTask.TaskName, GErrorType.ComponentNull);

        _taskData = pTask;

        _productImage.sprite = _taskData.TaskIcon;
        _productName.text = _taskData.TaskName;
        _crystalCost.text = _taskData.TaskCost.Crystal.ToString();
        _explosiveCost.text = _taskData.TaskCost.Explosive.ToString();
        _metalCost.text = _taskData.TaskCost.Metal.ToString();
        _electronicCost.text = _taskData.TaskCost.Electronic.ToString();

        if (pTask.Product.GetComponent<PawnBaseController>().PawnActionType == PawnBaseController.PawnType.SpaceShip)
            _productCounter.gameObject.SetActive(false);
        else
            PlayerUIController.GetInstance().StartCoroutine(_ObserveWeaponCount());
    }

    private IEnumerator _ObserveWeaponCount()
    {
        WaitForEndOfFrame frameWait = new WaitForEndOfFrame();
        while(this != null)
        {
            _productCounter.text = PlayerKingdom.GetInstance().WeaponCount(_taskData).ToString();
            yield return frameWait;
        }
        yield return null;
    }
}