using System.Collections.Generic;
using UnityEngine;

public class EnemySoundManager : MonoBehaviour
{
    public static EnemySoundManager Instance;

    public AudioSource AudioSource;

    [Header("사운드 클립 설정")]
    public AudioClip[] moveClips;
    public AudioClip[] attackClips;
    public AudioClip[] deathClips;

    [Header("사운드 나올 확률")]
    public float deathPlayChance = 0.3f;
    public float movePlayChance = 0.2f;
    public float attackPlayChance = 0.7f;

    [Header("사운드 제한거리, 사운드 쿨타임")]
    public float radiusLimit = 5f;
    public float soundCooldown = 0.5f;

    // 타입별 최근 재생 위치 저장용
    private Dictionary<EnemySoundType, List<Vector3>> recentSoundPositions = new();
    private float lastCleanupTime;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (EnemySoundType type in System.Enum.GetValues(typeof(EnemySoundType)))
            {
                recentSoundPositions[type] = new List<Vector3>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Time.time - lastCleanupTime > soundCooldown)
        {
            foreach (var key in recentSoundPositions.Keys)
            {
                recentSoundPositions[key].Clear();
            }
            lastCleanupTime = Time.time;
        }
    }

    public void Play3DSoundWithLimit(Vector3 position, EnemySoundType soundType)
    {
        float chance = soundType switch
        {
            EnemySoundType.EnemyMove => movePlayChance,
            EnemySoundType.EnemyAttck => attackPlayChance,
            EnemySoundType.EnemyDead => deathPlayChance,
            _ => 1f
        };

        if (Random.value > chance) return;

        // 중복 방지
        List<Vector3> recentPositions = recentSoundPositions[soundType];
        foreach (var pos in recentPositions)
        {
            if (Vector3.Distance(pos, position) < radiusLimit)
                return;
        }

        // 클립 선택
        AudioClip[] clipArray = soundType switch
        {
            EnemySoundType.EnemyMove => moveClips,
            EnemySoundType.EnemyAttck => attackClips,
            EnemySoundType.EnemyDead => deathClips,
            _ => null
        };

        if (clipArray == null || clipArray.Length == 0) return;

        AudioClip clip = clipArray[Random.Range(0, clipArray.Length)];
        AudioSource.PlayClipAtPoint(clip, position, 1f);

        recentPositions.Add(position);
    }
}
