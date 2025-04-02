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

    protected void Initialize_DontDestroyOnLoad()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject); // 씬이 전환되어도 게임오브젝트를 파괴하지 않는다 
        }
        else
        {
            Destroy(this);
        }
    }
}