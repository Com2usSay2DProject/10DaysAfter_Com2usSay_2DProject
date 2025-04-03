using UnityEngine;
using TMPro;

public class UITowerUpgradePopup : UIPage
{
    [SerializeField]
    private TextMeshProUGUI TowerText;
    [SerializeField]
    private TextMeshProUGUI GoldText;
    public void SetTowerData(string name, string gold)
    {
        TowerText.text = name;
        GoldText.text = gold;
    }
}
