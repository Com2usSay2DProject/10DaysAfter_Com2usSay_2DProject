using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class PhaseManager : Singleton<PhaseManager>
{
	private int _currentDay = 1;
	public int FinishDay = 10;
	public int CurrentDay => _currentDay;

	private bool _isNight = false;
	public bool isNight => _isNight;

	public float DayPhaseDuration = 30f;
	public float NightPhaseDuration = 30f;

	//for spawners and ui's to use
	public Action OnDayBegin;
	public Action OnDayEnd;
	public Action OnNightBegin;
	public Action OnNightEnd;

	//지금은 그냥 UI 직접 끄기
	public Button BuildButton;

	public IEnumerator PlayDayPhase(float dayDuration)
	{
		Debug.Log("This is the Day Phase");
		//건물 건설 활성화 (UI)
		//이벤트 발동 활성화

		yield return new WaitForSeconds(dayDuration);

		//건물 건설 비활성화(UI)
		//이벤트 발동 비활성화
	}

	public IEnumerator PlayNightPhase(float nightDuration)
	{
		Debug.Log("This is the Night Phase");
		//몹 스폰 활성화

		yield return new WaitForSeconds(nightDuration);

		//몹 스폰 비활성화
	}

	private IEnumerator PhaseRoutine()
    {
        while (true)
        {
			if (!_isNight)
			{
				if (_currentDay >= 10)
				{
					//엔딩 추가 (씬 전환)
					Debug.Log("축하합니다 10일 끝입니다");
				}
				OnDayBegin?.Invoke();							//건설 UI, 이벤트 시작

				yield return StartCoroutine(PlayDayPhase(DayPhaseDuration));

				
				OnDayEnd?.Invoke();								//건설 UI, 이벤트 끝
				BuildButton.gameObject.SetActive(false);		//지금은 그냥 직접 끔


				//다음 페이즈는 밤
				_isNight = true;
				Debug.Log("Day Phase is over");
			}
			else
			{
				
				OnNightBegin?.Invoke();							//스포너 키기(적 스폰), 빛 조절

				yield return StartCoroutine(PlayNightPhase(NightPhaseDuration));

				OnNightEnd?.Invoke();							//스포너 끄기, 빛 조절
				_isNight = false;								//다음 페이즈는 낮
				BuildButton.gameObject.SetActive(true);			//지금은 그냥 직접 킴(건설UI)

				_currentDay++;
				Debug.Log($"Night Phase is over. You survived {_currentDay} days");
			}
        }
    }

	private void Awake()
	{
		Initialize_DontDestroyOnLoad();
	}

	private void Start()
	{
        StartCoroutine(PhaseRoutine());
	}

	//낮/밤 시간 바꿀 때
	public void SetDayDuration(float newDuration)
	{
		DayPhaseDuration = newDuration;
	}

	public void SetNightDuration(float newDuration)
	{
		NightPhaseDuration = newDuration;
	}
}
