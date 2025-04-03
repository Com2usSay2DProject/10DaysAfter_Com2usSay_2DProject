using UnityEngine;

public class TileClickManager : Singleton<TileClickManager>
{
    public bool IsBuildingMode;

    private TileNode _selectedNode;

    private void Update()
    {
        // 1. 빌드 모드인가?
        // 2. 해당 위치에 건물이 지어져 있는가 -> IsWalkable = true 일 때 통과
        // 둘다 통과하면 건설 -> 빌드 모드 해제
        // else if
        // 1. 빌드 모드가 아니고, 클릭 한 것이 건물 -> 업그레이드 UI
        GetMouseClick();
    }

    private void GetMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
            if(hit.collider == null)
            {
                return;
            }

            if (UIManager.Instance.isBuildModeActive)
            {
                if (hit.transform.CompareTag("Tile"))
                {
                    Debug.Log("클릭한 위치가 타일임");
                    TileNode clickedTile = TileManager.Instance.GetNodeInfo();
                    if (clickedTile != null && clickedTile.IsWalkable)
                    {
                        TowerSpawner.Instance.SpawnTower(clickedTile.WorldPositon);
                        clickedTile.IsWalkable = false;
                        UIManager.Instance.ToggleBuildModeOff();
                    }
                }
                else
                {
                    Debug.Log("클릭한 위치가 타일이 아님: " + hit.transform.name);
                }
            }
            else
            {
                if(hit.transform.CompareTag("Tower"))
                {
                    // TODO : 업그레이드 UI 띄우기
                    Debug.Log("타워 업그레이드 UI를 띄워주세요");
                }
            }
        }
    }
}
