using UnityEngine.EventSystems;
using UnityEngine;
using Unity.Android.Gradle;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // ...
    public TextMeshProUGUI NeedResource1;
    public TextMeshProUGUI NeedResource2;
    [SerializeField]
    private ETowerType SellectBuildType;
    public GameObject SlotItem;

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