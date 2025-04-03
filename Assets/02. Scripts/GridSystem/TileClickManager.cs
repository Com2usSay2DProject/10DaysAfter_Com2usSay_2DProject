using UnityEngine;

public class TileClickManager : Singleton<TileClickManager>
{
    public bool IsBuildingMode;

    private TileNode _selectedNode;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 맞은 오브젝트에 "Tile" 태그가 없을 경우
                if (!hit.transform.CompareTag("Tile"))
                {
                    Debug.Log("클릭한 위치가 타일이 아님: " + hit.transform.name);
                    // 여기에 원하는 동작 추가

                    IsBuildingMode = true;
                }
                else
                {
                    Debug.Log("클릭한 위치가 타일임");
                    IsBuildingMode = false;
                }
            }
            else
            {

            }
        }
    }
}
