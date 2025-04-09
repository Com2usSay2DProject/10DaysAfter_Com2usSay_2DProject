using UnityEngine;

public class EnemySoundManager : MonoBehaviour
{
    public static EnemySoundManager Instance;

    public AudioSource AudioSource;
    public AudioClip[] audioClip;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }
    private void Update()
    {
    }
    public void SoundPlay(EnemySoundType soundType)
    {
        int index = (int)soundType;

        AudioSource.PlayOneShot(audioClip[index]);
    }
}
