using UnityEngine;

public class MetalTower : ResourceTower
{
    protected override void GenerateResource()
    {
        ResourceManager.Instance.AddResource(_resourceType, (int)_damage);
    }
}
