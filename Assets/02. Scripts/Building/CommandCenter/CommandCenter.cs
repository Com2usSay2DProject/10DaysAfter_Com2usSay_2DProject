using UnityEngine;

public class CommandCenter : TowerRoot
{
    //프리팹에 저장된 포지션 그대로 쓸 것
    void Start()
    {
        base.Start();
        Place();
    }

    protected override void Die()
    {
        BoundsInt areaToClean = GetGridArea();
        GridBuildingSystem.Instance.ClearArea(areaToClean);

        GameObject explode = EffectPoolManager.Instance.GetObject(EEffectType.BuildingExplode);
        explode.transform.position = transform.position;
        SoundManager.Instance.PlaySfx(ESfxType.BuildingExplode);

        //TODO: 배드엔딩 로직
        EncounterManager.Instance.BadEnd02();

        Destroy(gameObject);
    }
}
