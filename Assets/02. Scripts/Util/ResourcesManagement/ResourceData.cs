using System;
using System.Collections.Generic;

public enum ResourceType
{
	Wood,
	Stone,
	Food,
	Metal
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
