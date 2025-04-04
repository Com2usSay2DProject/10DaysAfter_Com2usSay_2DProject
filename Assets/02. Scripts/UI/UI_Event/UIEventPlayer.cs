using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Collections.Generic;

//실제로 보여지는 이벤트 팝업
public class UIEventPlayer : MonoBehaviour
{
	public static UIEventPlayer Instance;

	[SerializeField] private GameObject _playerUI;
	[SerializeField] private TextMeshProUGUI _eventText;
	[SerializeField] private Image _eventImage;
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
		EventPage page = _currentEvent.Pages[_currentPage];

		_eventText.text = page.text;
		_choiceContainer.gameObject.SetActive(false);
		_nextButton.gameObject.SetActive(true);

		if (!string.IsNullOrEmpty(page.imagePath))
		{
			Sprite image = Resources.Load<Sprite>("Images/" + page.imagePath);
			if (image != null)
			{
				_eventImage.sprite = image;
				_eventImage.gameObject.SetActive(true);
			}
		}

		// 이 페이지에 선택지가 있다면 다음 버튼 숨기고 선택지 보여줌
		if (page.Choices != null && page.Choices.Count > 0)
		{
			ShowChoices(page.Choices);
		}
	}

	private void NextPage()
	{
		_currentPage++;
		if (_currentPage < _currentEvent.Pages.Count)
		{
			ShowPage();
		}
		else
		{
			Close();
		}
	}

	private void ShowChoices(List<EventChoice> choices)
	{
		foreach (Transform child in _choiceContainer) Destroy(child.gameObject);

		foreach (var choice in choices)
		{
			GameObject button = Instantiate(_choiceButtonPrefab, _choiceContainer);
			button.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
			button.GetComponent<Button>().onClick.AddListener(() =>
			{
				EventManager.Instance.ResolveEvent(choice);
				NextPage();
				_nextButton.gameObject.SetActive(true);
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
