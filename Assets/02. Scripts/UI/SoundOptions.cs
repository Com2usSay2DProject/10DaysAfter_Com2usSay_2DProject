using UnityEngine;
using UnityEngine.UI;

public class SoundOptions : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider bgmSlider; // BGM 볼륨 슬라이더
    public Slider sfxSlider; // SFX 볼륨 슬라이더

    private void Start()
    {
        // 초기화: PlayerPrefs에서 저장된 값 불러오기
        float savedBGMVolume = PlayerPrefs.GetFloat("BGM_Volume", 1.0f);
        float savedSFXVolume = PlayerPrefs.GetFloat("SFX_Volume", 1.0f);

        // 슬라이더 초기값 설정
        bgmSlider.value = 1.0f - savedBGMVolume; // 저장된 값은 1.0f - value 형태
        sfxSlider.value = 1.0f - savedSFXVolume;

        // 이벤트 리스너 연결
        bgmSlider.onValueChanged.AddListener(OnBGMSliderChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
    }

    private void OnBGMSliderChanged(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.OnChangedBGMVolume(value); // SoundManager에 값 전달
        }
    }

    private void OnSFXSliderChanged(float value)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.OnVolumeChanged(SoundManager.AudioType.SFX, value); // SFX 볼륨 변경
        }
    }
}