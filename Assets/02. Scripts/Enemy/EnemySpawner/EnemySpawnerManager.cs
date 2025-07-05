using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    [Header("프리팹")]
    public GameObject spawnerPrefab;

    [Header("스포너 범위")]
    [SerializeField] private float spawnRadiusMin = 5f;
    [SerializeField] private float spawnRadiusMax = 10f;

    [Header("물량 좀비 수")]
    [SerializeField] private int clusterEnemyNum = 30;
    [SerializeField] private float clusterRadius = 3f;

    // 웨이브 데이터
    private List<WaveData> waveDatas;
    private int currentWaveIndex = 0;

    // 스포너
    private List<EnemySpawner> mainSpawners = new List<EnemySpawner>();
    private EnemySpawner clusterSpawner;
    private EnemySpawner uniqueSpawner;

    // 코루틴
    private Dictionary<EnemySpawner, Coroutine> spawnerCoroutines = new Dictionary<EnemySpawner, Coroutine>();
    private Coroutine clusterSpawnerCoroutine;
    private Coroutine uniqueSpawnerCoroutine;

    private const float SPECIAL_SPAWN_DELAY = 10f;

    private void Awake()
    {
        InitializeSpawnerList();
        LoadWaveData();
    }

    private void Start()
    {
        SetupInitialWave();
        SetupInitialSpawners();
        RegisterPhaseEvents();
    }

    private void Update()
    {
        if (!PhaseManager.Instance.isNight) return;

        HandleSpecialSpawners();
    }

    private void OnDestroy()
    {
        UnregisterPhaseEvents();
    }


    private void InitializeSpawnerList()
    {
        mainSpawners = new List<EnemySpawner>();
    }

    private void SetupInitialWave()
    {
        currentWaveIndex = PhaseManager.Instance.CurrentDay - 1;
    }

    private void SetupInitialSpawners()
    {
        CreateMainSpawners();
        SetAllSpawnersToRandomPositions();
        CreateSpecialSpawners();
    }

    private void RegisterPhaseEvents()
    {
        PhaseManager.Instance.OnDayBegin += OnDayBegin;
        PhaseManager.Instance.OnNightBegin += OnNightBegin;
    }

    private void UnregisterPhaseEvents()
    {
        if (PhaseManager.Instance != null)
        {
            PhaseManager.Instance.OnDayBegin -= OnDayBegin;
            PhaseManager.Instance.OnNightBegin -= OnNightBegin;
        }
    }
    private void LoadWaveData()
    {
        WaveDataCollection collection = LoadWaveDataCollection();
        waveDatas = collection.Datas;
        Debug.Log("웨이브 데이터 로드 완료");
    }

    private WaveDataCollection LoadWaveDataCollection()
    {
#if UNITY_EDITOR
        return JsonDataManager.LoadFromFile<WaveDataCollection>("Wave/WaveDataCollection");
#else
        TextAsset jsonText = Resources.Load<TextAsset>("Json/Wave/WaveDataCollection");
        if (jsonText == null)
        {
            Debug.LogError("웨이브 데이터 파일을 찾을 수 없습니다 (빌드 환경)");
            return new WaveDataCollection();
        }
        return JsonDataManager.FromJson<WaveDataCollection>(jsonText.text);
#endif
    }

    private WaveData GetCurrentWaveData()
    {
        return waveDatas[currentWaveIndex];
    }

    private void CreateMainSpawners()
    {
        int requiredCount = GetCurrentWaveData().spawnerCount;
        int currentCount = mainSpawners.Count;
        int toCreate = requiredCount - currentCount;

        if (toCreate <= 0) return;

        for (int i = 0; i < toCreate; i++)
        {
            CreateAndAddMainSpawner();
        }
    }

    private void CreateAndAddMainSpawner()
    {
        GameObject spawnerObj = Instantiate(spawnerPrefab, transform);
        EnemySpawner spawner = spawnerObj.GetComponent<EnemySpawner>();

        if (spawner != null)
        {
            spawner.gameObject.SetActive(false);
            mainSpawners.Add(spawner);
        }
    }

    private void CreateSpecialSpawners()
    {
        clusterSpawner = CreateSpawner();
        uniqueSpawner = CreateSpawner();

        SetRandomPositionForSpecialSpawner(clusterSpawner);
        SetRandomPositionForSpecialSpawner(uniqueSpawner);
        uniqueSpawner.SetPath();
    }

    private EnemySpawner CreateSpawner()
    {
        GameObject spawnerObj = Instantiate(spawnerPrefab);
        return spawnerObj.GetComponent<EnemySpawner>();
    }

    private void SetAllSpawnersToRandomPositions()
    {
        if (mainSpawners == null || mainSpawners.Count == 0) return;

        foreach (EnemySpawner spawner in mainSpawners)
        {
            Vector3 randomPos = GetRandomSpawnPosition();
            spawner.transform.position = transform.position + randomPos;
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);
        float t = Random.Range(0f, 1f);
        float radius = Mathf.Sqrt(t) * (spawnRadiusMax - spawnRadiusMin) + spawnRadiusMin;

        return new Vector3(
            Mathf.Cos(angle) * radius,
            Mathf.Sin(angle) * radius,
            0f
        );
    }

    private void SetRandomPositionForSpecialSpawner(EnemySpawner spawner)
    {
        if (spawner == null || mainSpawners.Count == 0) return;

        int randIndex = Random.Range(0, mainSpawners.Count);
        Vector3 randomSpawnerPos = mainSpawners[randIndex].transform.position;

        spawner.transform.position = randomSpawnerPos;
        spawner.SetPath();
    }


    private void StartMainSpawnerCoroutines()
    {
        foreach (EnemySpawner spawner in mainSpawners)
        {
            spawner.SetPath();
            spawner.gameObject.SetActive(true);

            if (!spawnerCoroutines.ContainsKey(spawner))
            {
                Coroutine coroutine = StartCoroutine(SpawnAtRandomIntervals(spawner));
                spawnerCoroutines[spawner] = coroutine;
            }
        }
    }

    private void StopAllMainSpawnerCoroutines()
    {
        foreach (EnemySpawner spawner in mainSpawners)
        {
            spawner.gameObject.SetActive(false);

            if (spawnerCoroutines.ContainsKey(spawner))
            {
                StopCoroutine(spawnerCoroutines[spawner]);
                spawnerCoroutines.Remove(spawner);
            }
        }
    }

    private void StopSpecialSpawnerCoroutines()
    {
        if (clusterSpawner != null)
        {
            clusterSpawner.gameObject.SetActive(false);
            if (clusterSpawnerCoroutine != null)
            {
                StopCoroutine(clusterSpawnerCoroutine);
                clusterSpawnerCoroutine = null;
            }
        }

        if (uniqueSpawner != null)
        {
            uniqueSpawner.gameObject.SetActive(false);
            if (uniqueSpawnerCoroutine != null)
            {
                StopCoroutine(uniqueSpawnerCoroutine);
                uniqueSpawnerCoroutine = null;
            }
        }
    }

    private IEnumerator SpawnAtRandomIntervals(EnemySpawner spawner)
    {
        WaveData currentWave = GetCurrentWaveData();

        while (true)
        {
            int spawnNum = Random.Range(currentWave.spawnMinnum, currentWave.spawnMaxnum + 1);
            float randomDelay = Random.Range(currentWave.minSpawnDelay, currentWave.maxSpawnDelay);

            yield return new WaitForSeconds(randomDelay);

            for (int i = 0; i < spawnNum; i++)
            {
                EEnemyType randomType = (EEnemyType)Random.Range(0, currentWave.enableSpawnType);
                spawner.Spawn(randomType);
            }
        }
    }

    private IEnumerator SpawnClusterEnemy(EnemySpawner spawner)
    {
        yield return new WaitForSeconds(SPECIAL_SPAWN_DELAY);
        spawner.SpawnEnemyCluster(clusterEnemyNum, clusterRadius);
        clusterSpawnerCoroutine = null;
    }

    private IEnumerator SpawnUniqueEnemy(EnemySpawner spawner)
    {
        yield return new WaitForSeconds(SPECIAL_SPAWN_DELAY);

        EEnemyType uniqueType = GetRandomUniqueEnemyType();
        spawner.Spawn(uniqueType);

        uniqueSpawnerCoroutine = null;
    }

    private EEnemyType GetRandomUniqueEnemyType()
    {
        return (EEnemyType)Random.Range((int)EEnemyType.Unique1, (int)EEnemyType.Unique2 + 1);
    }



    private void OnDayBegin()
    {
        currentWaveIndex = PhaseManager.Instance.CurrentDay - 1;
        StopAllMainSpawnerCoroutines();
        StopSpecialSpawnerCoroutines();
    }

    private void OnNightBegin()
    {
        CreateMainSpawners();
        SetAllSpawnersToRandomPositions();
        ActivateSpecialSpawners();
        StartMainSpawnerCoroutines();
    }

    private void ActivateSpecialSpawners()
    {
        if (clusterSpawner != null)
            clusterSpawner.gameObject.SetActive(true);

        if (uniqueSpawner != null)
            uniqueSpawner.gameObject.SetActive(true);
    }



    private void HandleSpecialSpawners()
    {
        WaveData currentWave = GetCurrentWaveData();

        HandleClusterSpawner(currentWave);
        HandleUniqueSpawner(currentWave);
    }

    private void HandleClusterSpawner(WaveData waveData)
    {
        if (waveData.useClusterEnemy && clusterSpawnerCoroutine == null)
        {
            SetRandomPositionForSpecialSpawner(clusterSpawner);
            clusterSpawnerCoroutine = StartCoroutine(SpawnClusterEnemy(clusterSpawner));
        }
    }

    private void HandleUniqueSpawner(WaveData waveData)
    {
        if (waveData.useUnipueEnemy && uniqueSpawnerCoroutine == null)
        {
            SetRandomPositionForSpecialSpawner(uniqueSpawner);
            uniqueSpawnerCoroutine = StartCoroutine(SpawnUniqueEnemy(uniqueSpawner));
        }
    }


    private void OnDrawGizmos()
    {
        DrawSpawnRangeGizmos();
    }

    private void DrawSpawnRangeGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadiusMax);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, spawnRadiusMin);
    }

}



