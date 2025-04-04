using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightTesting : MonoBehaviour
{
	public Light2D GlobalLight;

	public void Start()
	{
		PhaseManager.Instance.OnNightBegin += ChangeNightToDark;
		PhaseManager.Instance.OnDayBegin += ChangeDayToLight;
		Debug.Log("added light change for testing");
	}

	public void ChangeNightToDark()
	{
		GlobalLight.intensity = 0.35f;
	}

	public void ChangeDayToLight()
	{
		GlobalLight.intensity = 1f;
	}
}
