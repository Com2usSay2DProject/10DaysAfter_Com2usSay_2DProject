using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System;
using System.Collections;

//실제로 보여지는 이벤트 팝업
public class UIEncounterPlayer : Singleton<UIEncounterPlayer>
{
	[SerializeField] private GameObject _playerUI;
	[SerializeField] private TextMeshProUGUI _encounterText;
	[SerializeField] private Image _encounterImage;
	[SerializeField] private Button _nextButton;

	[SerializeField] private Transform _choiceContainer;
	[SerializeField] private GameObject _choiceButtonPrefab;
	[SerializeField] private GameObject _choiceEffect;
	[SerializeField] private TextMeshProUGUI _choiceEffectText;

	public Image EndFade;

	private GameEncounter _currentEncounter;
	private int _currentPage = 0;

	public Action OnEncounterClose;

	private void Awake()
	{
		_playerUI.SetActive(false);
		_nextButton.onClick.AddListener(NextPage);
	}

	public void Show(GameEncounter e, Action onCloseCallback = null)
	{
		_currentEncounter = e;
		_currentPage = 0;
		_playerUI.SetActive(true);
		Time.timeScale = 0f;
		OnEncounterClose = onCloseCallback;

		ShowPage();
	}

	private void ShowPage()
	{
		EncounterPage page = _currentEncounter.Pages[_currentPage];

		_encounterText.text = page.text;
		_choiceContainer.gameObject.SetActive(false);
		_nextButton.gameObject.SetActive(false);

		if (!string.IsNullOrEmpty(page.imagePath))
		{
			Sprite image = Resources.Load<Sprite>("Images/" + page.imagePath);
			if (image != null)
			{
				_encounterImage.sprite = image;
				_encounterImage.gameObject.SetActive(true);
			}
		}

		//bgm 넣어보기
		if (!string.IsNullOrEmpty(page.bgm))
		{
			if (page.bgm == "stopthebgm")
			{
				SoundManager.Instance.StopBgm();
			}
			else if (Enum.TryParse<EBgmType>(page.bgm, out var bgmType))
			{
				SoundManager.Instance.PlayBgm(bgmType);
				SoundManager.Instance.OnChangedBGMVolume(0.5f);
				Debug.Log($"playing bgmtype {bgmType}");
			}
		}

		// SFX 처리
		if (!string.IsNullOrEmpty(page.sfx))
		{
			if (Enum.TryParse<ESfxType>(page.sfx, out var sfxType))
			{
				SoundManager.Instance.PlaySfx(sfxType);
			}
		}

		// 이 페이지에 선택지가 있다면 다음 버튼 숨기고 선택지 보여줌
		if (page.Choices != null && page.Choices.Count > 0)
		{
			ShowChoices(page.Choices);
		}
		else
		{
			_nextButton.gameObject.SetActive(true);
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
			bool canAfford = true;

			GameObject button = Instantiate(_choiceButtonPrefab, _choiceContainer);
			button.GetComponentInChildren<TextMeshProUGUI>().text = choice.text;

			if(choice.effects != null)
			{
				foreach (var effect in choice.effects)
				{
					if (effect.amount < 0)
					{
						int currentAmount = ResourceManager.Instance.GetResourceAmount(effect.resourceType);
						if (currentAmount < -effect.amount)
						{
							canAfford = false;
							break;
						}
					}
				}
			}
			if (canAfford)
			{
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

					StartCoroutine(ChoiceEffectCoroutine(choice.effects));
				});
			}
			else
			{
				button.GetComponent<Button>().interactable = false;
				button.GetComponentInChildren<TextMeshProUGUI>().text = "(자원 부족)";
			}
		}

		_nextButton.gameObject.SetActive(false);
		_choiceContainer.gameObject.SetActive(true);
	}

	private IEnumerator ChoiceEffectCoroutine(List<EncounterEffect> effects)
	{
		ShowChoiceEffects(effects);
		yield return new WaitForSecondsRealtime(3f);
		_choiceEffect.SetActive(false);
	}
	private void ShowChoiceEffects(List<EncounterEffect> effects)
	{
		if (effects == null)
		{
			_choiceEffect.SetActive(false);
			return;
		}

		_choiceEffect.SetActive(true);

		System.Text.StringBuilder sb = new();

		foreach (EncounterEffect effect in effects)
		{
			if (effect.amount == 0) continue;

			string sign = effect.amount > 0 ? "+" : "-";
			string color = effect.amount > 0 ? "#7FD97A" : "#FF6F5A";
			string resourceName = effect.resourceType.ToString();

			sb.Append($"<color={color}>{sign}{Mathf.Abs(effect.amount)} {resourceName}</color>  ");	
		}

		_choiceEffectText.DOFade(255f, 1.5f).SetUpdate(true);
		_choiceEffectText.text = sb.ToString().TrimEnd();
		_choiceEffectText.DOFade(0f, 1.5f).SetUpdate(true);
	}

	private void Close()
	{
		Time.timeScale = 1f;

		if (_currentEncounter != null)
		{
			EncounterManager.Instance.ResolveEncounter(null); // 선택지가 없는 경우 처리
		}
		_playerUI.SetActive(false);

		OnEncounterClose?.Invoke();
		OnEncounterClose = null;
	}
}
