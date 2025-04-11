using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ResourceIconSO ResourceIcons;

    [Header("# UI Elements")]
    public TextMeshProUGUI NeedResource1;
    public TextMeshProUGUI NeedResource2;
    public GameObject SlotItem;

    [Header("# Tower Settings")]
    [SerializeField]
    private ETowerType SelectBuildType;

    void Start()
    {
        UpdateResourceTexts();
    }

    private void UpdateResourceTexts()
    {
        var costData = TowerDataManager.Instance.GetTowerCost(SelectBuildType);
        if (costData == null || costData.Count == 0) return;

        var costs = new List<KeyValuePair<ResourceType, int>>(costData);
        
        if (costs.Count >= 1 && NeedResource1 != null)
        {
            NeedResource1.transform.GetChild(0).GetComponent<Image>().sprite = SetIcon(costs[0].Key);
            NeedResource1.text = costs[0].Value.ToString();
        }
        
        if (costs.Count >= 2 && NeedResource2 != null)
        {
            NeedResource2.transform.GetChild(0).GetComponent<Image>().sprite = SetIcon(costs[1].Key);
            NeedResource2.text = costs[1].Value.ToString();
        }
        else
        {
            NeedResource2.gameObject.SetActive(false);
        }
    }

    private Sprite SetIcon(ResourceType type)
    {
        return ResourceIcons.GetIcon(type);
    }

    // 마우스 커서가 슬롯에 들어갈 때 발동
    public void OnPointerEnter(PointerEventData eventData)
    {
        SlotItem.SetActive(true);
    }

    // 마우스 커서가 슬롯에서 나올 때 발동
    public void OnPointerExit(PointerEventData eventData)
    {
        SlotItem.SetActive(false);
    }
}