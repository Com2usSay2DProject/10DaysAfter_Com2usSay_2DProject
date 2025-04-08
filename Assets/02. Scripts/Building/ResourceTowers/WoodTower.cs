using UnityEngine;

public class WoodTower : ResourceTower
{
    protected override void GenerateResource()
    {
        ResourceManager.Instance.AddResource(_resourceType, (int)_damage);
    }
}