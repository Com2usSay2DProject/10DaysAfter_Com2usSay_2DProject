using UnityEngine;
using UnityEngine.UI;
using TMPro;

//event가 trigger되었을 때 옆에 뜨는 탭
public class UIEventTab : MonoBehaviour
{
	public static UIEventTab Instance;

	[SerializeField] private GameObject tabUI;
	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private Button openButton;

	private void Awake()
	{
		Instance = this;
		tabUI.SetActive(false);
		openButton.onClick.AddListener(OnTabClicked);
	}

	public void ShowTab(string title)
	{
		titleText.text = title;
		tabUI.SetActive(true);
	}

	public void HideTab()
	{
		tabUI.SetActive(false);
	}

	private void OnTabClicked()
	{
		EventManager.Instance.OpenEventPlayer();
	}
}
