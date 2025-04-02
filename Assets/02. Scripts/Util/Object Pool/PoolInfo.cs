using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class PoolInfo
{
    public EObjectType Type;
    public int InitCount;
    public GameObject Prefab;
    public GameObject Container;

    public Queue<GameObject> PoolQueue = new Queue<GameObject>();
}