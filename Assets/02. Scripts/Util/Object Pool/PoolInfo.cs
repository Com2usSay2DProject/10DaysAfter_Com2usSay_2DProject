using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class PoolInfo // 문의 : 수민
{
    public EObjectType Type;
    public int InitCount;
    public GameObject Prefab;
    public GameObject Container;

    public Queue<GameObject> PoolQueue = new Queue<GameObject>();
}