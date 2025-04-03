using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PhaseManager : Singleton<PhaseManager>
{
    public float PhaseDuration = 30f;
	private bool _isNight = false;

	private IEnumerator PhaseRoutine()
    {
        while (true)
        {
			if (!_isNight)
			{
				//Add Day Phase start functions

				yield return new WaitForSeconds(PhaseDuration);
				_isNight = true;

				//Add Day Phase end functions
				Debug.Log("Day Phase is over");
			}
			else
			{
				//Add Night Phase start functions

				yield return new WaitForSeconds(PhaseDuration);
				_isNight = false;

				//Add Night Phase end functions
				Debug.Log("Night Phase is over");
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
}
