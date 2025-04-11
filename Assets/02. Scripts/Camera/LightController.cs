using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class LightController : MonoBehaviour
{
	public Light2D GlobalLight;
	public float FadeTime = 3f;
	public float DayIntensity = 1f;
	public float NightIntensity = 0.35f;
	public float DayRadius = 200f;
	public float NightRadius = 100f;

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
		while(GlobalLight.intensity > NightIntensity)
		{
			GlobalLight.intensity -= Time.deltaTime / FadeTime;
			yield return null;
		}
		while(GlobalLight.pointLightOuterRadius > NightRadius)
		{
			GlobalLight.pointLightOuterRadius -= Time.deltaTime / FadeTime;
			yield return null;
		}
		GlobalLight.intensity = NightIntensity;
		GlobalLight.pointLightOuterRadius = NightRadius;
	}

	private IEnumerator LightOn()
	{
		while (GlobalLight.intensity < DayIntensity)
		{
			GlobalLight.intensity += Time.deltaTime / FadeTime;
			yield return null;
		}
		while (GlobalLight.pointLightOuterRadius < DayRadius)
		{
			GlobalLight.pointLightOuterRadius += Time.deltaTime / FadeTime;
			yield return null;
		}
		GlobalLight.intensity = DayIntensity;
		GlobalLight.pointLightOuterRadius = DayRadius;
	}
}
