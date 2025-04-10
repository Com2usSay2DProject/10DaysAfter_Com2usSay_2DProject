using System;

[Serializable]
public class WaveData
{
    public int spawnMinnum;
    public int spawnMaxnum;
    public float minSpawnDelay;
    public float maxSpawnDelay;
    public int enableSpawnType; //4까지만
    public bool useUnipueEnemy;
    public bool useBossEnemy;
    public bool useClusterEnemy;
}
