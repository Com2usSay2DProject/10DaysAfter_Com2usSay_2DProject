using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class DownCanvas : MonoBehaviour
{
    public TextMeshProUGUI NotificationText;
    public GameObject DownBar;
    public Transform StartPosition;


    void Start()
    {
        Vector3 originalPosition = DownBar.transform.position;
        DownBar.transform.position = StartPosition.transform.position;
        DownBar.transform.DOMove(originalPosition, 2f).SetEase(Ease.InOutCubic).SetDelay(2f);

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
