using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonTest : MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    public void OnClickBuildMode()
    {
        UIManager.Instance.ToggleBuildMode(button);
        //button.interactable = UIManager.Instance.ToggleBuildMode();
    }



}
