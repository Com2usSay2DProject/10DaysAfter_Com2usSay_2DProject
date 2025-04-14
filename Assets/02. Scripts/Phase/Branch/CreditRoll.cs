using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class CreditRoll : MonoBehaviour
{
	public RectTransform creditContent;
	public float targetY = 1200f;
	public float duration = 15f;
	public float delayAfter = 2f;
	public string nextScene = "HyungJin_Title";

	void Start()
	{
		SoundManager.Instance.OnChangedBGMVolume(1f);
		// 초기 위치 설정 (화면 아래)	
		creditContent.anchoredPosition = new Vector2(0, -Screen.height);

		// DOTween 이동
		creditContent.DOAnchorPosY(targetY, duration)
			.SetEase(Ease.Linear)
			.SetUpdate(true)
			.OnComplete(() =>
			{
				DOVirtual.DelayedCall(delayAfter, () =>
				{
					SceneManager.LoadScene(nextScene);
				}, ignoreTimeScale: true);
			});
	}
}
