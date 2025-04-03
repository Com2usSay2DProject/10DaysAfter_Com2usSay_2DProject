using CityBuilderCore;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 각 타일 정보 조회, 수정
/// </summary>
public class TileManager : Singleton<TileManager> // 수민
{
    [SerializeField]
    private Tilemap _groundTilemap;

    private BoundsInt _bounds;
    private TileNode[,] _gridArray;

    public TileNode[,] GridArray => _gridArray;

    public Action NodeClickAction;
        
    private void Awake()
    {
        _bounds = _groundTilemap.cellBounds;
        MakeTileInfo();
    }

    private void Update()
    {
        //if(Input.GetMouseButtonDown(0))
        //{
        //    NodeClickAction?.Invoke();
        //}
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hit.collider != null)
            {
                if (!hit.transform.CompareTag("Tile"))
                {
                    Debug.Log("클릭한 위치가 타일이 아님: " + hit.transform.name);
                }
                else
                {
                    Debug.Log("클릭한 위치가 타일임");
                    TileNode clickedTile = GetNodeInfo();
                    if (clickedTile != null && clickedTile.IsWalkable)
                    {
                        TilemapClickTest.Instance.SpawnTower(clickedTile.WorldPositon);
                        clickedTile.IsWalkable = false;
                    }
                }
            }
            else
            {
                Debug.Log("클릭한 위치에 오브젝트가 없음");
            }
        }
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

                _gridArray[x - _bounds.xMin, y - _bounds.yMin] = new TileNode(x, y, hasTile, worldPosition);
            }
        }
    }

    public (int, int) GetNodeIndex(TileNode node)
    {
        return (node.X, node.Y);
    }
    public TileNode GetNodeInfo()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPosition = _groundTilemap.WorldToCell(pos);

        if (_groundTilemap.HasTile(gridPosition))
        {
            return GetNodeInfo(gridPosition);
        }
        else
        {
            return null;
        }
    }

    public TileNode GetNodeInfo(Vector3 position)
    {
        int x = (int)position.x;
        int y = (int)position.y;

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