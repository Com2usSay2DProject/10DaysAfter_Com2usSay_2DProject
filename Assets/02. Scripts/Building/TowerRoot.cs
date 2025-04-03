using UnityEngine;
using UnityEngine.EventSystems;

public class TowerRoot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject UpgradeUI;

    public void OnPointerClick(PointerEventData eventData)
    {
        TowerClick();
    }

    public void TowerClick()
    {
        UpgradeUI.SetActive(true);
    }
}
