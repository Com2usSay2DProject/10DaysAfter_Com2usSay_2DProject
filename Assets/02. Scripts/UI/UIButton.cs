using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class UIButton: Singleton<UIButton>
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnClickBuildMode()
    {
        UIManager.Instance.ToggleBuildMode(button);
    }

    public void OnClickUpgredeTowerUI()
    {
        UIManager.Instance.ShowUI("TowerUpgrade");
    }

    public void OnClickSellTowerUI()
    {
        UIManager.Instance.ShowUI("TowerSela");
    }

    public void OnClickCloseUI(GameObject UI)
    {
        UI.SetActive(false);
    }
}
