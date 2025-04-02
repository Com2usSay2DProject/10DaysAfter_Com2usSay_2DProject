using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
	private Dictionary<ResourceType, int> _resources = new();

	private void Awake()
	{
		//자원 데이터 불러옴.
		InitResources();
		LoadResourceData();
		DontDestroyOnLoad(gameObject);
	}

	private void InitResources()
	{
		foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
		{
			if (!_resources.ContainsKey(type))
				_resources[type] = 0;
		}
	}

	//AddResource : 자원 타입에 양만큼 추가
	public void AddResource(ResourceType type, int amount)
	{
		//존재하지 않을 경우 항목 추가
		if (!_resources.ContainsKey(type)) _resources[type] = 0;

		_resources[type] += amount;
		SaveResourceData();
	}

	//TryUseResource : 자원 사용할 양이 충분한지 확인, 충분할 경우 사용 및 true 반환
	//자원이 부족할 경우 false 반환
	public bool TryUseResource(ResourceType type, int amount)
	{
		if (_resources.TryGetValue(type, out int value) && value >= amount)
		{
			UseResource(type, amount);
			SaveResourceData();
			return true;
		}
		else
		{
			Debug.LogWarning($"Not enough {type} to use.");
			return false;
		}
	}

	private void UseResource(ResourceType type, int amount)
	{
		_resources[type] -= amount;
	}
	
	//현재 데이터가 업데이트 될 때마다 저장(자동)
	private void SaveResourceData()
	{
		ResourceData data = new ResourceData();

		foreach (var pair in _resources)
		{
			data.entries.Add(new ResourceEntry { type = pair.Key, amount = pair.Value });
		}

		string json = JsonDataManager<ResourceData>.ToJson(data);
		PlayerPrefs.SetString("ResourceData", json);
	}

	private void LoadResourceData()
	{
		if (PlayerPrefs.HasKey("ResourceData"))
		{
			string json = PlayerPrefs.GetString("ResourceData");
			ResourceData data = JsonDataManager<ResourceData>.FromJson(json);

			_resources.Clear();
			foreach (var entry in data.entries)
			{
				_resources[entry.type] = entry.amount;
			}
		}
	}

	//자원 양이 얼마나 있는지 반환
	public int GetResourceAmount(ResourceType type)
	{
		return _resources.TryGetValue(type, out int value) ? value : 0;
	}
}
