using System;
using System.Collections.Generic;

[Serializable]
public class GameEvent
{
	public string EventId;
	public string Title;

	public EventCondition Condition;

	public List<EventPage> Pages;
	
}

[Serializable]
public class EventPage
{
	public string text;                      // 페이지에 표시될 텍스트
	public string imagePath;                 // 페이지에 표시될 이미지 경로 (비워두면 없음)

	public List<EventChoice> Choices;       // 선택지가 있을 경우
}

[Serializable]
public class EventChoice
{
	public string text;                     // 선택지 텍스트
	public List<EventEffect> effects;       // 선택했을 때 발생하는 효과들
}

[Serializable]
public class EventEffect
{
	public ResourceType resourceType;
	public int amount;                      // 양 (+면 획득, -면 소모)
}

[Serializable]
public class EventCondition
{
	public List<int> triggerDays;           // 특정 날짜에만 발생 (예: 5일차)
	public bool specialConditionRequired;   // 특수 조건 여부
	// 조건 타입/값으로 확장 (ex: 특정 자원 보유, 특정 건물 존재 등)
}