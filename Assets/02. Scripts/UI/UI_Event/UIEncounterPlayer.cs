using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Text;

//실제로 보여지는 이벤트 팝업
public class UIEncounterPlayer : Singleton<UIEncounterPlayer>
{
	[SerializeField] private GameObject _playerUI;
	[SerializeField] private TextMeshProUGUI _encounterText;
	[SerializeField] private Image _encounterImage;
	[SerializeField] private Button _next;

	[SerializeField] private Transform _choiceContainer;
	[SerializeField] private GameObject _choiceButtonPrefab;
	[SerializeField] private GameObject _choiceEffect;
	[SerializeField] private TextMeshProUGUI _choiceEffectText;

	public Image EndFade;

	private GameEncounter _currentEncounter;
	private int _currentPage = 0;
	private Coroutine _blinkCoroutine;
	private Coroutine _textEffect;

	public int TextSoundInterval = 3;

	public Action OnEncounterClose;

	private void Awake()
	{
		_playerUI.SetActive(false);
	}

	private void Update()
	{
		if (_next.isActiveAndEnabled)
		{
			if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
			{
				NextPage();
			}
		}
	}

	//창 밑의 화살표 같은 것 깜빡이게
	IEnumerator Blink()
	{
		Debug.Log("start blinking");
		while (true)
		{
			_next.image.color = new Color(88f / 255f, 56f / 255f, 36 / 255f, 210f / 255f);
			yield return new WaitForSecondsRealtime(0.7f);
			_next.image.color = new Color(88f / 255f, 56f / 255f, 36 / 255f, 0f);
			yield return new WaitForSecondsRealtime(0.7f);
		}
	}


	//타이핑되는 것처럼 텍스트 나오는 효과
	IEnumerator TypeTextEffect(string text)
	{
		if(_textEffect != null)
		{
			StopCoroutine(_textEffect);
		}

		if (_blinkCoroutine != null)
		{
			StopCoroutine(_blinkCoroutine);
			_next.image.color = new Color(88f / 255f, 56f / 255f, 36 / 255f, 0f);
		}

		_encounterText.text = string.Empty;

		StringBuilder stringBuilder = new StringBuilder();

		for (int i = 0; i < text.Length; i++)
		{
			stringBuilder.Append(text[i]);

			if (i % TextSoundInterval == 0 && !char.IsWhiteSpace(text[i]))
			{
				SoundManager.Instance.PlaySfx(ESfxType.Type);
			}

			_encounterText.text = stringBuilder.ToString();
			yield return new WaitForSecondsRealtime(0.01f);
		}

		yield return new WaitForSecondsRealtime(0.5f);
		_blinkCoroutine = StartCoroutine(Blink());
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


		//_encounterText.text = page.text;
		_textEffect = StartCoroutine(TypeTextEffect(page.text));
		_choiceContainer.gameObject.SetActive(false);
		_next.gameObject.SetActive(false);

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
			_next.gameObject.SetActive(true);
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

		_next.gameObject.SetActive(false);
		_choiceContainer.gameObject.SetActive(true);
	}

	//선택지의 효과 보여주는 용도
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
			string resourceName = ResourceNameTranslator.GetLocalizedName(effect.resourceType);

			sb.Append($"<color={color}>{resourceName} {sign}{Mathf.Abs(effect.amount)}</color>  ");	
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

		if (!PhaseManager.Instance.isNight)
		{
			SoundManager.Instance.PlayBgm(EBgmType.Game);
		} else SoundManager.Instance.PlayBgm(EBgmType.Game_Night);
		OnEncounterClose?.Invoke();
		OnEncounterClose = null;
	}
}
