using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ClickMoveTest : MonoBehaviour
{
    public Tilemap map;
    public float speed = 5f;

    MouseInput mouseInput;

    public TilemapClickTest tile;

    private Vector3 destination;

    private void Awake()
    {
        mouseInput = new MouseInput();
    }

    private void OnEnable()
    {
        mouseInput.Enable();
    }

    private void OnDisable()
    {
        mouseInput.Disable();
    }

    private void Start()
    {
        destination = transform.position;
        mouseInput.MouseClick.MouseClick.performed += _ => MouseClick();
        PrintCellInfo();
    }

    private void MouseClick()
    {
        /*Vector2 mousePosition = mouseInput.MouseClick.MousePosition.ReadValue<Vector2>();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3Int gridPosition = map.WorldToCell(mousePosition);*/
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3Int gridPosition = map.WorldToCell(mousePosition);

            if (map.HasTile(gridPosition))
            {
                // 클릭한 타일의 중앙 좌표를 가져옴
                Vector3 tileCenter = map.GetCellCenterWorld(gridPosition);
                destination = tileCenter;

                // 타일 색상 변경
                map.SetTileFlags(gridPosition, TileFlags.None); // 기본 플래그 제거 (색상 변경 가능하도록 설정)
                map.SetColor(gridPosition, Color.red);          // 해당 타일을 빨간색으로 변경
                destination = new Vector3(destination.x, destination.y + 0.5f, destination.z);

                tile.SpawnTower(destination);

                Debug.Log($"Grid Position: {gridPosition}");
                Debug.Log($"Tile Center Position: {tileCenter}");
            }
        }
    }
    int[,] gridArray;
    void PrintCellInfo()
    {
        BoundsInt bounds = map.cellBounds;
        gridArray = new int[bounds.size.x, bounds.size.y];

        for(int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for(int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                gridArray[x - bounds.xMin, y - bounds.yMin] = map.HasTile(cellPosition) ? 1 : 0;
            }
        }

        PrintGrid();
    }

    private void PrintGrid()
    {
        string output = "";
        for (int y = gridArray.GetLength(1) - 1; y >= 0; y--) // Y축을 뒤집어서 출력
        {
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                output += gridArray[x, y] + " ";
            }
            output += "\n";
        }
        Debug.Log(output);
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, destination) > 0.1f)
        {
            //transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        }
    }
}
