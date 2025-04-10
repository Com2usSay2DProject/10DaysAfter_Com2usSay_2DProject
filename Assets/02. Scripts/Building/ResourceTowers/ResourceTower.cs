using Unity.VisualScripting;
using UnityEngine;

public abstract class ResourceTower : TowerRoot
{
    // 자원 회복 량 -> Damage
    // 자원 회복 쿨타임 -> AtkSpeed

    [Header("# Resources")]
    [SerializeField]
    protected ResourceType _resourceType;
    [SerializeField]
    protected GameObject _resourceEffect;
    [SerializeField]
    protected UINumberPopup _resourceTextPopup;

    protected float _timer = 0f;

    private void Update()
    {
        //base.Update();

        if(!IsPlaced)
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