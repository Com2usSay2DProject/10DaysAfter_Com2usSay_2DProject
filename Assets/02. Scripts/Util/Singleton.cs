using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component // 문의 : 수민
{
    protected static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>();
            }
            return _instance;
        }
    }
}