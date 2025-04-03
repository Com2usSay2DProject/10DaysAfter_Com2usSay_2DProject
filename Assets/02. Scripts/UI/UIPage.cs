using UnityEngine;

public class UIPage : MonoBehaviour
{
    public string PageName;
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
