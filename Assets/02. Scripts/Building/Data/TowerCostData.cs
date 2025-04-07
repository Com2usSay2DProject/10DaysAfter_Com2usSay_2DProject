using System;

[Serializable]
public class TowerCostData
{
    public ResourceType Type;
    public int Amount;

    public TowerCostData(ResourceType type, int amount)
    {
        Type = type;
        Amount = amount;
    }
}