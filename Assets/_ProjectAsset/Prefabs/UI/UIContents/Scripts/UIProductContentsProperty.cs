using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using PlayerKindom;
using PlayerKindom.PlayerKindomTypes;
using Pawn;

public class UIProductContentsProperty : MonoBehaviour
{
    [SerializeField]
    private Image _productImage = null;

    [SerializeField]
    private Text _productName = null;

    [SerializeField]
    private Text _productCounter = null;

    [SerializeField]
    private Text _productCounterText = null;

    [SerializeField]
    private Text _productSpendTime = null;

    public ProductionTask TaskData = null;
    
    private Button _contentsButton = null;

    private void Awake()
    {
        _contentsButton = GetComponent<Button>();
    }

    public void SetUIContentsData(ProductionTask pTask)
    {
        if (_productImage == null &&
            _productSpendTime == null &&
            _productCounter == null)
            GlobalLogger.CallLogError(pTask.TaskName, GErrorType.ComponentNull);

        TaskData = pTask;

        _productImage.sprite = TaskData.TaskIcon;
        _productName.text = TaskData.TaskName;
        _productSpendTime.text = TaskData.TaskExecuteTime.ToString();

        if (PawnBaseController.CompareType(pTask.Product, PawnType.SpaceShip))
        {
            _productCounter.gameObject.SetActive(false);
            _productCounterText.gameObject.SetActive(false);
        }
        else
            Singleton<PlayerUIController>.ListenSingletonLoaded(() => {
                PlayerUIController.GetInstance().StartCoroutine(_ObserveWeaponCount());
            });
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
