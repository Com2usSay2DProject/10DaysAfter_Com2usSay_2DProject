using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;

public class ObstacleGenerate : MonoBehaviour
{
    [SerializeField] private GameObject[] obstaclePrefabs;
    [SerializeField] private float _spawnRadiusMin;
    [SerializeField] private float _spawnRadiusMax;
    [SerializeField] private int count = 50;

    [ContextMenu("Spawn In Circle")]
    void SpawnInCircle()
    {
        // 기존 자식 제거
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

#if UNITY_EDITOR
        Undo.RegisterFullObjectHierarchyUndo(this.gameObject, "Clear Old Obstacles");
#endif

        for (int i = 0; i < count; i++)
        {
            GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            GameObject obstacle = (GameObject)GameObject.Instantiate(obstaclePrefab);

            obstacle.transform.SetParent(transform);

            float angle = Random.Range(0f, 2 * Mathf.PI);
            float t = Random.Range(0f, 1f);
            float radius = Mathf.Sqrt(t) * (_spawnRadiusMax - _spawnRadiusMin) + _spawnRadiusMin;
            Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);

            obstacle.transform.position = transform.position + pos;

#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(obstacle, "Spawn Obstacle");
#endif
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(this.gameObject); // 변경 플래그 설정
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
#endif
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _spawnRadiusMax);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _spawnRadiusMin);
    }
}