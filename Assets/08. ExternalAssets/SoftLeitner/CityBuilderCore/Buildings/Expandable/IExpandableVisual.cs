using System;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// common interface for visuals that change depending on an <see cref="ExpandableBuilding"/>s expansion size
    /// </summary>
    public interface IExpandableVisual
    {
        event Action VisualsUpdated;

        GameObject GameObject { get; }

        Transform GetPart(int i);
        void UpdateVisuals(Vector2Int expansion);
    }
}
