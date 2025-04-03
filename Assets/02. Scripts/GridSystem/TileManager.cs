using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 각 타일 정보 조회, 수정
/// </summary>
public class TileManager : Singleton<TileManager> // 수민
{
    [SerializeField]
    private Tilemap _groundTilemap;

    private TileNode[,] _gridArray;
    public TileNode[,] GridArray
    {
        get => _gridArray;
    }

    private void Awake()
    {
        MakeTileInfo();
    }

    private void MakeTileInfo()
    {
        BoundsInt bounds = _groundTilemap.cellBounds;
        _gridArray = new TileNode[bounds.size.x, bounds.size.y];

        for(int x=0; x<bounds.xMax; x++)
        {
            for(int y=0; y<bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                TileNode thisCell = new TileNode(x, y, _groundTilemap.HasTile(cellPosition), _groundTilemap.GetCellCenterWorld(cellPosition));
                _gridArray[x - bounds.xMin, y - bounds.yMin] = thisCell;
            }
        }
    }
}