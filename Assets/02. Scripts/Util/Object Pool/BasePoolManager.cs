using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class BasePoolManager<T, U> : MonoBehaviour
    where T : Enum
    where U : BasePoolInfo<T>
{
    [SerializeField] protected List<U> _poolInfoList;

    protected virtual void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        foreach (U info in _poolInfoList)
        {
            for (int i = 0; i < info.InitCount; i++)
            {
                info.PoolQueue.Enqueue(CreateNewObject(info));
            }
        }
    }

    private GameObject CreateNewObject(U info)
    {
        GameObject newObject = Instantiate(info.Prefab, info.Container.transform);
        newObject.SetActive(false);
        return newObject;
    }

    private U GetPoolByType(T type)
    {
        foreach (U info in _poolInfoList)
        {
            if (type.Equals(info.Type))
            {
                return info;
            }
        }
        return null;
    }

    public GameObject GetObject(T type)
    {
        U info = GetPoolByType(type);
        if (info == null) return null;

        GameObject obj;
        if (info.PoolQueue.Count > 0)
        {
            obj = info.PoolQueue.Dequeue();
        }
        else
        {
            obj = CreateNewObject(info);
        }
        obj.SetActive(true);
        return obj;
    }

    public void ReturnObject(GameObject obj, T type)
    {
        U info = GetPoolByType(type);
        if (info == null) return;

        info.PoolQueue.Enqueue(obj);
        obj.SetActive(false);
    }
}
