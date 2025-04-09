using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;



public class EnemySpawnerManager : MonoBehaviour
{
    public GameObject spawnerPrefab;             // 스포너 프리팹

    List<WaveData> WaveDatas;
    int WaveDatasCurIndex = 0;

    //웨이브 데이터 필요한거 
    //public int spawnerCount = 10;
    //public float minSpawnDelay = 1f;
    //public float maxSpawnDelay = 5f;
    //private int enableSpawnType = 6; //-> 애너미 타입중에 해당 웨이브에서 나오게할 종류의수 int를 애너미 타입으로 형변환해서 사용함 현재 4종류 적 예정?
    //-----------------------------------

    [SerializeField] private float _spawnRadiusMin;
    [SerializeField] private float _spawnRadiusMax;

    //스포너들
    [SerializeField] private List<EnemySpawner> spawners;
    private EnemySpawner ClusterSpawner;
    //코루틴 저장용
    private Dictionary<EnemySpawner, Coroutine> spawnerCoroutines = new Dictionary<EnemySpawner, Coroutine>();

    [Header("ClusterSpawnerSetting")]
    [SerializeField] Transform[] ClusterSpawnerPos;
    [SerializeField] int ClusterEnemyNum;
    [SerializeField] float ClusterRadius;

    private void Awake()
    {
        //spawners = new List<EnemySpawner>();
        GetData();


    }
    private void Start()
    {
        WaveDatasCurIndex = PhaseManager.Instance.CurrentDay - 1;

        if (spawners.Count ==0)
        {
            spawners = new List<EnemySpawner>();
            for (int i = 0; i < 12; i++)
            {
                CreateSpawner();
            }
        }

        CreateClusterSpanwer();

        PhaseManager.Instance.OnDayBegin += DisActiveSpawners;
        PhaseManager.Instance.OnNightBegin += ActiveSpawners;
        if (ClusterSpawnerPos.Length >= 0)
            PhaseManager.Instance.OnNightBegin += SpawnCluster;
    }
    private void GetData()
    {
        WaveDataCollection collection = JsonDataManager.LoadFromFile<WaveDataCollection>("Wave/WaveDataCollection");
        WaveDatas = collection.Datas;

        Debug.Log("적 데이터 로드 완료");
    }
    private void SpawnCluster()
    {
        if (ClusterSpawnerPos.Length > 0)
            ClusterSpawner.SpawnEnemyCluster(ClusterEnemyNum, ClusterRadius);

    }

    void CreateSpawner()
    {
        // angle은 0 ~ 2π 사이에서 균일하게 선택
        float angle = Random.Range(0f, 2 * Mathf.PI);

        //균일한 분포로 변환
        float t = Random.Range(0f, 1f);
        float radius = Mathf.Sqrt(t) * (_spawnRadiusMax - _spawnRadiusMin) + _spawnRadiusMin;

        Vector3 pos = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);

        // 스포너 생성 및 등록
        GameObject spawnerObj = Instantiate(spawnerPrefab, pos, Quaternion.identity);
        spawnerObj.transform.SetParent(transform);

        EnemySpawner spawner = spawnerObj.GetComponent<EnemySpawner>();
        if (spawner != null)
        {
            spawner.gameObject.SetActive(false);
            spawners.Add(spawner);
        }
        //float angle = Random.Range(0f, 2 * Mathf.PI);
        //float radius = Random.Range(_spawnRadiusMin, _spawnRadiusMax);
        //Vector3 pos = Vector3.zero + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        //GameObject spawnerObj = Instantiate(spawnerPrefab, pos, Quaternion.identity);
        //spawnerObj.transform.SetParent(transform);
        //EnemySpawner spawner = spawnerObj.GetComponent<EnemySpawner>();
        //if (spawner != null)
        //{
        //    spawners.Add(spawner);
        //}
    }
    void CreateClusterSpanwer()
    {

        int rand = Random.Range(0, 4);
        // 스포너 생성 및 등록
        if (ClusterSpawnerPos.Length > 0)
            ClusterSpawner = Instantiate(spawnerPrefab, ClusterSpawnerPos[rand].position, Quaternion.identity).GetComponent<EnemySpawner>();
    }
    IEnumerator SpawnAtRandomIntervals(EnemySpawner spawner)
    {
        while (true)
        {
            float randomDelay = Random.Range(WaveDatas[WaveDatasCurIndex].minSpawnDelay, WaveDatas[WaveDatasCurIndex].maxSpawnDelay);
            yield return new WaitForSeconds(randomDelay);
            int randomType = Random.Range(0, WaveDatas[WaveDatasCurIndex].enableSpawnType);

            spawner.Spawn((EEnemyType)randomType);
            //spawner.Spawn(EEnemyType.Unique2);

        }
    }

    void DisActiveSpawners()
    {
        WaveDatasCurIndex = PhaseManager.Instance.CurrentDay -1;
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
        if (ClusterSpawnerPos.Length > 0)
            ClusterSpawner.gameObject.SetActive(false);
    }
    void ActiveSpawners()
    {

        if (ClusterSpawnerPos.Length > 0)
            ClusterSpawner.gameObject.SetActive(true);

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



