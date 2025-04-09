using UnityEngine;

public class EffectPoolManager : BasePoolManager<EEffectType, EffectPoolInfo>
{
    private static EffectPoolManager _instance;
    public static EffectPoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<EffectPoolManager>();
            }
            return _instance;
        }
    }
}