using UnityEngine;
using UnityEngine.UI;
using TMPro;

//실제로 보여지는 이벤트 팝업
public class UIEventPlayer : MonoBehaviour
{
	public static UIEventPlayer Instance;

	[SerializeField] private GameObject _playerUI;
	[SerializeField] private TextMeshProUGUI _eventText;
	[SerializeField] private Button _nextButton;
	[SerializeField] private Transform _choiceContainer;
	[SerializeField] private GameObject _choiceButtonPrefab;

	private GameEvent _currentEvent;
	private int _currentPage = 0;
	private void Awake()
	{
		Instance = this;
		_playerUI.SetActive(false);
		_nextButton.onClick.AddListener(NextPage);
	}

	public void Show(GameEvent e)
	{
		_currentEvent = e;
		_currentPage = 0;
		_playerUI.SetActive(true);
		Time.timeScale = 0f;
		ShowPage();
	}

	private void ShowPage()
	{
		_eventText.text = _currentEvent.pages[_currentPage];
		_nextButton.gameObject.SetActive(true);
		_choiceContainer.gameObject.SetActive(false);
	}

	private void NextPage()
	{
		_currentPage++;
		if (_currentPage < _currentEvent.pages.Count)
		{
			ShowPage();
		}
		else
		{
			if (_currentEvent.hasChoices)
				ShowChoices();
			else
				Close();
		}
	}

	private void ShowChoices()
	{
		foreach (Transform child in _choiceContainer)
			Destroy(child.gameObject);

		foreach (var choice in _currentEvent.choices)
		{
			GameObject btn = Instantiate(_choiceButtonPrefab, _choiceContainer);
			btn.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
			btn.GetComponent<Button>().onClick.AddListener(() =>
			{
				EventManager.Instance.ResolveEvent(choice);
				Close();
			});
		}

		_nextButton.gameObject.SetActive(false);
		_choiceContainer.gameObject.SetActive(true);
	}

	private void Close()
	{
		Time.timeScale = 1f;
		_playerUI.SetActive(false);
	}
}
