using UnityEngine;

public class TitleManager : MonoBehaviour
{
    void Start()
    {
        SoundManager.Instance.PlayBgm(EBgmType.Title);
    }

}
