using UnityEngine;

public class StoneTower : ResourceTower
{
    protected override void GenerateResource()
    {
        ResourceManager.Instance.AddResource(_resourceType, (int)_damage);

        _resourceTextPopup?.gameObject.SetActive(true);
        _resourceTextPopup?.SetText((int)_damage);
    }
}
