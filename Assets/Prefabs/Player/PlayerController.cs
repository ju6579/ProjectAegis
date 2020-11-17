using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.Extras;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField]
    private GameObject _playerHandLeft = null;

    [SerializeField]
    private GameObject _playerHandRight = null;

    [SerializeField]
    private SteamVR_LaserPointer _laserPointer = null;

    private SteamVR_Action_Boolean _interactUI = null;

    private SteamVR_Behaviour_Pose _trackedObjectLeft = null;
    private SteamVR_Behaviour_Pose _trackedObjectRight = null;

    protected override void Awake()
    {
        _interactUI = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("InteractUI");

        _trackedObjectLeft = _playerHandLeft.GetComponent<SteamVR_Behaviour_Pose>();
        _trackedObjectRight = _playerHandRight.GetComponent<SteamVR_Behaviour_Pose>();

        base.Awake();
    }

    private void OnEnable()
    {
        _laserPointer.PointerIn -= HandlePointerIn;
        _laserPointer.PointerIn += HandlePointerIn;

        _laserPointer.PointerOut -= HandlePointerOut;
        _laserPointer.PointerOut += HandlePointerOut;

        _laserPointer.PointerClick -= HandleTriggerClicked;
        _laserPointer.PointerClick += HandleTriggerClicked;
    }

    private void Update()
    {
        
    }

    private void HandleTriggerClicked(object sender, PointerEventArgs e)
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }
    }

    private void HandlePointerIn(object sender, PointerEventArgs e)
    {
        var button = e.target.GetComponent<Button>();
        if (button != null)
        {
            button.Select();
            Debug.Log("HandlePointerIn", e.target.gameObject);
        }
    }

    private void HandlePointerOut(object sender, PointerEventArgs e)
    {

        var button = e.target.GetComponent<Button>();
        if (button != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            Debug.Log("HandlePointerOut", e.target.gameObject);
        }
    }
}
