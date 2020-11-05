using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapSystem : Singleton<MapSystem>
{
    [SerializeField]
    private int MaxMapWidth = 30;

    [SerializeField]
    private int MaxMapHeight = 10;

    [SerializeField]
    private ScrollRect mapScrollView = null;

    [SerializeField]
    private GameObject mapButtonSample = null;

    [SerializeField]
    private RectTransform anchor = null;

    // [Width] [Height]
    private SingleTile[][] _hexagonalMap;

    protected override void Awake()
    {
        base.Awake();
        GenerateTileMap();
    }
    
    private void GenerateTileMap()
    {
        _hexagonalMap = new SingleTile[MaxMapWidth][];
        for (int i = 0; i < MaxMapWidth; i++)
        {
            _hexagonalMap[i] = new SingleTile[MaxMapHeight];

            for (int j = 0; j < MaxMapHeight; j++)
                _hexagonalMap[i][j] = new SingleTile(new SingleTile.TileProperties(i, j));
        }

        for (int i = 0; i < MaxMapWidth; i++)
        {
            for (int j = 0; j < MaxMapHeight; j++)
            {
                // Next Tile Left
                if (j - 1 >= 0)
                {
                    _hexagonalMap[i][j].SetNextTileByDirection(SingleTile.TileDirection.Up, _hexagonalMap[i][j - 1]);
                }

                // Next Tile Left Up
                if ((i - 1 >= 0) && (j - i % 2 >= 0)) 
                    _hexagonalMap[i][j].SetNextTileByDirection(SingleTile.TileDirection.UpLeft, _hexagonalMap[i - 1][j - i % 2]);

                // Next Tile Left Down
                if ((i - 1 >= 0) && (j + 1 - i % 2 < MaxMapHeight)) 
                    _hexagonalMap[i][j].SetNextTileByDirection(SingleTile.TileDirection.DownLeft, _hexagonalMap[i - 1][j + 1 - i % 2]);

                // Next Tile Right
                if (j + 1 < MaxMapHeight) 
                    _hexagonalMap[i][j].SetNextTileByDirection(SingleTile.TileDirection.Down, _hexagonalMap[i][j+1]);

                // Next Tile Right Up
                if ((i + 1 < MaxMapWidth) && (j - i % 2 >= 0)) 
                    _hexagonalMap[i][j].SetNextTileByDirection(SingleTile.TileDirection.UpRight, _hexagonalMap[i + 1][j - i % 2]);

                // Next Tile Right Down
                if ((i + 1 < MaxMapWidth) && (j + 1 - i % 2 < MaxMapHeight)) 
                    _hexagonalMap[i][j].SetNextTileByDirection(SingleTile.TileDirection.DownRight, _hexagonalMap[i + 1][j + 1 - i % 2]);

                // For Debug
                Button buttonCache = Instantiate(mapButtonSample, mapScrollView.content).GetComponent<Button>();
                RectTransform buttonTransform = buttonCache.GetComponent<RectTransform>();
                Vector3 targetPosition = anchor.localPosition;
                targetPosition.x += _hexagonalMap[i][j].Properties.Position.x * buttonTransform.sizeDelta.x + 1f * 9 / 16;
                targetPosition.y -= _hexagonalMap[i][j].Properties.Position.y * buttonTransform.sizeDelta.y - (i % 2 + 1) * buttonTransform.sizeDelta.y / 2f - 1f;
                targetPosition.z -= 1f;
                buttonTransform.localPosition = targetPosition;
                _hexagonalMap[i][j].SetDebugButton(buttonCache);
            }
        }
    }

    private class SingleTile
    {
        public PlayerKingdom.SpendableResource TileResource;

        public struct TileProperties
        {
            public Vector3 Position => _tilePosition;
            private Vector3 _tilePosition;

            public TileProperties(int x, int y)
            {
                _tilePosition = Vector3.zero;
                _tilePosition.x = x;
                _tilePosition.y = y;
            }
        }

        public enum TileDirection{
            UpLeft = 1,
            Up = 2,
            UpRight = 3,
            DownRight = -1,
            Down = -2,
            DownLeft = -3
        }

        //
        public void SetDebugButton(Button button)
        {
            DebugButton = button;
            DebugButton.GetComponentInChildren<Text>().text = Properties.Position.x.ToString() + Properties.Position.y.ToString();
            DebugButton.onClick.AddListener(() =>
            {
                this.NextTileList.ForEach((SingleTile st) => st.DebugButton.GetComponentInChildren<Text>().text = "Click");
            });
        }
        private Button DebugButton;
        //

        public TileProperties Properties => _tileProperties;
        public List<SingleTile> NextTileList => _nextTileHash.Values.ToList<SingleTile>();

        private Dictionary<TileDirection, SingleTile> _nextTileHash = new Dictionary<TileDirection, SingleTile>();
        private TileProperties _tileProperties;

        public SingleTile(TileProperties tileProperties) 
        {
            _tileProperties = tileProperties;
        }

        public void DestroySingleTile()
        {
            foreach(KeyValuePair<TileDirection, SingleTile> st in _nextTileHash)
            {
                st.Value.DeleteLinkedTile((TileDirection)(-(int)st.Key));
            }
        }

        public void DeleteLinkedTile(TileDirection dir) { _nextTileHash.Remove(dir); }
        public SingleTile GetNextTileByDirection(TileDirection dir) { return _nextTileHash[dir]; }
        public List<TileDirection> GetAvailableDirection() { return _nextTileHash.Keys.ToList<TileDirection>(); }

        public void SetNextTileByDirection(TileDirection dir, SingleTile tile) { _nextTileHash[dir] = tile; }
    }
}
