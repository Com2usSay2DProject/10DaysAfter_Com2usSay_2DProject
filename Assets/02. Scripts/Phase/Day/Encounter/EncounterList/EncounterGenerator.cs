using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class EncounterJsonCreator : EditorWindow
{
	private string encounterId = "EVT_001";
	private string Title = "새로운 이벤트";
	private List<int> triggerDays = new() { 1 };
	private string specialCondition = "";

	private List<EncounterPage> pages = new();
	private Vector2 scrollPos;

	[MenuItem("Tools/Encounter JSON Creator")]
	public static void ShowWindow()
	{
		GetWindow<EncounterJsonCreator>("Encounter JSON Creator");
	}

	private void OnGUI()
	{
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		GUILayout.Label("이벤트 기본 정보", EditorStyles.boldLabel);
		encounterId = EditorGUILayout.TextField("Encounter ID", encounterId);
		Title = EditorGUILayout.TextField("Title", Title);

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
			pages.Add(new EncounterPage());

		for (int i = 0; i < pages.Count; i++)
		{
			GUILayout.BeginVertical("box");
			GUILayout.Label($"페이지 {i + 1}");
			pages[i].text = EditorGUILayout.TextField("텍스트", pages[i].text);
			pages[i].imagePath = EditorGUILayout.TextField("이미지 경로", pages[i].imagePath);

			if (pages[i].Choices == null)
				pages[i].Choices = new();

			if (GUILayout.Button("선택지 추가"))
				pages[i].Choices.Add(new EncounterChoice());

			for (int j = 0; j < pages[i].Choices.Count; j++)
			{
				GUILayout.BeginVertical("box");

				pages[i].Choices[j].text = EditorGUILayout.TextField("선택지 텍스트", pages[i].Choices[j].text);
				pages[i].Choices[j].branchKey = EditorGUILayout.TextField("분기 키 (branchKey)", pages[i].Choices[j].branchKey);
				pages[i].Choices[j].setBranchTrue = EditorGUILayout.Toggle("분기 활성화 여부", pages[i].Choices[j].setBranchTrue);

				if (pages[i].Choices[j].effects == null)
					pages[i].Choices[j].effects = new();

				GUILayout.Space(4);
				GUILayout.Label("효과 목록", EditorStyles.miniBoldLabel);

				if (GUILayout.Button("효과 추가"))
					pages[i].Choices[j].effects.Add(new EncounterEffect());

				for (int k = 0; k < pages[i].Choices[j].effects.Count; k++)
				{
					GUILayout.BeginHorizontal();
					pages[i].Choices[j].effects[k].resourceType = (ResourceType)EditorGUILayout.EnumPopup("자원 타입", pages[i].Choices[j].effects[k].resourceType);
					pages[i].Choices[j].effects[k].amount = EditorGUILayout.IntField("양", pages[i].Choices[j].effects[k].amount);
					if (GUILayout.Button("삭제", GUILayout.Width(50)))
						pages[i].Choices[j].effects.RemoveAt(k);
					GUILayout.EndHorizontal();
				}

				GUILayout.Space(4);
				if (GUILayout.Button("선택지 삭제"))
					pages[i].Choices.RemoveAt(j);

				GUILayout.EndVertical();
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
		GameEncounter gameencounter = new GameEncounter
		{
			EncounterId = encounterId,
			Title = Title,
			Condition = new EncounterCondition
			{
				triggerDays = triggerDays,
				specialConditionRequired = specialCondition
			},
			Pages = pages
		};

		string json = JsonUtility.ToJson(gameencounter, true);
		string path = EditorUtility.SaveFilePanel("이벤트 JSON 저장", Application.dataPath, encounterId, "json");
		if (!string.IsNullOrEmpty(path))
		{
			File.WriteAllText(path, json);
			Debug.Log($"이벤트 JSON 저장 완료: {path}");
		}
	}
}