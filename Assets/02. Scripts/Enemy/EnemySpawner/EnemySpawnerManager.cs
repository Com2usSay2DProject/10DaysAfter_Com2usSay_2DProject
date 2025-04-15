using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    public GameObject spawnerPrefab;             // 스포너 프리팹

    List<WaveData> WaveDatas;
    int WaveDatasCurIndex = 0;

    //-----------------------------------

    [SerializeField] private float _spawnRadiusMin;
    [SerializeField] private float _spawnRadiusMax;

    //스포너들
    [SerializeField] private List<EnemySpawner> spawners;
    private EnemySpawner ClusterSpawner;
    private EnemySpawner UniqueSpawner;

    //코루틴 저장용
    private Dictionary<EnemySpawner, Coroutine> spawnerCoroutines = new Dictionary<EnemySpawner, Coroutine>();
    private Coroutine ClusterSpawnerCoroutine;
    private Coroutine UniqueSpawnerCoroutines;

    [Header("ClusterSpawnerSetting")]
    //[SerializeField] Transform[] ClusterSpawnerPos;
    [SerializeField] private int _clusterEnemyNum;
    [SerializeField] private float _clusterRadius;

    private void Awake()
    {
        //spawners = new List<EnemySpawner>();
        GetData();

    }
    private void Start()
    {
        WaveDatasCurIndex = PhaseManager.Instance.CurrentDay - 1;

        if (spawners.Count == 0)
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

    }
    private void GetData()
    {
        WaveDataCollection collection;

#if UNITY_EDITOR
        // 에디터에서는 직접 파일에서 로딩
        collection = JsonDataManager.LoadFromFile<WaveDataCollection>("Wave/WaveDataCollection");
#else
    // 빌드에서는 Resources.Load 사용
    TextAsset jsonText = Resources.Load<TextAsset>("Json/Wave/WaveDataCollection");
    if (jsonText == null)
    {
        Debug.LogError("웨이브 데이터 파일을 찾을 수 없습니다 (빌드 환경)");
        return;
    }
    collection = JsonDataManager.FromJson<WaveDataCollection>(jsonText.text);
#endif

        WaveDatas = collection.Datas;

        Debug.Log("웨이브 데이터 로드 완료");
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

    }
    void CreateClusterSpanwer()
    {

        ClusterSpawner = Instantiate(spawnerPrefab).GetComponent<EnemySpawner>();
        SetRandPos(ClusterSpawner);

        UniqueSpawner = Instantiate(spawnerPrefab).GetComponent<EnemySpawner>();
        SetRandPos(UniqueSpawner);
        UniqueSpawner.SetPath();
    }
    IEnumerator SpawnAtRandomIntervals(EnemySpawner spawner)
    {
        int curIndex = WaveDatasCurIndex;

        while (true)
        {
            int spawnNum = Random.Range(WaveDatas[curIndex].spawnMinnum, WaveDatas[curIndex].spawnMaxnum);

            float randomDelay = Random.Range(WaveDatas[curIndex].minSpawnDelay, WaveDatas[curIndex].maxSpawnDelay);
            yield return new WaitForSeconds(randomDelay);
            for (int i = 0; i < spawnNum; ++i)
            {

                int randomType = Random.Range(0, WaveDatas[curIndex].enableSpawnType);

                spawner.Spawn((EEnemyType)randomType);
            }
        }
    }

    void DisActiveSpawners()
    {
        WaveDatasCurIndex = PhaseManager.Instance.CurrentDay - 1;
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

        if (ClusterSpawner != null)
        {
            ClusterSpawner.gameObject.SetActive(false);
            if(ClusterSpawnerCoroutine!=null)
                StopCoroutine(ClusterSpawnerCoroutine);
        }
        if (UniqueSpawner != null)
        {
            UniqueSpawner.gameObject.SetActive(false);
            if (UniqueSpawnerCoroutines != null)
                StopCoroutine(UniqueSpawnerCoroutines);
        }
    }
    void ActiveSpawners()
    {

        if (ClusterSpawner != null)
            ClusterSpawner.gameObject.SetActive(true);
        if (UniqueSpawner != null)
            UniqueSpawner.gameObject.SetActive(true);

        //나중에 코루틴 종료할때 필요해서 저장
        foreach (var spawner in spawners)
        {
            spawner.SetPath();
            spawner.gameObject.SetActive(true);
            if (!spawnerCoroutines.ContainsKey(spawner))
            {
                Coroutine co = StartCoroutine(SpawnAtRandomIntervals(spawner));
                spawnerCoroutines[spawner] = co;
            }
        }
    }
    private void Update()
    {
        if (PhaseManager.Instance.isNight == false) return;

        if (WaveDatas[WaveDatasCurIndex].useClusterEnemy /*&& ClusterSpawner.gameObject.activeSelf*/ && ClusterSpawnerCoroutine == null)
        {
            SetRandPos(ClusterSpawner);
            ClusterSpawnerCoroutine = StartCoroutine(SpawnClusterEnemy(ClusterSpawner));
        }

        if (WaveDatas[WaveDatasCurIndex].useUnipueEnemy && UniqueSpawnerCoroutines ==null)
        {
            SetRandPos(UniqueSpawner);
            UniqueSpawnerCoroutines = StartCoroutine(SpawnUniqueEnemy(UniqueSpawner));
        }
    }
    IEnumerator SpawnClusterEnemy(EnemySpawner spawner)
    {
        yield return new WaitForSeconds(10f);

        spawner.SpawnEnemyCluster(30, 3);
        ClusterSpawnerCoroutine = null;
    }
    IEnumerator SpawnUniqueEnemy(EnemySpawner spawner)
    {
        yield return new WaitForSeconds(10f);

        int randInt = Random.Range((int)EEnemyType.Unique1, (int)EEnemyType.Unique2 + 1);

        spawner.Spawn((EEnemyType)randInt);
        UniqueSpawnerCoroutines = null;
    }
    private void SetRandPos(EnemySpawner spawner)
    {
        if (spawner == null || spawners == null || spawners.Count == 0) return;

        // spawners 리스트에서 랜덤한 스포너 하나 선택
        int randIndex = Random.Range(0, spawners.Count);
        Vector3 randomSpawnerPos = spawners[randIndex].transform.position;

        // 선택된 위치로 현재 스포너 이동
        spawner.transform.position = randomSpawnerPos;
        spawner.SetPath();
    }


}



