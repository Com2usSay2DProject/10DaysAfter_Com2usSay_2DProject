using System;
using System.Collections.Generic;

public enum ResourceType
{
	//자유롭게 추가 가능
	Wood,
	Stone,
	Food,
	Metal,
	Population
}

[Serializable]
public class ResourceEntry
{
	public ResourceType type;
	public int amount;
}

//자원entry 데이터를 여러 개 가지는 형식
[Serializable]
public class ResourceData
{
	public List<ResourceEntry> entries = new();
}

public static class ResourceNameTranslator
{
	public static string GetLocalizedName(ResourceType type)
	{
		return type switch
		{
			ResourceType.Wood => "목재",
			ResourceType.Stone => "석재",
			ResourceType.Food => "식량",
			ResourceType.Metal => "금속",
			ResourceType.Population => "인구",
			_ => type.ToString()
		};
	}
}
