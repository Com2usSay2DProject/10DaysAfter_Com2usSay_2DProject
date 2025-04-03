//using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine;
//public class TestNode
//{
//    public int x, y;
//    public bool isWalkable;

//    public int gCost, hCost;
//    public TestNode parent;

//    public int fCost => gCost + hCost;

//    public TestNode(int x, int y, bool isWalkable)
//    {
//        this.x = x;
//        this.y = y;
//        this.isWalkable = isWalkable;
//    }
//}
//public class TestGridManager : MonoBehaviour
//{
//    public static TestGridManager Instance;

//    public int width, height;
//    public float cellSize;
//    private TestNode[,] grid;

//    public LayerMask obstacleLayer;

//    private void Awake()
//    { 
//        Instance = this;
//        CreateGrid();
//    }

//    private void CreateGrid()
//    {
//        grid = new TestNode[width, height];

//        for (int x = 0; x < width; x++)
//        {
//            for (int y = 0; y < height; y++)
//            {
//                Vector3 worldPos = GetWorldPosition(x, y);
//                bool isWalkable = !Physics2D.OverlapCircle(worldPos, cellSize * 0.4f, obstacleLayer);
//                grid[x, y] = new TestNode(x, y, isWalkable);
//            }
//        }
//    }

//    public TestNode GetNodeFromWorldPos(Vector3 worldPos)
//    {
//        // 그리드 원점 계산
//        Vector3 gridOrigin = new Vector3(-width * cellSize / 2f, -height * cellSize / 2f, 0f);
//        int x = Mathf.Clamp(Mathf.FloorToInt((worldPos.x - gridOrigin.x) / cellSize), 0, width - 1);
//        int y = Mathf.Clamp(Mathf.FloorToInt((worldPos.y - gridOrigin.y) / cellSize), 0, height - 1);
//        return grid[x, y];
//    }

//    public Vector3 GetWorldPosition(int x, int y)
//    {
//        Vector3 gridOrigin = new Vector3(-width * cellSize / 2f, -height * cellSize / 2f, 0f);
//        return gridOrigin + new Vector3(x + 0.5f, y + 0.5f, 0f) * cellSize;
//    }

//    public List<TestNode> GetNeighbors(TestNode node)
//    {
//        List<TestNode> neighbors = new List<TestNode>();

//        int[,] offsets = new int[,]
//        {
//        { 0, 1 },   // 상
//        { 1, 0 },   // 우
//        { 0, -1 },  // 하
//        { -1, 0 },  // 좌
//        { 1, 1 },   // 우상
//        { 1, -1 },  // 우하
//        { -1, 1 },  // 좌상
//        { -1, -1 }  // 좌하
//        };

//        for (int i = 0; i < offsets.GetLength(0); i++)
//        {
//            int newX = node.x + offsets[i, 0];
//            int newY = node.y + offsets[i, 1];

//            if (newX >= 0 && newY >= 0 && newX < width && newY < height)
//            {
//                neighbors.Add(grid[newX, newY]);
//            }
//        }

//        return neighbors;
//    }

//    private void OnDrawGizmos()
//    {
//        if (grid == null)
//        {
//            CreateGrid();
//        }

//        for (int x = 0; x < width; x++)
//        {
//            for (int y = 0; y < height; y++)
//            {
//                TestNode node = grid[x, y];
//                Gizmos.color = node.isWalkable ? Color.white : Color.red;
//                Vector3 worldPos = GetWorldPosition(x, y);
//                Gizmos.DrawWireCube(worldPos, Vector3.one * cellSize);
//            }
//        }
//    }
//}
