using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UI_TouchBounce : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float EndScale = 0.9f;
    public float StartScale = 1f;
    public float Duration = 0.2f;

    private void Awake()
    {
        StartScale = transform.localScale.x;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOScale(EndScale, Duration).SetEase(Ease.InOutBounce).OnComplete(() => transform.localScale = Vector3.one * EndScale);

    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        transform.DOScale(StartScale, Duration).SetEase(Ease.InOutBounce).OnComplete(() => transform.localScale = Vector3.one * StartScale);
        PlaySFX();
    }
    public void PlaySFX()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySfx(ESfxType.BuildSound); // 테스트 효과음 재생
        }
    }

}