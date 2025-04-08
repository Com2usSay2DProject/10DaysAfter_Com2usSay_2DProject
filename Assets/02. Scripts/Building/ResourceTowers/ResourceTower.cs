using Unity.VisualScripting;
using UnityEngine;

public abstract class ResourceTower : TowerRoot
{
    // 자원 회복 량 -> Damage
    // 자원 회복 쿨타임 -> AtkSpeed

    [SerializeField]
    protected ResourceType _resourceType;
    [SerializeField]
    protected GameObject _resourceEffect;

    protected float _timer = 0f;

    protected override void Update()
    {
        base.Update();

        if(!IsBuilt)
        {
            return;
        }

        _timer += Time.deltaTime;

        if(_timer >= _atkSpeed)
        {
            _timer = 0f;
            GenerateResource();
        }
    }

    protected abstract void GenerateResource();
}