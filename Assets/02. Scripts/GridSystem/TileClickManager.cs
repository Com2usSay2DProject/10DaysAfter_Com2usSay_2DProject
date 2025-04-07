using System;
using UnityEngine;
using UniRx;

public class TileClickManager : Singleton<TileClickManager>
{
    public bool IsBuildingMode;

    private TileNode _selectedNode;

    public GameObject SelectedTower;
    private TowerRoot _selectedTower;

    public Action TowerClick;

    private void Start()
    {
        /*this.ObserveEveryValueChanged(_ => SelectedTower)
            .Pairwise() // 이전 값과 현재 값을 함께 가져오기
            .Where(pair => pair.Previous == null && pair.Current != null)
            .Subscribe(_ =>
            {
                _selectedTower = SelectedTower.GetComponent<TowerRoot>();
                // 필요한 로직 실행
            })
            .AddTo(this);*/

        this.ObserveEveryValueChanged(_ => _selectedTower)
            .Pairwise()
            .Where(pair => pair.Previous != null && pair.Current == null)
            .Subscribe(_ =>
            {
                SelectedTower = null;
            })
            .AddTo(this);
    }

    private void Update()
    {
        // 1. 빌드 모드인가?
        // 2. 해당 위치에 건물이 지어져 있는가 -> IsWalkable = true 일 때 통과
        // 둘다 통과하면 건설 -> 빌드 모드 해제
        // else if
        // 1. 빌드 모드가 아니고, 클릭 한 것이 건물 -> 업그레이드 UI
        GetMouseClick();
    }

    private void GetMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D[] hits = Physics2D.OverlapPointAll(mouseWorldPos);

            if (hits.Length == 0)
                return;

            if (UIManager.Instance.isBuildModeActive)
            {
                bool tileClicked = false;
                _selectedTower = SelectedTower.GetComponent<TowerRoot>();
                foreach (var hit in hits)
                {
                    if (hit.gameObject.layer == LayerMask.NameToLayer("Tile") ||
                        hit.CompareTag("Tile"))
                    {
                        if (_selectedTower.CanBuild)
                        {
                            tileClicked = true;
                            Debug.Log("클릭한 위치가 타일임: " + hit.transform.position);
                            _selectedTower.SetPosition();
                            _selectedTower.Isbuilt = true;
                            _selectedTower = null;
                            UIManager.Instance.ToggleBuildModeOff();
                            break;
                        }
                        else
                        {
                            // 건설 불가 사운드
                            break;
                        }
                    }
                }

                if (!tileClicked)
                {
                    Debug.Log("클릭한 위치에 타일이 없음");
                }
            }
            /*else
            {
                if(hit.transform.CompareTag("Tower"))
                {
                    TowerClick?.Invoke();
                    // TODO : 업그레이드 UI 띄우기
                    Debug.Log("타워 업그레이드 UI를 띄워주세요");
                    hit.transform.GetComponent<TowerRoot>().TowerClick();
                }
            }*/
        }
    }
}
