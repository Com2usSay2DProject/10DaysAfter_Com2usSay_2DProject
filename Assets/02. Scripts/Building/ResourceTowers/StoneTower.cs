using UnityEngine;

public class StoneTower : ResourceTower
{
    protected override void GenerateResource()
    {
        ResourceManager.Instance.AddResource(_resourceType, (int)_damage);
    }
}
