using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class UIButton: Singleton<UIButton>
{
    private Button button;
    [SerializeField]
    private ETowerType AllocatedTower;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnClickDisplayResource()
    {
        //ResourceManager.Instance.AddResource(ResourceType.Wood, 300);
        UIManager.Instance.DisplayTopResources(ResourceType.Wood, 300);
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

    public void OnClickCloseUI(GameObject UI)
    {
        UI.SetActive(false);
    }
}
