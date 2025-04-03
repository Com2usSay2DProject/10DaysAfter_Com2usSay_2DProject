using UnityEngine;
using System.Collections;

public class DayPhase
{
	public IEnumerator PlayDayPhase(float dayDuration)
	{
		Debug.Log("This is the Day Phase");
		//건물 건설 활성화 (UI)
		//이벤트 발동 활성화

		yield return new WaitForSeconds(dayDuration);

		//건물 건설 비활성화(UI)
		//이벤트 발동 비활성화
	}
}