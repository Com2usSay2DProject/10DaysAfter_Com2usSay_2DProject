using System;

[Serializable]
public class TowerData
{
    public EObjectType TowerType;
    public string TypeString;
    public float MaxHp;
    public float Damage;
    public float AtkSpeed;
    public float Range;

    public float PopulationMultiplier = 0.05f;
    public float GetModifiedStat(float baseStat, int population)
    {
        return baseStat * (1 + PopulationMultiplier * (population - 1));
    }
}