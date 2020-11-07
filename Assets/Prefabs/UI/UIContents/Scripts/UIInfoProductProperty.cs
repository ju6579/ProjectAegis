using UnityEngine;
using UnityEngine.UI;

using PlayerKindom;
using PlayerKindom.PlayerKindomTypes;

public class UIInfoProductProperty : MonoBehaviour
{
    [SerializeField]
    private Image _targetImage = null;

    [SerializeField]
    private Text _targetName = null;

    [SerializeField]
    private Text _targetType = null;

    [SerializeField]
    private Text _targetCount = null;

    [SerializeField]
    private Text _targetInformation = null;

    private ProductionTask _selectedTask = null;
    private PawnBaseController.PawnType _selectedProductType = PawnBaseController.PawnType.NotSet;

    public void ReplaceProductInfo(ProductionTask pTask)
    {
        _selectedTask = pTask;

        EnableTargetDataExceptCountText();

        _selectedProductType = pTask.ProductType;

        _targetImage.sprite = pTask.TaskIcon;
        _targetName.text = pTask.TaskName;
        _targetType.text = _selectedProductType.ToString();
        _targetInformation.text = pTask.TaskInformation;

        if (_selectedProductType == PawnBaseController.PawnType.Weapon)
        {
            _targetCount.gameObject.SetActive(true);
            _targetCount.text = PlayerKingdom.GetInstance().WeaponCount(pTask).ToString();
        }
        else
            _targetCount.gameObject.SetActive(false);
    }

    private void Awake()
    {
        PlayerUIController.ActiveUIPanelEventCallbacks += DisableTargetData;
        PlayerUIController.DisableUIPanelEventCallbacks += DisableTargetData;
    }

    private void Update()
    {
        if (_selectedProductType == PawnBaseController.PawnType.Weapon)
            _targetCount.text = PlayerKingdom.GetInstance().WeaponCount(_selectedTask).ToString();
    }

    private void EnableTargetDataExceptCountText()
    {
        _targetImage.gameObject.SetActive(true);
        _targetName.gameObject.SetActive(true);
        _targetType.gameObject.SetActive(true);
        _targetInformation.gameObject.SetActive(true);
    }

    private void DisableTargetData()
    {
        _targetImage.gameObject.SetActive(false);
        _targetName.gameObject.SetActive(false);
        _targetType.gameObject.SetActive(false);
        _targetCount.gameObject.SetActive(false);
        _targetInformation.gameObject.SetActive(false);
    }
}
