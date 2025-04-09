using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaveDataEditor : EditorWindow
{
    private WaveDataCollection collection;
    private Vector2 scrollPos;

    private const string FileName = "Wave/WaveDataCollection";

    [MenuItem("Tools/Wave Data Editor")]
    public static void ShowWindow()
    {
        GetWindow<WaveDataEditor>("Wave Data Editor");
    }

    private void OnEnable()
    {
        try
        {
            collection = JsonDataManager.LoadFromFile<WaveDataCollection>(FileName);
            if (collection.Datas == null)
                collection.Datas = new List<WaveData>();
        }
        catch
        {
            Debug.LogWarning("WaveDataCollection 파일이 없어서 새로 생성합니다.");
            collection = new WaveDataCollection();
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Wave Data Editor", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        for (int i = 0; i < collection.Datas.Count; i++)
        {
            var wave = collection.Datas[i];
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField($"Wave {i + 1}", EditorStyles.boldLabel);

            wave.spawnMinnum = EditorGUILayout.IntField("Spawn Min Num", wave.spawnMinnum);
            wave.spawnMaxnum = EditorGUILayout.IntField("Spawn Max Num", wave.spawnMaxnum);
            wave.minSpawnDelay = EditorGUILayout.FloatField("Min Spawn Delay", wave.minSpawnDelay);
            wave.maxSpawnDelay = EditorGUILayout.FloatField("Max Spawn Delay", wave.maxSpawnDelay);
            wave.enableSpawnType = EditorGUILayout.IntSlider("Enable Spawn Type", wave.enableSpawnType, 0, 4);
            wave.useUnipueEnemy = EditorGUILayout.Toggle("Use Unique Enemy", wave.useUnipueEnemy);
            wave.useBossEnemy = EditorGUILayout.Toggle("Use Boss Enemy", wave.useBossEnemy);

            if (GUILayout.Button("삭제"))
            {
                collection.Datas.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);

        if (GUILayout.Button("새로운 웨이브 추가"))
        {
            collection.Datas.Add(new WaveData());
        }

        if (GUILayout.Button("JSON으로 저장"))
        {
            JsonDataManager.CreateFile(FileName, collection);
            Debug.Log("WaveDataCollection 저장됨");
            AssetDatabase.Refresh();
        }
    }
}
