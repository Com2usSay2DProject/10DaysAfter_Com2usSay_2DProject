using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnemySpawnerManager : MonoBehaviour
{
    public GameObject spawnerPrefab;             // 스포너 프리팹

    //public WaveData[] WaveDatas = new WaveData[10];
    //private WaveData CurrentData;

    //웨이브 데이터 필요한거 
    public int spawnerCount = 10;
    public float minSpawnDelay = 1f;
    public float maxSpawnDelay = 5f;
    private int enableSpawnType = 1; //-> 애너미 타입중에 해당 웨이브에서 나오게할 종류의수 int를 애너미 타입으로 형변환해서 사용함 현재 4종류 적 예정?
    //-----------------------------------

    [SerializeField] private float _spawnRadiusMin;
    [SerializeField] private float _spawnRadiusMax;

    //스포너들
    private List<EnemySpawner> spawners = new List<EnemySpawner>();
    //코루틴 저장용
    private Dictionary<EnemySpawner, Coroutine> spawnerCoroutines = new Dictionary<EnemySpawner, Coroutine>();


    private void Start()
    {

        for (int i = 0; i < spawnerCount; i++)
        {
            CreateSpawner();
        }


        PhaseManager.Instance.OnDayBegin += DisActiveSpawners;
        PhaseManager.Instance.OnNightBegin += ActiveSpawners;
    }
    void CreateSpawner()
    {
        float angle = Random.Range(0f, 2 * Mathf.PI);
        float radius = Random.Range(_spawnRadiusMin, _spawnRadiusMax);
        Vector3 pos = Vector3.zero + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        GameObject spawnerObj = Instantiate(spawnerPrefab, pos, Quaternion.identity);
        spawnerObj.transform.SetParent(transform);
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
            int randomType = Random.Range(0, enableSpawnType + 1);

            //spawner.Spawn((EEnemyType)randomType);
            spawner.Spawn(EEnemyType.Boomer);
        }
    }

    void DisActiveSpawners()
    {
        //스포너 멈추고 코루틴도 멈추기
        foreach (var spawner in spawners)
        {
            spawner.gameObject.SetActive(false);
            if (spawnerCoroutines.ContainsKey(spawner))
            {
                StopCoroutine(spawnerCoroutines[spawner]);
                spawnerCoroutines.Remove(spawner);
            }
        }
    }
    void ActiveSpawners()
    {
        spawnerCount += 2;

        //스포너 부족하면 생성
        if (spawners.Count < spawnerCount)
        {
            int missingCount = spawnerCount - spawners.Count;
            for (int i = 0; i < missingCount; i++)
            {
                CreateSpawner();
            }
        }
        //나중에 코루틴 종료할때 필요해서 저장
        foreach (var spawner in spawners)
        {
            spawner.gameObject.SetActive(true);
            if (!spawnerCoroutines.ContainsKey(spawner))
            {
                Coroutine co = StartCoroutine(SpawnAtRandomIntervals(spawner));
                spawnerCoroutines[spawner] = co;
            }
        }
    }
}



