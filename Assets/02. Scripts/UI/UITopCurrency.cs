using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITopCurrency : MonoBehaviour
{
    public List<TextMeshProUGUI> Resources;
    public TextMeshProUGUI DayNightText;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI DayText;
    public float DisPlayInterval = 0.5f;

    private float GameHours = 12f; // 게임 내에서 표현되는 시간
    private float conversionFactor; // 변환 비율


    private void Start()
    {
        conversionFactor = GameHours / (PhaseManager.Instance.DayPhaseDuration/3600f);
        DisplayTopResources();
        ResourceManager.Instance.OnReourceChange += DisplayTopResources;
        StartCoroutine("DisplayTime");
    }

    void Update()
    {
        //DisplayTopResources();

    }

    public void SetTimeText()
    {
        TimeText.text = PhaseManager.Instance.TimeUntilNextPhase.ToString();
    }


    

    IEnumerator DisplayTime()
    {
        while (true)
        {
            if (PhaseManager.Instance.isNight)
            {
                DayNightText.text = "밤";
            }
            else
            {
                DayNightText.text = "낮";
            }

            float inGameHours = Mathf.Clamp(PhaseManager.Instance.TimeUntilNextPhase, 0f, PhaseManager.Instance.DayPhaseDuration)
                   * conversionFactor / 3600f;
            TimeText.text = $"{Mathf.FloorToInt(inGameHours)}남은시간";
            DayText.text = $"{PhaseManager.Instance.CurrentDay.ToString()}일째";
            yield return new WaitForSeconds(DisPlayInterval);
        }
    }


    public void DisplayTopResources()
    {
        var resourceTypes = (ResourceType[])System.Enum.GetValues(typeof(ResourceType));

        for (int i = 0; i < Resources.Count && i < resourceTypes.Length; i++)
        {
            Resources[i].text = ResourceManager.Instance.GetResourceAmount(resourceTypes[i]).ToString();
        }
    }

}
