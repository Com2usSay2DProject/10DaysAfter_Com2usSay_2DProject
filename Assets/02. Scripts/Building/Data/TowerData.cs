using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TowerData
{
    public ETowerType TowerType;
    public string TypeString;
    public float MaxHp;
    public float Damage;
    public float AtkSpeed;
    public float Range;
    public float BuildTime;
    public List<TowerCostData> Cost;

    public float PopulationMultiplier = 0.03f;
    public float GetModifiedStat(float baseStat, int population)
    {
        return baseStat * (1 + PopulationMultiplier * Mathf.Pow(population, 0.5f));
    }
}