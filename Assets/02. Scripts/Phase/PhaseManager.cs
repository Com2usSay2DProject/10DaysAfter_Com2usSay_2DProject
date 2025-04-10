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
	public float FadeTime = 3f;

	//for spawners and ui's to use
	public Action OnDayBegin;
	public Action OnDayEnd;
	public Action OnNightBegin;
	public Action OnNightEnd;

	public Action OnDateChange;

	//다음 페이즈까지 타이머
	private float _timeUntilNextPhase;
	public float TimeUntilNextPhase => _timeUntilNextPhase;

	//디버깅용
	//public bool DoTriggerEncounters = false;

	public IEnumerator PlayDayPhase(float dayDuration)
	{
		Debug.Log("This is the Day Phase");
		//이벤트 발동 활성화
		EncounterManager.Instance.TriggerStory(_currentDay);

		yield return new WaitForSeconds(dayDuration);

		//이벤트 발동 비활성화
		EncounterManager.Instance.IgnoreEncounter();
	}

	public IEnumerator PlayNightPhase(float nightDuration)
	{
		Debug.Log("This is the Night Phase");
		EncounterManager.Instance.TriggerStory(_currentDay);

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
				OnDayBegin?.Invoke();							//이벤트 시작

				yield return StartCoroutine(PlayDayPhase(DayPhaseDuration));

				
				OnDayEnd?.Invoke();                             //이벤트 끝
				yield return new WaitForSeconds(FadeTime);


				//다음 페이즈는 밤
				_isNight = true;
				Debug.Log("Day Phase is over");
				_timeUntilNextPhase = NightPhaseDuration + FadeTime;
			}
			else
			{
				
				OnNightBegin?.Invoke();							//스포너 키기(적 스폰), 빛 조절

				yield return StartCoroutine(PlayNightPhase(NightPhaseDuration));

				OnNightEnd?.Invoke();							//스포너 끄기, 빛 조절
				yield return new WaitForSeconds(FadeTime);
				_isNight = false;								//다음 페이즈는 낮

				Debug.Log($"Night Phase is over. You survived {_currentDay} days");
				_currentDay++;
				OnDateChange?.Invoke();
				_timeUntilNextPhase = DayPhaseDuration + FadeTime;
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
		_timeUntilNextPhase = DayPhaseDuration + FadeTime;
	}

	private void Update()
	{
		if (_timeUntilNextPhase > 0) _timeUntilNextPhase -= Time.deltaTime;
		else _timeUntilNextPhase = 0;
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