using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using PlayerKindom.PlayerKindomTypes;
using MapTileProperties;

public class MapSystem : Singleton<MapSystem>
{
    #region Interface
    public SingleTile CurrentMapTile => _currentTile;
    public SingleTile[][] WholeMap => _hexagonalMap;
    public int MaxWidth => _maxMapWidth;
    public int MaxHeight => _maxMapHeight;
    public float MapDifficulty => _currentTile.Properties.Difficulty;

    public void HarvestResourceAtCurrentTile(SpendableResource container)
    {
        container.AddResourceByData(_currentTile.Properties.TileResource);
        _currentTile.Properties.Difficulty += DifficultyFormular(_currentTile.Properties.Difficulty) * Time.deltaTime;
    }

    public void SetCurrentTile(SingleTile clickedTile)
    {
        GlobalGameManager.GetInstance().ChangeSkyByProgress(GetTileProgressByPosition(clickedTile));
        _currentTile = clickedTile;
    }
    #endregion

    [SerializeField]
    private int _maxMapWidth = 30;

    [SerializeField]
    private int _maxMapHeight = 10;

    [SerializeField]
    private float _difficultyOffset = 0f;
    // [Width] [Height]
    private SingleTile[][] _hexagonalMap;
    private SingleTile _currentTile = null;

    protected override void Awake()
    {
        GenerateTileMap();
        _currentTile = _hexagonalMap[0][0];
        base.Awake();
    }

    private void Start()
    {
        SetCurrentTile(_currentTile);
    }

    private void GenerateTileMap()
    {
        _hexagonalMap = new SingleTile[_maxMapWidth][];
        for (int i = 0; i < _maxMapWidth; i++)
        {
            _hexagonalMap[i] = new SingleTile[_maxMapHeight];

            for (int j = 0; j < _maxMapHeight; j++)
                _hexagonalMap[i][j] = new SingleTile(GenerateTilePropertyBySeed(i, j));
        }

        for (int i = 0; i < _maxMapWidth; i++)
        {
            for (int j = 0; j < _maxMapHeight; j++)
            {
                // Next Tile Left
                if (j - 1 >= 0)
                    _hexagonalMap[i][j].SetNextTileByDirection(SingleTile.TileDirection.Up, _hexagonalMap[i][j - 1]);

                // Next Tile Left Up
                if ((i - 1 >= 0) && (j - i % 2 >= 0))
                    _hexagonalMap[i][j].SetNextTileByDirection(SingleTile.TileDirection.UpLeft, _hexagonalMap[i - 1][j - i % 2]);

                // Next Tile Left Down
                if ((i - 1 >= 0) && (j + 1 - i % 2 < _maxMapHeight))
                    _hexagonalMap[i][j].SetNextTileByDirection(SingleTile.TileDirection.DownLeft, _hexagonalMap[i - 1][j + 1 - i % 2]);

                // Next Tile Right
                if (j + 1 < _maxMapHeight)
                    _hexagonalMap[i][j].SetNextTileByDirection(SingleTile.TileDirection.Down, _hexagonalMap[i][j + 1]);

                // Next Tile Right Up
                if ((i + 1 < _maxMapWidth) && (j - i % 2 >= 0))
                    _hexagonalMap[i][j].SetNextTileByDirection(SingleTile.TileDirection.UpRight, _hexagonalMap[i + 1][j - i % 2]);

                // Next Tile Right Down
                if ((i + 1 < _maxMapWidth) && (j + 1 - i % 2 < _maxMapHeight))
                    _hexagonalMap[i][j].SetNextTileByDirection(SingleTile.TileDirection.DownRight, _hexagonalMap[i + 1][j + 1 - i % 2]);

            }
        }
    }

    private TileProperties GenerateTilePropertyBySeed(int x, int y)
    {
        // Generate Tile Property by Seed Value
        int crystalSeed = Random.Range(1, 10);
        int explosiveSeed = Random.Range(1, 10);
        int metalSeed = Random.Range(1, 10);
        int electronicSeed = Random.Range(1, 10);

        SpendableResource tileResource = new SpendableResource(crystalSeed, 
                                                          explosiveSeed, 
                                                          metalSeed, 
                                                          electronicSeed);

        return new TileProperties(x, y, tileResource);
    }

    private float DifficultyFormular(float currentDifficulty)
    {
        return currentDifficulty * _difficultyOffset + 0.1f;
    }

    private float GetTileProgressByPosition(SingleTile tile)
    {
        float xProgress = (float)tile.Properties.X / (float)MaxWidth;
        float yProgress = (float)tile.Properties.Y / (float)MaxHeight;
        Debug.Log((xProgress + yProgress));
        return (xProgress + yProgress);
    }
}

namespace MapTileProperties
{
    public class SingleTile
    {
        public enum TileDirection
        {
            UpLeft = 1,
            Up = 2,
            UpRight = 3,
            DownRight = -1,
            Down = -2,
            DownLeft = -3
        }

        public TileProperties Properties => _tileProperties;
        public List<SingleTile> NextTileList => _nextTileHash.Values.ToList<SingleTile>();

        private Dictionary<TileDirection, SingleTile> _nextTileHash = new Dictionary<TileDirection, SingleTile>();
        private TileProperties _tileProperties = null;

        public SingleTile(TileProperties tileProperties)
        {
            _tileProperties = tileProperties;
        }

        public void DestroySingleTile()
        {
            foreach (KeyValuePair<TileDirection, SingleTile> st in _nextTileHash)
            {
                st.Value.DeleteLinkedTile((TileDirection)(-(int)st.Key));
            }
        }

        public void DeleteLinkedTile(TileDirection dir) { _nextTileHash.Remove(dir); }
        public SingleTile GetNextTileByDirection(TileDirection dir) { return _nextTileHash[dir]; }
        public List<TileDirection> GetAvailableDirection() { return _nextTileHash.Keys.ToList<TileDirection>(); }

        public void SetNextTileByDirection(TileDirection dir, SingleTile tile) { _nextTileHash[dir] = tile; }
    }

    public class TileProperties
    {
        public int X;
        public int Y;

        public SpendableResource TileResource;

        public float Difficulty = 0f;

        public TileProperties(int x, int y, SpendableResource resource)
        {
            this.X = x;
            this.Y = y;

            TileResource = resource;
        }
    }
}
