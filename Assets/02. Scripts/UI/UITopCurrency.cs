using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UITopCurrency : MonoBehaviour
{
    public List<TextMeshProUGUI> Resources;
    public TextMeshProUGUI DayNightText;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI DayText;
    public float DisPlayInterval = 0.5f;
    public float _CurrentDuration;
    public Sprite SunSprite;
    public Sprite NightSprite;
    public Image CurrentDayImage;
    public GameObject TopCanvas;
    public Transform StartPosition;

    private float GameHours = 12f; // 게임 내에서 표현되는 시간
    private float conversionFactor; // 변환 비율
    private float inGameHours;
    
    
    private Dictionary<ResourceType, float> _currentDisplayValues = new Dictionary<ResourceType, float>();
    [SerializeField] private float _resourceAnimationDuration = 0.5f;

    private void Start()
    {
        SoundManager.Instance.PlayBgm(EBgmType.Game);

        Vector3 originalPosition = TopCanvas.transform.position;
        TopCanvas.transform.position = StartPosition.position;
        TopCanvas.transform.DOMove(originalPosition, 2f).SetEase(Ease.InOutCubic).SetDelay(2f);

        InitializeCurrentValues();
        DisplayTopResources();
        ResourceManager.Instance.OnReourceChange += DisplayTopResources;
        StartCoroutine("DisplayTime");
    }

    private void InitializeCurrentValues()
    {
        var resourceTypes = (ResourceType[])System.Enum.GetValues(typeof(ResourceType));
        foreach (var type in resourceTypes)
        {
            _currentDisplayValues[type] = ResourceManager.Instance.GetResourceAmount(type);
        }
    }

    public void SetTimeText()
    {
        TimeText.text = $"다음 {(PhaseManager.Instance.isNight ? "낮" : "밤")} 까지 : {PhaseManager.Instance.TimeUntilNextPhase.ToString("F0")} 초";
    }

    IEnumerator DisplayTime()
    {
        while (true)
        {
            if (PhaseManager.Instance.isNight)
            {
                DayNightText.text = "밤";
                CurrentDayImage.sprite = NightSprite;
                //conversionFactor = GameHours / (PhaseManager.Instance.NightPhaseDuration / 3600f);
                //_CurrentDuration = PhaseManager.Instance.NightPhaseDuration;
            }
            else
            {
                DayNightText.text = "낮";
                CurrentDayImage.sprite = SunSprite;
                //conversionFactor = GameHours / (PhaseManager.Instance.DayPhaseDuration / 3600f);
                //_CurrentDuration = PhaseManager.Instance.DayPhaseDuration;
            }
            //inGameHours = Mathf.Clamp(PhaseManager.Instance.TimeUntilNextPhase, 0f, _CurrentDuration) * conversionFactor / 3600f;

            //TimeText.text = $"{Mathf.FloorToInt(inGameHours)}";
            SetTimeText();
            DayText.text = $"{PhaseManager.Instance.CurrentDay.ToString()}일째";
            yield return new WaitForSeconds(DisPlayInterval);
        }
    }


    public void DisplayTopResources()
    {
        var resourceTypes = (ResourceType[])System.Enum.GetValues(typeof(ResourceType));

        for (int i = 0; i < Resources.Count && i < resourceTypes.Length; i++)
        {
            ResourceType type = resourceTypes[i];
            float targetValue = ResourceManager.Instance.GetResourceAmount(type);
            float currentValue = _currentDisplayValues[type];

            int index = i; // 클로저를 위한 로컬 변수
            DOTween.To(
                () => currentValue,
                (value) =>
                {
                    _currentDisplayValues[type] = value;
                    if (Resources[index] != null)
                    {
                        Resources[index].text = Mathf.Floor(value).ToString();
                    }
                },
                targetValue,
                _resourceAnimationDuration
            )
            .SetEase(Ease.OutQuad)
            .SetId(type.ToString()); // 각 자원 타입별로 고유한 ID 설정
        }
    }
}
