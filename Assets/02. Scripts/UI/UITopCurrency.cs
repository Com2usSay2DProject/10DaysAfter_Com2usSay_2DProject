using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITopCurrency : MonoBehaviour
{
    public List<TextMeshProUGUI> Resources;

    private void Start()
    {
        DisplayTopResources();
        ResourceManager.Instance.OnReourceChange += DisplayTopResources;
    }

    void Update()
    {
        //DisplayTopResources();
    }

    public void DisplayTopResources()
    {
        var resourceTypes = (ResourceType[])System.Enum.GetValues(typeof(ResourceType));

        for (int i = 0; i < Resources.Count && i < resourceTypes.Length; i++)
        {
            Resources[i].text = ResourceManager.Instance.GetResourceAmount(resourceTypes[i]).ToString();
        }
    }


}
