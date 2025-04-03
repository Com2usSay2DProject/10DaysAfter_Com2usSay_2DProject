using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class UIButton: MonoBehaviour
{
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    public void OnClickBuildMode()
    {
        UIManager.Instance.ToggleBuildMode(button);
    }
}
