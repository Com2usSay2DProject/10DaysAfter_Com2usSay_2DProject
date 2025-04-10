using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class EncounterManager : Singleton<EncounterManager>
{
	// 이벤트 데이터 전체 보유
	public List<GameEncounter> GameEncounters;
	private GameEncounter _pendingEncounter;

	private void Awake()
	{
		Initialize_DontDestroyOnLoad();
		// 이벤트 리스트를 받아옴
		LoadAllEncounters();
	}

	// 테스트용으로 사용
	private void Start()
	{
		
	}

	private void LoadAllEncounters()
	{
		string path = Application.dataPath + "/Resources/Json/Encounters";
		if (!Directory.Exists(path)) return;

		string[] files = Directory.GetFiles(path, "*.json");
		foreach (string file in files)
		{
			string rawJson = File.ReadAllText(file);
			GameEncounter e = JsonDataManager.FromJson<GameEncounter>(rawJson);
			if (e != null)
			{
				GameEncounters.Add(e);
			}
		}
	}

	// 페이즈 전환 시 이벤트 발생 or 미발생 (조건 중 랜덤으로 골라옴)
	public void TriggerEncounter(int currentDay)
	{
		Debug.Log($"triggered encounter day {currentDay}");
		List<GameEncounter> validEncounter = new();

		foreach (var e in GameEncounters)
		{
			if (e.Condition == null) continue;
			bool conditionMet = true;

			if (!string.IsNullOrEmpty(e.Condition.specialConditionRequired))
			{
				conditionMet = StateManager.Instance.GetBranch(e.Condition.specialConditionRequired);
			}

			if (e.Condition.triggerDays.Contains(currentDay) && conditionMet)
			{
				validEncounter.Add(e);
			}
		}

		if (validEncounter.Count == 0)
		{
			Debug.Log("[EncounterManager] 해당 조건에 맞는 이벤트 없음.");
			return;
		}

		_pendingEncounter = validEncounter[Random.Range(0, validEncounter.Count)];
		UIEncounterTab.Instance.ShowTab(_pendingEncounter.Title);
	}

	// 페이즈 전환 시 스토리 무조건 발생(날짜에 따라)
	public void TriggerStory(int currentDay)
	{
		string storyId = "";

		switch (currentDay)
		{
			case 1:
				storyId = PhaseManager.Instance.isNight ? "T_EVT_002" : "T_EVT_001";
				break;
			case 2:
				if (!PhaseManager.Instance.isNight)storyId = "T_EVT_003";
				break;
			case 5:
				if (PhaseManager.Instance.isNight) storyId = "T_EVT_004";
				break;
			case 8:
				if (!PhaseManager.Instance.isNight) storyId = "T_EVT_005";
				break;
			case 9:
				if (!PhaseManager.Instance.isNight) storyId = "T_EVT_006";
				break;
			case 10:
				break;
			default:
				break;
		}

		if (!string.IsNullOrEmpty(storyId))
		{
			GameEncounter found = GameEncounters.Find(e => e.EncounterId == storyId);
			if (found != null)
			{
				UIEncounterPlayer.Instance.Show(found, () =>
				{
					if (!PhaseManager.Instance.isNight)
					{
						TriggerEncounter(currentDay);
					}
				});
			}
			else
			{
				if (!PhaseManager.Instance.isNight)
				{
					TriggerEncounter(currentDay); // 스토리 없으면 일반 바로
				}
			}
		}
		else
		{
			if (!PhaseManager.Instance.isNight)
			{
				TriggerEncounter(currentDay); // 스토리 없으면 일반 바로
			}
		}
	}

	// 강제로 이벤트 실행시킬 때 사용(encounterID 필요)
	public void ForceTriggerEncounter(string encounterId)
	{
		GameEncounter found = GameEncounters.Find(e => e.EncounterId == encounterId);
		if (found != null)
		{
			_pendingEncounter = found;
			UIEncounterTab.Instance.ShowTab(found.Title);
		}
		else
		{
			Debug.LogWarning($"[EncounterManager] Encounter ID '{encounterId}'를 찾을 수 없습니다.");
		}
	}

	/*public void StoryEncounter(string encounterId)
	{
		GameEncounter found = GameEncounters.Find(e => e.EncounterId == encounterId);
		if (found != null)
		{
			UIEncounterPlayer.Instance.Show(found);
		}
	}*/

	public void OpenEncounterPlayer()
	{
		if (_pendingEncounter == null) return;
		UIEncounterPlayer.Instance.Show(_pendingEncounter);
	}

	public void ResolveEncounter(EncounterChoice choice)
	{
		if(choice != null)
		{
			foreach (var effect in choice.effects)
			{
				if (effect.amount < 0) ResourceManager.Instance.TryUseResource(effect.resourceType, -effect.amount);
				else ResourceManager.Instance.AddResource(effect.resourceType, effect.amount);
			}

			//특별효과 (스토리?)
			if (!string.IsNullOrEmpty(choice.branchKey))
			{
				StateManager.Instance.SetBranch(choice.branchKey);
			}
		}

		_pendingEncounter = null;
		UIEncounterTab.Instance.HideTab();
	}

	public void IgnoreEncounter()
	{
		//페이즈 끝날 때까지 무시했을 경우
		_pendingEncounter = null;
		UIEncounterTab.Instance.HideTab();
	}

	public void SkipStory()
	{

	}
}
