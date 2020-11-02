using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : Singleton<PlayerUIController>
{
    [SerializeField]
    private GameObject buttonSample;

    [SerializeField]
    private ScrollRect playerKingdomControlPanel = null;
    private bool _isUIActive;

    private void Start()
    {
        List<PlayerKingdom.ProductionTask> productionTasks = PlayerKingdom.GetInstance().ProductionTasks;
        productionTasks.ForEach((PlayerKingdom.ProductionTask pTask) =>
        {
            GameObject buttonCache = Instantiate(buttonSample, playerKingdomControlPanel.content);

            buttonCache.transform.Find("Name").GetComponent<Text>().text = pTask.TaskName;
            buttonCache.transform.Find("Task Time").GetComponent<Text>().text = pTask.TaskExecuteTime.ToString();

            buttonCache.GetComponent<Button>().onClick.AddListener(() => PlayerKingdom.GetInstance().RequestTaskToKingdom(pTask));
        });

        _isUIActive = false;
        DisableUIPanel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isUIActive = !_isUIActive;

            if (_isUIActive)
                ActiveUIPanel();
            else
                DisableUIPanel();
        }
    }

    private void ActiveUIPanel()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        playerKingdomControlPanel.gameObject.SetActive(true);
    }

    private void DisableUIPanel()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        playerKingdomControlPanel.gameObject.SetActive(false);
    }
}
