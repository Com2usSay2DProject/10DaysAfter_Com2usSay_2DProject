using UnityEngine;

public class ObjectDetector : MonoBehaviour
{
    [SerializeField]
    private TilemapClickTest _towerSpawner;
    [SerializeField]
    private Grid _grid;  // Tilemap 대신 Grid 참조

    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            Ray ray = _mainCamera.ScreenPointToRay(mouseScreenPos);
            // Grid의 Z 값이 0인 평면을 기준으로 클릭 지점 찾기
            Plane gridPlane = new Plane(Vector3.back, _grid.transform.position);
            if (gridPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter); // 충돌한 월드 좌표
                //hitPoint = new Vector3(hitPoint.x * 1.55f, hitPoint.y * 1.17f, 0);

                // Grid 좌표 변환
                Vector3 cellPos = _grid.WorldToCell(hitPoint);
                cellPos.z = 0; // Z 축을 0으로 설정
                //cellPos = new Vector3(cellPos.x * 1.55f, hitPoint.y * 1.17f, 0);
                Debug.Log($"클릭한 Grid 셀 좌표: {cellPos}");
                _towerSpawner.SpawnTower(cellPos);
            }
        }
    }
}
