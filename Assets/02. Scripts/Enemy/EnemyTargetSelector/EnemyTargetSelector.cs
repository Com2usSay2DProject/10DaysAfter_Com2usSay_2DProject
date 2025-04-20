using UnityEngine;


public enum ETargetType
{
    MainTower,
    Tower
}
public static class EnemyTargetSelector
{
    public static GameObject FindTarget(Vector3 fromPosition, ETargetType type)
    {
        string tag = GetTagFromTargetType(type);
        GameObject[] candidates = GameObject.FindGameObjectsWithTag(tag);

        if (candidates.Length == 0)
            return null;

        GameObject closest = candidates[0];
        float minDist = (fromPosition - closest.transform.position).sqrMagnitude;

        foreach (GameObject obj in candidates)
        {
            float dist = (fromPosition - obj.transform.position).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                closest = obj;
            }
        }

        return closest;
    }

    private static string GetTagFromTargetType(ETargetType type)
    {
        switch (type)
        {
            case ETargetType.MainTower: return "MainTower";
            case ETargetType.Tower: return "Tower";
            default:
                Debug.LogWarning("Wrong Tag");
                return "MainTower";
        }
    }
}
