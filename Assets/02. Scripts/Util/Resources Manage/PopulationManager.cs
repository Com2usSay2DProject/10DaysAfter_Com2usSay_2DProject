using UnityEngine;

public class PopulationManager : Singleton<PopulationManager>
{
    [SerializeField] private float foodPerPersonPerSecond = 0.05f;
    [SerializeField] private float tickInterval = 1f;
    private float _tickTimer;

    private void Update()
    {
        _tickTimer += Time.deltaTime;
        if (_tickTimer >= tickInterval)
        {
            _tickTimer = 0f;
            HandleResourceCycle();
        }
    }

    private void HandleResourceCycle()
    {
        int population = ResourceManager.Instance.GetResourceAmount(ResourceType.Population);
        int foodConsumption = Mathf.CeilToInt(population * foodPerPersonPerSecond);

        if (ResourceManager.Instance.GetResourceAmount(ResourceType.Food) >= foodConsumption)
        {
            ResourceManager.Instance.AddResource(ResourceType.Food, -foodConsumption);
        }
        else
        {
            int food = ResourceManager.Instance.GetResourceAmount(ResourceType.Food);
            int deficit = foodConsumption - food;
            int popDecrease = Mathf.CeilToInt(deficit * 0.5f); // 식량 부족 대비 감소 비율

            ResourceManager.Instance.SetResource(ResourceType.Food, 0);
            ResourceManager.Instance.AddResource(ResourceType.Population, -popDecrease);

            Debug.Log($"식량 부족! 인구 {popDecrease} 감소");
        }
    }
}