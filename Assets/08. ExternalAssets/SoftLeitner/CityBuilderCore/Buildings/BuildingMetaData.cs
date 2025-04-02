using System;
using UnityEngine;

namespace CityBuilderCore
{
    [Serializable]
    public class BuildingMetaData
    {
        public string Id;
        public string Key;
        public int Index;
        public Vector2Int Point;
        public int Rotation;
        public string Data;
    }
}
