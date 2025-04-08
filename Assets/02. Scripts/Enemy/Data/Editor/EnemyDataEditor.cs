using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyDataEditor : EditorWindow
{
    private EnemyDataCollection collection = new EnemyDataCollection();

    [MenuItem("Tools/Enemy Data Editor")]
    public static void ShowWindow()
    {
        GetWindow<EnemyDataEditor>("Enemy Data Editor");
    }

    private Vector2 scrollPos;
    private const string FileName = "Enemy/EnemyDataCollection";
    private void OnEnable()
    {
        try
        {
            collection = JsonDataManager.LoadFromFile<EnemyDataCollection>(FileName);
            if (collection.Datas == null)
                collection.Datas = new List<EnemyData>();
        }
        catch
        {
            Debug.LogWarning("기존 데이터가 없어서 새로 생성합니다.");
            collection = new EnemyDataCollection { Datas = new List<EnemyData>() };
        }
    }
    private void OnGUI()
    {
        EditorGUILayout.LabelField("Enemy Data List", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        for (int i = 0; i < collection.Datas.Count; i++)
        {
            var data = collection.Datas[i];
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField($"Enemy {i + 1}", EditorStyles.boldLabel);

            data.TypeString = EditorGUILayout.TextField("Type String", data.TypeString);
            data.EnemyType = (EEnemyType)EditorGUILayout.EnumPopup("Enemy Type", data.EnemyType);
            data.TargetType = (ETargetType)EditorGUILayout.EnumPopup("Target Type", data.TargetType);
            data.MaxHp = EditorGUILayout.FloatField("Max HP", data.MaxHp);
            data.Speed = EditorGUILayout.FloatField("Speed", data.Speed);
            data.AtkSpeed = EditorGUILayout.FloatField("Attack Speed", data.AtkSpeed);
            data.Damage = EditorGUILayout.FloatField("Damage", data.Damage);
            data.Range = EditorGUILayout.FloatField("Range", data.Range);

            if (GUILayout.Button("삭제"))
            {
                collection.Datas.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("새로운 적 추가"))
        {
            collection.Datas.Add(new EnemyData());
        }

        if (GUILayout.Button("JSON으로 저장"))
        {
            JsonDataManager.CreateFile("Enemy/EnemyDataCollection", collection);
            Debug.Log("EnemyDataCollection 저장됨");
        }
    }
}
