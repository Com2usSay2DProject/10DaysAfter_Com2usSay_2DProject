using JetBrains.Annotations;
using System;
using System.ComponentModel;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public void OnClickDisplayResource(string resource)
    {

        if(Enum.TryParse(resource, true, out ResourceType resourceType))
        {
            ResourceManager.Instance.AddResource(resourceType, 300);
        }

        //var input = Enum.Parse(ResourceType, resorceType, true);
        //UIManager.Instance.DisplayTopResources();
        //ResourceManager.Instance.AddResource(ResourceType.Wood, 300);
        //UIManager.Instance.DisplayTopResources(ResourceType.Wood, 300);
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
    }

    public void OnClickCloseUI(GameObject PopupUI)
    {
        PopupUI.SetActive(false);
    }

    public void OnClickLoadScene(String name)
    {
        SceneManager.LoadScene(name);
    }
}
