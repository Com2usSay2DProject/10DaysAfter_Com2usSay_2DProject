using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class EventJsonCreator : EditorWindow
{
	private string eventId = "EVT_001";
	private string title = "새로운 이벤트";
	private List<int> triggerDays = new() { 1 };
	private string specialCondition = "";

	private List<EventPage> pages = new();
	private Vector2 scrollPos;

	[MenuItem("Tools/Event JSON Creator")]
	public static void ShowWindow()
	{
		GetWindow<EventJsonCreator>("Event JSON Creator");
	}

	private void OnGUI()
	{
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		GUILayout.Label("이벤트 기본 정보", EditorStyles.boldLabel);
		eventId = EditorGUILayout.TextField("Event ID", eventId);
		title = EditorGUILayout.TextField("Title", title);

		GUILayout.Label("Trigger Days (쉼표 구분)");
		string daysString = EditorGUILayout.TextField(string.Join(",", triggerDays));
		triggerDays = new List<int>();
		foreach (var part in daysString.Split(','))
		{
			if (int.TryParse(part.Trim(), out int result))
				triggerDays.Add(result);
		}

		specialCondition = EditorGUILayout.TextField("Special Condition", specialCondition);

		GUILayout.Space(10);
		GUILayout.Label("페이지 목록", EditorStyles.boldLabel);
		if (GUILayout.Button("페이지 추가"))
			pages.Add(new EventPage());

		for (int i = 0; i < pages.Count; i++)
		{
			GUILayout.BeginVertical("box");
			GUILayout.Label($"페이지 {i + 1}");
			pages[i].text = EditorGUILayout.TextField("텍스트", pages[i].text);
			pages[i].imagePath = EditorGUILayout.TextField("이미지 경로", pages[i].imagePath);

			if (pages[i].Choices == null)
				pages[i].Choices = new();

			if (GUILayout.Button("선택지 추가"))
				pages[i].Choices.Add(new EventChoice());

			for (int j = 0; j < pages[i].Choices.Count; j++)
			{
				GUILayout.BeginHorizontal();
				pages[i].Choices[j].text = EditorGUILayout.TextField("선택지 텍스트", pages[i].Choices[j].text);
				if (GUILayout.Button("삭제"))
					pages[i].Choices.RemoveAt(j);
				GUILayout.EndHorizontal();
			}

			if (GUILayout.Button("페이지 삭제"))
				pages.RemoveAt(i);

			GUILayout.EndVertical();
		}

		GUILayout.Space(20);
		if (GUILayout.Button("JSON 저장"))
			SaveJson();

		EditorGUILayout.EndScrollView();
	}

	private void SaveJson()
	{
		GameEvent gameEvent = new GameEvent
		{
			EventId = eventId,
			Title = title,
			Condition = new EventCondition
			{
				triggerDays = triggerDays,
				specialConditionRequired = specialCondition
			},
			Pages = pages
		};

		string json = JsonUtility.ToJson(gameEvent, true);
		string path = EditorUtility.SaveFilePanel("이벤트 JSON 저장", Application.dataPath, eventId, "json");
		if (!string.IsNullOrEmpty(path))
		{
			File.WriteAllText(path, json);
			Debug.Log($"이벤트 JSON 저장 완료: {path}");
		}
	}
}