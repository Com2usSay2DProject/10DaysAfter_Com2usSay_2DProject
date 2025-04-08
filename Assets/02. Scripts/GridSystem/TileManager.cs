using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 각 타일 정보 조회, 수정
/// </summary>
public class TileManager : Singleton<TileManager> // 수민
{
    [Header("# Tilemap")]
    [SerializeField]
    private Tilemap _groundTilemap;
    private BoundsInt _bounds;
    public BoundsInt Bounds
    {
        get => _bounds;
    }
    public Bounds WorldBounds;
    private TileNode[,] _gridArray;
    public TileNode[,] GridArray => _gridArray;

    [Header("# Obstacles")]
    [SerializeField] private GameObject[] Trees;
        
    private void Awake()
    {
        _bounds = _groundTilemap.cellBounds;
        WorldBounds = TransformBoundsToWorld(_groundTilemap.transform, _groundTilemap.localBounds);

        Debug.Log($"Bounds: xMin:{_bounds.xMin}, xMax:{_bounds.xMax}, yMin:{_bounds.yMin}, yMax:{_bounds.yMax}");

        MakeTileInfo();
        MakeTrees();
    }

    public static Bounds TransformBoundsToWorld(Transform transform, Bounds localBounds)
    {
        Vector3 center = transform.TransformPoint(localBounds.center);
        Vector3 extents = localBounds.extents;
        Vector3 worldExtents = new Vector3(
            Mathf.Abs(transform.lossyScale.x) * extents.x,
            Mathf.Abs(transform.lossyScale.y) * extents.y,
            Mathf.Abs(transform.lossyScale.z) * extents.z
        );
        return new Bounds(center, worldExtents * 2);
    }

    private void MakeTrees()
    {
        int treeCount = 50; // 생성할 나무 개수
        int width = _gridArray.GetLength(0);
        int height = _gridArray.GetLength(1);

        int tries = 0;
        int maxTries = 50;

        while (treeCount > 0 && tries < maxTries)
        {
            tries++;

            int randX = Random.Range(0, width);
            int randY = Random.Range(0, height);
            TileNode node = _gridArray[randX, randY];

            // 타일이 존재하고, 아직 장애물이 없는 곳에만 나무 생성
            if (node.IsWalkable)
            {
                GameObject treePrefab = Trees[Random.Range(0, Trees.Length)];
                GameObject tree = Instantiate(treePrefab, node.WorldPositon, Quaternion.identity, transform);

                node.IsWalkable = false;
                treeCount--;

                Debug.Log($"Tree placed at: ({node.X}, {node.Y})");
            }
        }

        if (treeCount > 0)
        {
            Debug.LogWarning($"{treeCount}개의 나무를 배치하지 못했습니다. (시도 제한 도달)");
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
            return GetNodeInfo(pos); //여까진 맞음
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
            return GetNodeInfo(gridPosition.x - _bounds.xMin, gridPosition.y - _bounds.yMin);
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
        if (x < 0 || y < 0 || x >= _gridArray.GetLength(0) || y >= _gridArray.GetLength(1))
        {
            return null;
        }
        return _gridArray[x, y];
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