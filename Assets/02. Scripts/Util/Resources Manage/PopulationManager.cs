using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    [SerializeField] private float foodPerPersonPerSecond = 0.05f;
    [SerializeField] private float tickInterval = 1f;
    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= tickInterval)
        {
            _timer = 0f;
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
            int populationDecrease = Mathf.CeilToInt(deficit * 0.5f); // 식량 부족 대비 감소 비율

            ResourceManager.Instance.SetResource(ResourceType.Food, 0);
            ResourceManager.Instance.AddResource(ResourceType.Population, -populationDecrease);
        }
    }
}