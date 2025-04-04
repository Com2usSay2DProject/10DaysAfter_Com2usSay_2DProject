using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class EventManager : Singleton<EventManager>
{
	// 이벤트 데이터 전체 보유
	public List<GameEvent> GameEvents;
	private GameEvent _pendingEvent;

	private void Awake()
	{
		Initialize_DontDestroyOnLoad();
		// 이벤트 리스트를 받아옴
		LoadAllEvents();
	}

	// 테스트용으로 사용
	private void Start()
	{
		ForceTriggerEvent("EVT_001");
	}

	private void LoadAllEvents()
	{
		string path = Application.dataPath + "/Resources/Json/Events";
		if (!Directory.Exists(path)) return;

		string[] files = Directory.GetFiles(path, "*.json");
		foreach (string file in files)
		{
			string rawJson = File.ReadAllText(file);
			GameEvent e = JsonDataManager.FromJson<GameEvent>(rawJson);
			if (e != null)
			{
				GameEvents.Add(e);
			}
		}
	}

	// 페이즈 전환 시 이벤트 발생 or 미발생 (조건 중 랜덤으로 골라옴)
	public void TriggerEvent(int currentDay)
	{
		List<GameEvent> validEvents = new();

		foreach (var e in GameEvents)
		{
			if (e.Condition.triggerDays.Contains(currentDay) && !e.Condition.specialConditionRequired)
			{
				validEvents.Add(e);
			}
		}

		if (validEvents.Count == 0)
		{
			Debug.Log("[EventManager] 해당 조건에 맞는 이벤트 없음.");
			return;
		}

		_pendingEvent = validEvents[Random.Range(0, validEvents.Count)];
		UIEventTab.Instance.ShowTab(_pendingEvent.Title);
	}

	// 강제로 이벤트 실행시킬 때 사용(eventID 필요)
	public void ForceTriggerEvent(string eventId)
	{
		GameEvent found = GameEvents.Find(e => e.EventId == eventId);
		if (found != null)
		{
			_pendingEvent = found;
			UIEventTab.Instance.ShowTab(found.Title);
		}
		else
		{
			Debug.LogWarning($"[EventManager] Event ID '{eventId}'를 찾을 수 없습니다.");
		}
	}

	public void OpenEventPlayer()
	{
		if (_pendingEvent == null) return;
		if (_pendingEvent == null) return;
		UIEventPlayer.Instance.Show(_pendingEvent);
	}

	public void ResolveEvent(EventChoice choice)
	{
		foreach (var effect in choice.effects)
		{
			if (effect.amount < 0)
				ResourceManager.Instance.TryUseResource(effect.resourceType, -effect.amount);
			else
				ResourceManager.Instance.AddResource(effect.resourceType, effect.amount);
		}

		//특별효과 (스토리?) 미구현
		/*if (choice.specialEffects != null)
		{
			foreach (var special in choice.specialEffects)
			{
				if (special.hasBranched)
					EventBranches.Add(special.storyBranch);
				else
					EventBranches.Remove(special.storyBranch);
			}
		}*/

		_pendingEvent = null;
		UIEventTab.Instance.HideTab();
	}
}
