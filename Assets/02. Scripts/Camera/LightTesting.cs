using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class LightTesting : MonoBehaviour
{
	public Light2D GlobalLight;
	public float FadeTime = 3f;

	public void Start()
	{
		PhaseManager.Instance.OnDayEnd += ChangeNightToDark;
		PhaseManager.Instance.OnNightEnd += ChangeDayToLight;
		Debug.Log("added light change for testing");
	}

	public void ChangeNightToDark()
	{
		StartCoroutine(LightOut());
		//GlobalLight.intensity = 0.35f;
	}

	public void ChangeDayToLight()
	{
		StartCoroutine(LightOn());
		//GlobalLight.intensity = 1f;
	}

	private IEnumerator LightOut()
	{
		while(GlobalLight.intensity > 0.35f)
		{
			GlobalLight.intensity -= Time.deltaTime / FadeTime;
			yield return null;
		}
		GlobalLight.intensity = 0.35f;
	}

	private IEnumerator LightOn()
	{
		while (GlobalLight.intensity < 1.5f)
		{
			GlobalLight.intensity += Time.deltaTime / FadeTime;
			yield return null;
		}
		GlobalLight.intensity = 1.5f;
	}
}
