using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class UITempResources : MonoBehaviour
{
	public List<TextMeshProUGUI> Resources;

	private void Update()
	{
		var resourceTypes = (ResourceType[])System.Enum.GetValues(typeof(ResourceType));

		for (int i = 0; i < Resources.Count && i < resourceTypes.Length; i++)
		{
			Resources[i].text = ResourceManager.Instance.GetResourceAmount(resourceTypes[i]).ToString();
		}
	}
}
