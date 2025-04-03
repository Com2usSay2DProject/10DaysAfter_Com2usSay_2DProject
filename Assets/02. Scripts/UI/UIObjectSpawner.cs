using UnityEngine;

public class UIObjectSpawner : MonoBehaviour
{
    public GameObject objectPrefab; // 생성할 오브젝트 프리팹
    
    void Update()
    {
        SpawnGameObject();
    }

    public void SpawnGameObject()
    {
        if (UIManager.Instance.isBuildModeActive && Input.GetMouseButtonDown(0)) // 왼쪽 클릭
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10f; // 카메라와의 거리 설정 (2D 게임에서는 z값 고정)
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Instantiate(objectPrefab, worldPosition, Quaternion.identity); // 오브젝트 생성
            UIManager.Instance.ToggleBuildModeOff();
        }
    }

}
