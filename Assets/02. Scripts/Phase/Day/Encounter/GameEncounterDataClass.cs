using System;
using System.Collections.Generic;

[Serializable]
public class GameEncounter
{
	public string EncounterId;
	public string Title;

	public EncounterCondition Condition;

	public List<EncounterPage> Pages;
}

[Serializable]
public class EncounterPage
{
	public string text;						// 페이지에 표시될 텍스트
	public string imagePath;				// 페이지에 표시될 이미지 경로 (비워두면 없음)

	public List<EncounterChoice> Choices;       // 선택지가 있을 경우
	public int nextPageIndex = -1;
}

[Serializable]
public class EncounterChoice
{
	public string text;                     // 선택지 텍스트
	public List<EncounterEffect> effects;       // 선택했을 때 발생하는 효과들

	//특수조건 설정하기
	public string branchKey;				// 분기 조건 이름
	public bool setBranchTrue;

	//선택지로 인한 반응 변경용
	public int nextPageIndex = -1; 
}

[Serializable]
public class EncounterEffect
{
	public ResourceType resourceType;
	public int amount;                      // 양 (+면 획득, -면 소모)
	// 엔딩 조건 및 분기 기능 추가하기
}

[Serializable]
public class SpecialEffect
{
	public string storyBranch;
	public bool hasBranched;
}

[Serializable]
public class EncounterCondition
{
	public List<int> triggerDays;           // 특정 날짜에만 발생 (예: 5일차)
	public string specialConditionRequired;   // 특수 조건 여부
	// 조건 타입/값으로 확장 (ex: 특정 자원 보유, 특정 건물 존재 등)
}