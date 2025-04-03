using UnityEngine;
using System.Collections.Generic;
using System.IO;

//Made for testing purposes
public class EventGenerator : MonoBehaviour
{
	private void Start()
	{
		GenerateSampleEvent();
	}
	private void GenerateSampleEvent()
	{
		GameEvent newEvent = new GameEvent
		{
			eventId = "E_TEST_001",
			title = "테스트 이벤트 (직접 저장)",
			condition = new EventCondition
			{
				triggerDays = new List<int> { 2, 4, 6 },
				specialConditionRequired = false
			},
			pages = new List<string>
			{
				"이것은  테스트 이벤트입니다.",
				"선택지를 테스트할 수 있습니다."
			},
			hasChoices = true,
			choices = new List<EventChoice>
			{
				new EventChoice
				{
					text = "식량 3개를 잃는다",
					effects = new List<EventEffect>
					{
						new EventEffect
						{
							resourceType = ResourceType.Food,
							amount = -3
						}
					}
				},
				new EventChoice
				{
					text = "무시한다",
					effects = new List<EventEffect>()
				}
			}
		};

		// 직렬화
		string json = JsonUtility.ToJson(newEvent, true); // true = pretty print

		// 경로 생성
		string dirPath = Application.dataPath + "/Resources/Json/Events";
		if (!Directory.Exists(dirPath))
		{
			Directory.CreateDirectory(dirPath);
		}

		string fullPath = dirPath + "/E_TEST_001_TestEvent.json";
		File.WriteAllText(fullPath, json);

		Debug.Log($"[EventGenerator] JSON 저장 완료: {fullPath}");
	}
}
