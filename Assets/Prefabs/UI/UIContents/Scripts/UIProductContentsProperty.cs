using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using PlayerKindom;
using PlayerKindom.PlayerKindomTypes;

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

    public ProductionTask TaskData = null;
    
    private Button _contentsButton = null;

    private void Awake()
    {
        _contentsButton = GetComponent<Button>();
    }

    public void SetUIContentsData(ProductionTask pTask)
    {
        if (_productImage == null &&
            _productName == null &&
            _crystalCost == null &&
            _explosiveCost == null &&
            _metalCost == null &&
            _electronicCost == null &&
            _productCounter == null)
            GlobalLogger.CallLogError(pTask.TaskName, GErrorType.ComponentNull);

        TaskData = pTask;

        _productImage.sprite = TaskData.TaskIcon;
        _productName.text = TaskData.TaskName;
        _crystalCost.text = TaskData.TaskCost.Crystal.ToString();
        _explosiveCost.text = TaskData.TaskCost.Explosive.ToString();
        _metalCost.text = TaskData.TaskCost.Metal.ToString();
        _electronicCost.text = TaskData.TaskCost.Electronic.ToString();

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
            _productCounter.text = PlayerKingdom.GetInstance().WeaponCount(TaskData).ToString();
            yield return frameWait;
        }
        yield return null;
    }

    private void OnDisable()
    {
        _contentsButton.image.color = Color.white;
    }
}
