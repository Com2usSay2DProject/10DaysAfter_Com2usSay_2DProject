using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager> // 문의 : 수민
{
    [SerializeField] private List<PoolInfo> _poolInfoList;

    private void Awake()
    {
        Initialize_DontDestroyOnLoad();
        Initialize();
    }

    private void Initialize()
    {
        foreach (PoolInfo info in _poolInfoList)
        {
            for (int i = 0; i < info.InitCount; i++)
            {
                info.PoolQueue.Enqueue(CreateNewObject(info));
            }
        }
    }

    private GameObject CreateNewObject(PoolInfo info)
    {
        GameObject newObject = Instantiate(info.Prefab, info.Container.transform);
        newObject.SetActive(false);
        return newObject;
    }

    private PoolInfo GetPoolByType(EObjectType type)
    {
        foreach (PoolInfo info in _poolInfoList)
        {
            if (type == info.Type)
            {
                return info;
            }
        }
        return null;
    }

    public GameObject GetObject(EObjectType type)
    {
        PoolInfo info = Instance.GetPoolByType(type);
        GameObject obj = null;
        if (info.PoolQueue.Count > 0)
        {
            obj = info.PoolQueue.Dequeue();
        }
        else
        {
            obj = Instance.CreateNewObject(info);
        }
        obj.SetActive(true);

        return obj;
    }

    public void ReturnObject(GameObject obj, EObjectType type)
    {
        PoolInfo info = Instance.GetPoolByType(type);
        info.PoolQueue.Enqueue(obj);
        obj.SetActive(false);
    }
}