using UnityEngine;


public enum TargetType
{
    MainTower,
    Tower
}
public class EnemyTargetSelector : MonoBehaviour
{
    public Transform FindTarget(TargetType type)
    {
        string tag = GetTagFromTargetType(type);
        GameObject[] candidates = GameObject.FindGameObjectsWithTag(tag);

        if (candidates.Length == 0)
            return null;

        Transform closest = candidates[0].transform;
        float minDist = Vector2.Distance(transform.position, closest.position);

        foreach (GameObject obj in candidates)
        {
            float dist = Vector2.Distance(transform.position, obj.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = obj.transform;
            }
        }

        return closest;
    }

    private string GetTagFromTargetType(TargetType type)
    {
        switch (type)
        {
            case TargetType.MainTower: return "MainTower";
            case TargetType.Tower: return "Tower";
            default:
                {
                    Debug.LogWarning("Wrong Tag");
                    return "MainTower";
                }
        }
    }
}
