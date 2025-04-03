using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour //Singleton<UIManager>
{
    public static UIManager Instance;

    public List<UIPage> UIPages = new List<UIPage>();

    private UIPage CurrentPage;

    private Button CrrentButton;

    public bool isBuildModeActive = false; // 빌드 모드 활성화 여부

    private void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        foreach(UIPage page in UIPages)
        {
            page.gameObject.SetActive(false);
        }
    }

    public void ToggleBuildMode(Button button)
    {
        Debug.Log("Build Mode: " + (isBuildModeActive ? "빌드모드 활성화" : "빌드 모드 비활성"));
        
        isBuildModeActive = !isBuildModeActive; // 상태 전환
        CrrentButton = button;
        CrrentButton.interactable = !isBuildModeActive;
    }

    public void ToggleBuildModeOff()
    {
        isBuildModeActive = false;
        CrrentButton.interactable = !isBuildModeActive; ;
    }


    public void ShowUI(string name)
    {
        foreach(UIPage page in UIPages)
        {
           if( page.name == name)
            {
                if (CurrentPage != null)
                {
                    CurrentPage.Hide();
                }
                //페이지 안에 있는 재화를 불러서 넣어 준다.
                page.Show();
                CurrentPage = page;
                return;
            }
        }
        Debug.Log("리턴할 오브젝트가 존재 하지 않습니다.");
    }
}
