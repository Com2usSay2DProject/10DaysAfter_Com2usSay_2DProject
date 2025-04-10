using System.Collections;
using TMPro;
using UnityEngine;

public class DownCanvas : MonoBehaviour
{
    public TextMeshProUGUI NotificationText;
    //public string CurrentString;
    //private string NotificationContent;
    void Start()
    {
        //NotificationContent = NotificationText.text;
        //GridBuildingSystem.Instance.OnBuildFailed += 건설실패띄우는함수;
        GridBuildingSystem.Instance.OnBuildFailed += Notification;
    }
    

    public void Notification()
    {
        NotificationText.text = "건설 할 수 없습니다.";
        StartCoroutine(NotificationCoroution());
    }

    IEnumerator NotificationCoroution()
    {
        yield return new WaitForSeconds(2f);
        NotificationText.text = "";
    }
    
}
