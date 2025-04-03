using CityBuilderCore;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

/// <summary>
/// 각 타일 정보 조회, 수정
/// </summary>
public class TileManager : Singleton<TileManager> // 수민
{
    [SerializeField]
    private Tilemap _groundTilemap;

    private BoundsInt _bounds;
    public BoundsInt Bounds
    {
        get => _bounds;
    }

    private TileNode[,] _gridArray;

    public TileNode[,] GridArray => _gridArray;
        
    private void Awake()
    {
        _bounds = _groundTilemap.cellBounds;
        MakeTileInfo();
    }

    private void MakeTileInfo()
    {
        int width = _bounds.size.x;
        int height = _bounds.size.y;
        _gridArray = new TileNode[width, height];

        for (int x = _bounds.xMin; x < _bounds.xMax; x++)
        {
            for (int y = _bounds.yMin; y < _bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                bool hasTile = _groundTilemap.HasTile(cellPosition);
                Vector3 worldPosition = _groundTilemap.GetCellCenterWorld(cellPosition);

                _gridArray[x - _bounds.xMin, y - _bounds.yMin] = new TileNode(x - _bounds.xMin, y - _bounds.yMin, hasTile, worldPosition);
            }
        }
    }

    public (int, int) GetNodeIndex(TileNode node)
    {
        return (node.X, node.Y);
    }

    /// <summary>
    /// 마우스 위치로 노드 정보
    /// </summary>
    /// <returns></returns>
    public TileNode GetNodeInfo()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPosition = _groundTilemap.WorldToCell(pos);

        if (_groundTilemap.HasTile(gridPosition))
        {
            return GetNodeInfo(gridPosition.x, gridPosition.y);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Vector 좌표로 노드 정보
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public TileNode GetNodeInfo(Vector3 position)
    {
        Vector3Int gridPosition = _groundTilemap.WorldToCell(position);

        if (_groundTilemap.HasTile(gridPosition))
        {
            return GetNodeInfo(gridPosition.x, gridPosition.y);
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// x, y 인덱스로 노드 정보
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public TileNode GetNodeInfo(int x, int y)
    {
        if (x < _bounds.xMin || y < _bounds.yMin || x >= _bounds.xMax || y >= _bounds.yMax)
        {
            return null;
        }
        return _gridArray[x - _bounds.xMin, y - _bounds.yMin];
    }

    public void SetNodeWalkable(int x, int y, bool flag)
    {
        if (x < _bounds.xMin || y < _bounds.yMin || x >= _bounds.xMax || y >= _bounds.yMax)
        {
            return;
        }
        _gridArray[x - _bounds.xMin, y - _bounds.yMin].IsWalkable = flag;
    }
}