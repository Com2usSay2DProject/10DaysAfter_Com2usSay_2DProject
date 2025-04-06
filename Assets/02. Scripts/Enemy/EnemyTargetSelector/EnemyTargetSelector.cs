using UnityEngine;


public enum ETargetType
{
    MainTower,
    Tower
}
public class EnemyTargetSelector : MonoBehaviour
{
    public GameObject FindTarget(ETargetType type)
    {
        string tag = GetTagFromTargetType(type);
        GameObject[] candidates = GameObject.FindGameObjectsWithTag(tag);

        if (candidates.Length == 0)
            return null;

        GameObject closest = candidates[0];
        float minDist = (transform.position - closest.transform.position).sqrMagnitude;

        foreach (GameObject obj in candidates)
        {
            float dist = (transform.position - obj.transform.position).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                closest = obj;
            }
        }

        return closest;
    }

    private string GetTagFromTargetType(ETargetType type)
    {
        switch (type)
        {
            case ETargetType.MainTower: return "MainTower";
            case ETargetType.Tower: return "Tower";
            default:
                {
                    Debug.LogWarning("Wrong Tag");
                    return "MainTower";
                }
        }
    }
}
