using System.Collections.Generic;
using UnityEngine;

public class TowerDataManager : Singleton<TowerDataManager>
{
    public static Dictionary<ETowerType, TowerData> TowerDataDictionary { get; private set; }
    public bool IsInitialized { get; private set; }

    private void Awake()
    {
        LoadTowerData();
        IsInitialized = true;
    }

    private void LoadTowerData()
    {
        if (TowerDataDictionary != null) return;
        TowerDataCollection collection;
#if UNITY_EDITOR
        collection = JsonDataManager.LoadFromFile<TowerDataCollection>("Tower/TowerDataCollection");
#else
        TextAsset jsonText = Resources.Load<TextAsset>("Json/Tower/TowerDataCollection");
        if(jsonText == null)
        {
            Debug.LogError("데이터 파일이 없습니다(빌드 환경)");
            return;
        }
        collection = JsonDataManager.FromJson<TowerDataCollection>(jsonText.text);
#endif
        TowerDataDictionary = new Dictionary<ETowerType, TowerData>();

        foreach (TowerData d in collection.Datas)
        {
            d.TypeString = d.TowerType.ToString();
            TowerDataDictionary[d.TowerType] = d;
        }
    }

    public TowerData GetTowerData(ETowerType towerType)
    {
        if (TowerDataDictionary.TryGetValue(towerType, out TowerData data))
        {
            return data;
        }
        
        Debug.LogError($"타워 데이터 없음: {towerType}");
        return null;
    }

    public Dictionary<ResourceType, int> GetTowerCost(ETowerType towerType)
    {
        TowerData data = GetTowerData(towerType);
        if (data == null) return null;

        Dictionary<ResourceType, int> costDict = new Dictionary<ResourceType, int>();
        foreach (var cost in data.Cost)
        {
            costDict.Add(cost.Type, cost.Amount);
        }
        return costDict;
    }

    public float GetModifiedStat(ETowerType towerType, float baseValue)
    {
        TowerData data = GetTowerData(towerType);
        if (data == null) return baseValue;

        return data.GetModifiedStat(baseValue, ResourceManager.Instance.GetResourceAmount(ResourceType.Population));
    }
}
