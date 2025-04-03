using UnityEngine;
using System.Collections;

public class NightPhase
{
	public IEnumerator PlayNightPhase(float nightDuration)
	{
		Debug.Log("This is the Night Phase");
		//몹 스폰 활성화

		yield return new WaitForSeconds(nightDuration);

		//몹 스폰 비활성화
	}
}
