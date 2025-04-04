using UnityEngine;
using System.Collections.Generic;

public class TowerRoot : MonoBehaviour
{
    private static Dictionary<EObjectType, TowerData> _towerDataDict; // 모든 타워가 공유할 데이터

    [Header("# Stats")]
    public EObjectType TowerType; // 타워의 타입 -> 프리팹에서 설정해두면 데이터 찾아옴
    protected TowerData Data; // 해당 타워의 데이터

    [SerializeField]
    private GameObject UpgradeUI;

    private void Awake()
    {
        GetData();
        GetDataForThis();
    }

    private void GetData()
    {
        // JSON 데이터가 아직 로드되지 않았다면 불러오기
        if (_towerDataDict == null)
        {
            //LoadTowerData();
            TowerDataCollection collection =
                JsonDataManager.LoadFromFile<TowerDataCollection>("Tower/TowerDataCollection");

            _towerDataDict = new Dictionary<EObjectType, TowerData>();

            foreach (TowerData d in collection.Datas)
            {
                d.TypeString = d.TowerType.ToString();
                _towerDataDict[d.TowerType] = d;
            }

            Debug.Log("타워 데이터 로드 완료");
        }
    }

    private void GetDataForThis()
    {
        // 내 타워 타입에 맞는 데이터 찾기
        if (_towerDataDict.TryGetValue(TowerType, out TowerData data))
        {
            Data = new TowerData();
            Data = data;
            //Debug.Log(Data.TypeString);
        }
        else
        {
            Debug.LogError($"타워 데이터 없음: {TowerType}");
        }
    }

    public void TowerClick()
    {
        // 현재 클릭된 타워 제외 나머지 타워 클릭 UI 비활성화
        TileClickManager.Instance.TowerClick += () => UpgradeUI.SetActive(false);
        UpgradeUI.SetActive(true);
    }
}