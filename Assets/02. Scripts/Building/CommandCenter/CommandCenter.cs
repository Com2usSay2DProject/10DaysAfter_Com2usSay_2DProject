using UnityEngine;

public class CommandCenter : TowerRoot
{
    private float _timer;

    //프리팹에 저장된 포지션 그대로 쓸 것
    private void Start()
    {
        //base.Start();
        Place();
    }

    private void Update()
    {
        _hp = 2000;
        _timer += Time.deltaTime;

        if(_timer > _atkSpeed)
        {
            _timer = 0;
            ResourceManager.Instance.AddResource(ResourceType.Wood, (int)_damage);
        }
    }

    protected override void Die()
    {
        BoundsInt areaToClean = GetGridArea();
        GridBuildingSystem.Instance.ClearArea(areaToClean);

        GameObject explode = EffectPoolManager.Instance.GetObject(EEffectType.BuildingExplode);
        explode.transform.position = transform.position;
        SoundManager.Instance.PlaySfx(ESfxType.BuildingExplode);

        //TODO: 배드엔딩 로직
        //EncounterManager.Instance.BadEnd02();

        Destroy(gameObject);
    }
}
