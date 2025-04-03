using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PhaseManager : Singleton<PhaseManager>
{
	private int _currentDay = 1;
	public int FinishDay = 10;
	public int CurrentDay => _currentDay;

	private DayPhase _dayPhase = new DayPhase();
	private NightPhase _nightPhase = new NightPhase();
	private bool _isNight = false;

	public float DayPhaseDuration = 30f;
	public float NightPhaseDuration = 30f;

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
				yield return StartCoroutine(_dayPhase.PlayDayPhase(DayPhaseDuration));
				_isNight = true;
				Debug.Log("Day Phase is over");
			}
			else
			{
				yield return StartCoroutine(_nightPhase.PlayNightPhase(NightPhaseDuration));
				_isNight = false;

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

	public void SetDayDuration(float newDuration)
	{
		DayPhaseDuration = newDuration;
	}

	public void SetNightDuration(float newDuration)
	{
		NightPhaseDuration = newDuration;
	}
}
