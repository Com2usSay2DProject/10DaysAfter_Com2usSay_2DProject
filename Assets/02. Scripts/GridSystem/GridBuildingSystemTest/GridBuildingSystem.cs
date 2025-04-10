using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GridBuildingSystem : Singleton<GridBuildingSystem>
{
    public GridLayout GridLayout;
    public Tilemap MainTilemap;
    public Tilemap Temptilemap;

    private static Dictionary<TileType, TileBase> _tileBases = new Dictionary<TileType, TileBase>();

    private Building _tempBuilding;
    private TowerRoot _tempTower;
    private Vector3 _prevPos;
    private BoundsInt _prevArea;

    public Action OnBuildFailed;

    #region UnityMethods


    private void Start()
    {
        string tilePath = @"Tiles\";
        _tileBases.Add(TileType.Empty, null);
        _tileBases.Add(TileType.White, Resources.Load<TileBase>(tilePath + "RandGroundPixel"));
        _tileBases.Add(TileType.Green, Resources.Load<TileBase>(tilePath + "green"));
        _tileBases.Add(TileType.Red, Resources.Load<TileBase>(tilePath + "red"));
    }

    private void Update()
    {
        if (!_tempBuilding)
        {
            return;
        }

        if (!_tempBuilding.Placed)
        {
            if (EventSystem.current.IsPointerOverGameObject(0))
            {
                return;
            }

            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 worldPos = new Vector3(touchPos.x, touchPos.y, 0);
            Vector3Int cellPos = GridLayout.WorldToCell(worldPos);

            if (_prevPos != cellPos)
            {
                Vector3 basePosition = GridLayout.CellToWorld(cellPos);
                Vector3 offset = new Vector3(0.5f, 1.8f, 0f);
                _tempBuilding.transform.position = basePosition + offset;
                _prevPos = cellPos;
                FollowBuilding();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (_tempBuilding.CanBePlaced() && ResourceManager.Instance.TryUseMultipleResources(_tempTower.CostDataDict))
            {
                _tempBuilding.Place();
                _tempBuilding.GetComponent<TowerRoot>().SetPosition();
            }
            else
            {
                OnBuildFailed?.Invoke();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearArea();
            TowerPoolManager.Instance.ReturnObject(_tempTower.gameObject, _tempTower.TowerType);
        }
    }
    #endregion

    #region Tilemap Management
    private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;

        foreach (var v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = tilemap.GetTile(pos);
            counter++;
        }

        return array;
    }

    private static void SetTilesBlock(BoundsInt area, TileType type, Tilemap tilemap)
    {
        int size = area.size.x * area.size.y * area.size.z;
        TileBase[] tileArray = new TileBase[size];
        FillTiles(tileArray, type);
        tilemap.SetTilesBlock(area, tileArray);
    }

    private static void FillTiles(TileBase[] arr, TileType type)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = _tileBases[type];
        }
    }
    #endregion

    #region Building Placement
    public void InitializeWithBuulding(TowerRoot building)
    {
        _tempTower = TowerPoolManager.Instance.GetObject(building.TowerType).GetComponent<TowerRoot>();
        _tempBuilding = _tempTower.GetComponent<Building>();
        FollowBuilding();
    }

    private void ClearArea()
    {
        TileBase[] toClear = new TileBase[_prevArea.size.x * _prevArea.size.y * _prevArea.size.z];
        FillTiles(toClear, TileType.Empty);
        Temptilemap.SetTilesBlock(_prevArea, toClear);
    }

    private void FollowBuilding()
    {
        ClearArea();
        Vector3 position = _tempBuilding.transform.position;
        Vector3 offset = new Vector3(-0.5f, -1.8f, 0f);  // 건물 오프셋을 역으로 적용
        Vector3 basePosition = position + offset;
        _tempBuilding.area.position = GridLayout.WorldToCell(basePosition);
        BoundsInt buildingArea = _tempBuilding.area;

        TileBase[] baseArray = GetTilesBlock(buildingArea, MainTilemap);

        int size = baseArray.Length;
        TileBase[] tileArray = new TileBase[size];

        for (int i = 0; i < baseArray.Length; i++)
        {
            if (baseArray[i] == _tileBases[TileType.White])
            {
                tileArray[i] = _tileBases[TileType.Green];
            }
            else
            {
                FillTiles(tileArray, TileType.Red);
                break;
            }
        }

        Temptilemap.SetTilesBlock(buildingArea, tileArray);
        _prevArea = buildingArea;
    }

    public bool CanTakeArea(BoundsInt area)
    {
        TileBase[] baseArray = GetTilesBlock(area, MainTilemap);
        foreach (var b in baseArray)
        {
            if (b != _tileBases[TileType.White])
            {
                Debug.Log("Cannot Place here");
                return false;
            }
        }

        return true;
    }

    public void TakeArea(BoundsInt area)
    {
        SetTilesBlock(area, TileType.Empty, Temptilemap);
        SetTilesBlock(area, TileType.Green, MainTilemap);
    }
    #endregion
}

public enum TileType
{
    Empty,
    White,
    Green,
    Red
}