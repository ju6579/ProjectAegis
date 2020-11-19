using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MapTileProperties;
using PlayerKindom;

public class UIMapPanelController : MonoBehaviour
{
    public float RemainTime => Mathf.Clamp(_escapeTimeOffset - (Time.time - _escapeTimeStamp), 
                                      0, _escapeTimeOffset);

    [SerializeField]
    private GameObject _targetPanel;

    [SerializeField]
    private GameObject _mapButtonSample = null;

    [SerializeField]
    private RectTransform _anchor = null;

    [SerializeField]
    private float _escapeTimeOffset = 10f;

    private Dictionary<Button, SingleTile> _buttonToTile = new Dictionary<Button, SingleTile>();
    private Dictionary<SingleTile, Button> _tileToButton = new Dictionary<SingleTile, Button>();

    private float _escapeTimeStamp = 0f;
    private bool _isTileButtonEnabled = false;

    private void OnEnable()
    {
        _isTileButtonEnabled = false;
    }

    private void Start()
    {
        SingleTile[][] map = MapSystem.GetInstance().WholeMap;
        int width = MapSystem.GetInstance().MaxWidth;
        int height = MapSystem.GetInstance().MaxHeight;

        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                Button buttonCache = Instantiate(_mapButtonSample, _targetPanel.transform).GetComponent<Button>();
                
                _buttonToTile.Add(buttonCache, map[i][j]);
                _tileToButton.Add(map[i][j], buttonCache);

                RectTransform buttonTransform = buttonCache.GetComponent<RectTransform>();

                Vector3 targetPosition = _anchor.localPosition;
                targetPosition.x += ((float)map[i][j].Properties.X) * buttonTransform.sizeDelta.x + 1f * 9 / 16;
                targetPosition.y -= ((float)map[i][j].Properties.Y) * buttonTransform.sizeDelta.y - (i % 2 + 1) * buttonTransform.sizeDelta.y / 2f - 1f;
                targetPosition.z = -0.01f;

                buttonTransform.localPosition = targetPosition;

                buttonTransform.localRotation = Quaternion.identity;

                SetDebugButton(buttonCache);
            }
        }

        _escapeTimeStamp = Time.time;
    }

    private void Update()
    {
        if(Time.time - _escapeTimeStamp > _escapeTimeOffset && !_isTileButtonEnabled)
        {
            ActiveLinkedTile(MapSystem.GetInstance().CurrentMapTile);
            _isTileButtonEnabled = true;
        }
    }

    private void EscapeOnButtonPress()
    {
        PlayerKingdom.GetInstance().ExecuteEscape();

        _escapeTimeStamp = Time.time + PlayerKingdom.GetInstance().EscapeTime;
        _isTileButtonEnabled = false;
    }

    private void UpdateCurrentTileByClick(Button clickedButton)
    {
        SingleTile currentTile = MapSystem.GetInstance().CurrentMapTile;

        DisableLinkedTile(currentTile);

        MapSystem.GetInstance().SetCurrentTile(_buttonToTile[clickedButton]);
    }

    private void ActiveLinkedTile(SingleTile target)
    {
        target.NextTileList.ForEach((SingleTile tile) =>
        {
            _tileToButton[tile].enabled = true;
            _tileToButton[tile].GetComponentInChildren<Text>().text = "Click";
        });
    }

    private void DisableLinkedTile(SingleTile target)
    {
        target.NextTileList.ForEach((SingleTile tile) =>
        {
            _tileToButton[tile].enabled = false;
            _tileToButton[tile].GetComponentInChildren<Text>().text = tile.Properties.X.ToString()
                                                         + tile.Properties.Y.ToString();
        });
    }

    private void SetDebugButton(Button button)
    {
        SingleTile tile = _buttonToTile[button];

        button.enabled = false;
        button.GetComponentInChildren<Text>().text = tile.Properties.X.ToString() 
                                             + tile.Properties.Y.ToString();
        button.onClick.AddListener(() =>
        {
            EscapeOnButtonPress();
            UpdateCurrentTileByClick(button);
        });
    }
}
