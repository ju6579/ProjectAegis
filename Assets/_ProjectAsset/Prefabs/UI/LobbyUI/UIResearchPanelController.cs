using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PlayerKindom.PlayerKindomTypes;
using Pawn;

public class UIResearchPanelController : MonoBehaviour
{
    public void OnClickRearchPanelButton()
    {
        if (_researchUIPanel.activeInHierarchy)
            TurnOffResearchPanel();
        else
            TurnOnResearchPanel();
    }

    public void OnClickShipListButton()
    {
        _weaponResearchListContents.ForEach((GameObject go) => go.SetActive(false));
        _shipResearchListContents.ForEach((GameObject go) => go.SetActive(true));
    }

    public void OnClickWeaponListButton()
    {
        _shipResearchListContents.ForEach((GameObject go) => go.SetActive(false));
        _weaponResearchListContents.ForEach((GameObject go) => go.SetActive(true));
    }

    [SerializeField]
    private GameObject _researchUIPanel = null;

    [SerializeField]
    private GameObject _researchUIContents = null;

    [SerializeField]
    private Button _shipListButton = null;

    [SerializeField]
    private Button _weaponListButton = null;

    [SerializeField]
    private ScrollRect _contentsScrollView = null;

    #region Information Panel View
    [SerializeField]
    private Text _currentTP = null;

    [SerializeField]
    private Image _productImage = null;

    [SerializeField]
    private Text _productName = null;

    [SerializeField]
    private Text _productInformation = null;

    [SerializeField]
    private Button _unlockButton = null;

    [SerializeField]
    private Text _unlockTPValueText = null;
    #endregion

    private static readonly string _unlockedTextComment = "DONE";

    private List<GameObject> _shipResearchListContents = new List<GameObject>();
    private List<GameObject> _weaponResearchListContents = new List<GameObject>();

    private void Awake()
    {
        TurnOffResearchPanel();

        Singleton<ResearchManager>.ListenSingletonLoaded(() =>
        {
            GameObject cache = null;
            Button buttonCache = null;

            ResearchManager.GetInstance().AllShipList.ForEach((ProductionTask pTask) =>
            {
                cache = Instantiate(_researchUIContents, _contentsScrollView.content);
                cache.transform.localRotation = Quaternion.identity;
                cache.transform.localPosition = new Vector3(0, 0, -0.01f);

                cache.GetComponent<UIResearchContentProperty>().SetResearchContentsProperties(pTask);

                buttonCache = cache.GetComponent<Button>();
                buttonCache.onClick.AddListener(() => OnClickUIContentsButton(pTask));

                cache.SetActive(false);
                _shipResearchListContents.Add(cache);
            });

            ResearchManager.GetInstance().AllWeaponList.ForEach((ProductionTask pTask) =>
            {
                cache = Instantiate(_researchUIContents, _contentsScrollView.content);
                cache.transform.localRotation = Quaternion.identity;
                cache.transform.localPosition = new Vector3(0, 0, -0.01f);

                cache.GetComponent<UIResearchContentProperty>().SetResearchContentsProperties(pTask);

                buttonCache = cache.GetComponent<Button>();
                buttonCache.onClick.AddListener(() => OnClickUIContentsButton(pTask));

                cache.SetActive(false);
                _weaponResearchListContents.Add(cache);
            });
        });
    }

    private void SetOffInformationPanel()
    {
        _productImage.gameObject.SetActive(false);
        _productName.gameObject.SetActive(false);
        _productInformation.gameObject.SetActive(false);
        _unlockButton.gameObject.SetActive(false);
    }

    private void SetOnInformationPanel(ProductionTask pTask)
    {
        _productImage.sprite = pTask.TaskIcon;
        _productName.text = pTask.TaskName;
        _productInformation.text = pTask.TaskInformation;

        bool isUnlocked = ResearchManager.GetInstance().IsProductUnlocked(pTask);
        _unlockTPValueText.text = isUnlocked ? _unlockedTextComment : pTask.ProductionTPPoint.ToString();

        _productImage.gameObject.SetActive(true);
        _productName.gameObject.SetActive(true);
        _productInformation.gameObject.SetActive(true);
        _unlockButton.gameObject.SetActive(true);
    }

    private void OnClickUIContentsButton(ProductionTask pTask)
    {
        SetOnInformationPanel(pTask);
    }

    private void TurnOnResearchPanel()
    {
        OnClickShipListButton();
        SetOffInformationPanel();
        _researchUIPanel.SetActive(true);
    }

    private void TurnOffResearchPanel()
    {
        _researchUIPanel.SetActive(false);
    }

    private void Update()
    {
        _currentTP.text = ResearchManager.GetInstance().TrainingPoint.ToString();
    }
}