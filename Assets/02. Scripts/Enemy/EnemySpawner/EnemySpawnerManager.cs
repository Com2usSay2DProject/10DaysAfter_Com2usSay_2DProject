using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemySpawnerManager : MonoBehaviour
{
    public GameObject spawnerPrefab;             // 스포너 프리팹
    public int spawnerCount = 10;

    public float minSpawnDelay = 1f;   
    public float maxSpawnDelay = 5f;

    private int enableSpawnType = 1;

    private List<EnemySpawner> spawners = new List<EnemySpawner>();

    private void Start()
    {
        if (spawners.Count < spawnerCount)
        {
            int missingCount = spawnerCount - spawners.Count;
            for (int i = 0; i < missingCount; i++)
            {
                CreateSpawner();
            }
        }

        foreach (EnemySpawner spawner in spawners)
        {
            StartCoroutine(SpawnAtRandomIntervals(spawner));
        }
    }
    void CreateSpawner()
    {
        float angle = Random.Range(0f, 2 * Mathf.PI);
        float radius = Random.Range(4f, 6f);
        Vector3 pos = Vector3.zero + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        GameObject spawnerObj = Instantiate(spawnerPrefab, pos, Quaternion.identity);
        EnemySpawner spawner = spawnerObj.GetComponent<EnemySpawner>();
        if (spawner != null)
        {
            spawners.Add(spawner);
        }
    }
    IEnumerator SpawnAtRandomIntervals(EnemySpawner spawner)
    {
        while (true)
        {
            float randomDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(randomDelay);
            int randomType = Random.Range(0, enableSpawnType+1);

            spawner.Spawn((EEnemyType)randomType);
        }
    }
}



