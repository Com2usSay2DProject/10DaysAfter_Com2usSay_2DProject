using System.Collections.Generic;
using UnityEngine;

public class TowerPoolManager : BasePoolManager<ETowerType, TowerPoolInfo>
{
    private static TowerPoolManager _instance;
    public static TowerPoolManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindAnyObjectByType<TowerPoolManager>();
            }
            return _instance;
        }
    }
}