using DG.Tweening;
using TMPro;
using UnityEngine;

public class UINumberPopup : MonoBehaviour
{
    public TextMeshProUGUI PopupText;
    public float Value = 2f;
    public float Duration = 1f;
    private Vector3 _originaPosition;

    private void Awake()
    {
       _originaPosition = PopupText.transform.position;
    }

    private void OnEnable()
    {
        //_originaPosition = PopupText.transform.position;
        PopupText.transform.DOMoveY(transform.position.y+Value, Duration)
            .SetEase(Ease.OutQuart)
            .OnComplete(() => SetPosition()
            );
    }

    private void SetPosition()
    {
        PopupText.transform.position = _originaPosition;
        transform.gameObject.SetActive(false);
    }

}
