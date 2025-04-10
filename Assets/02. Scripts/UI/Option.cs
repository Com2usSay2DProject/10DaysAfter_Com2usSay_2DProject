using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{

    public Slider MusicSlider;
    public Slider EffectSoundSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MusicSlider.value = SoundManager.Instance.GetVolume(SoundManager.AudioType.BGM);
        EffectSoundSlider.value = SoundManager.Instance.GetVolume(SoundManager.AudioType.SFX);
    }

    // Update is called once per frame
    void Update()
    {

            SoundManager.Instance.OnChangedBGMVolume(MusicSlider.value);
            SoundManager.Instance.OnChangedBGMVolume(EffectSoundSlider.value);
        
    }
}
