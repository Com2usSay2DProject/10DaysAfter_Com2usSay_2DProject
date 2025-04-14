using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.ComponentModel;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIButton: UI_TouchBounce
{
    private Button button;
    [SerializeField]
    private ETowerType AllocatedTower;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnExitConfirmed()
    {
        // 게임 종료
        Application.Quit();

        // 에디터 상에서는 종료되지 않으므로 아래 코드 참고
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


    public void OnClickDisplayResource(string resource)
    {

        if(Enum.TryParse(resource, true, out ResourceType resourceType))
        {
            ResourceManager.Instance.AddResource(resourceType, 300);
        }
    }

    public void OnClickBuildMode()
    {
        UIManager.Instance.ToggleBuildMode(button, AllocatedTower);
    }

    public void OnClickUpgredeTowerUI()
    {
        UIManager.Instance.ShowUI("TowerUpgrade");
    }

    public void OnClickSellTowerUI()
    {
        UIManager.Instance.ShowUI("TowerSela");
    }

    public void OnClicPopupUI(GameObject popupUI)
    {
        popupUI.SetActive(true);
        popupUI.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).OnComplete(() => transform.localScale = Vector3.one);
    }

    public void OnClickCloseUI(GameObject PopupUI)
    {
        PopupUI.transform.DOScale(Vector3.one * 0.5f, 0.3f).SetEase(Ease.InBack).OnComplete(() => PopupUI.SetActive(false));
    }

    public void OnClickLoadScene(String name)
    {
        SceneManager.LoadScene(name);
        SoundManager.Instance.StopBgm();
        
    }
}
