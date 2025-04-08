using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Collections.Generic;

//실제로 보여지는 이벤트 팝업
public class UIEncounterPlayer : MonoBehaviour
{
	public static UIEncounterPlayer Instance;

	[SerializeField] private GameObject _playerUI;
	[SerializeField] private TextMeshProUGUI _encounterText;
	[SerializeField] private Image _encounterImage;
	[SerializeField] private Button _nextButton;

	[SerializeField] private Transform _choiceContainer;
	[SerializeField] private GameObject _choiceButtonPrefab;

	private GameEncounter _currentEncounter;
	private int _currentPage = 0;
	private void Awake()
	{
		Instance = this;
		_playerUI.SetActive(false);
		_nextButton.onClick.AddListener(NextPage);
	}

	public void Show(GameEncounter e)
	{
		_currentEncounter = e;
		_currentPage = 0;
		_playerUI.SetActive(true);
		Time.timeScale = 0f;
		ShowPage();
	}

	private void ShowPage()
	{
		EncounterPage page = _currentEncounter.Pages[_currentPage];

		_encounterText.text = page.text;
		_choiceContainer.gameObject.SetActive(false);
		_nextButton.gameObject.SetActive(true);

		if (!string.IsNullOrEmpty(page.imagePath))
		{
			Sprite image = Resources.Load<Sprite>("Images/" + page.imagePath);
			if (image != null)
			{
				_encounterImage.sprite = image;
				_encounterImage.gameObject.SetActive(true);
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
		EncounterPage current = _currentEncounter.Pages[_currentPage];

		if (current.nextPageIndex >= 0)
		{
			_currentPage = current.nextPageIndex;
		}
		else
		{
			_currentPage++;
		}

		if (_currentPage < _currentEncounter.Pages.Count)
		{
			ShowPage();
		}
		else
		{
			Close();
		}
	}

	private void ShowChoices(List<EncounterChoice> choices)
	{
		foreach (Transform child in _choiceContainer) Destroy(child.gameObject);

		foreach (var choice in choices)
		{
			GameObject button = Instantiate(_choiceButtonPrefab, _choiceContainer);
			button.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;
			button.GetComponent<Button>().onClick.AddListener(() =>
			{
				EncounterManager.Instance.ResolveEncounter(choice);

				//같은 인카운터 내 선택지로 반응 바뀌는 용
				if (choice.nextPageIndex >= 0 && choice.nextPageIndex < _currentEncounter.Pages.Count)
				{
					_currentPage = choice.nextPageIndex;
					ShowPage();
				}
				else
				{
					NextPage();
				}

				_nextButton.gameObject.SetActive(true);
			});
		}

		_nextButton.gameObject.SetActive(false);
		_choiceContainer.gameObject.SetActive(true);
	}

	private void Close()
	{
		Time.timeScale = 1f;

		if (_currentEncounter != null)
		{
			EncounterManager.Instance.ResolveEncounter(null); // 선택지가 없는 경우 처리
		}
		_playerUI.SetActive(false);
	}
}
