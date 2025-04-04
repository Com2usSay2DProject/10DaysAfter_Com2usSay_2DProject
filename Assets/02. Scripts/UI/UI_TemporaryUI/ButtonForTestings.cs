using UnityEngine;
using UnityEngine.UI;

public class ButtonForTestings : MonoBehaviour
{
    public Button addbutton;
    public ResourceType resourceType;

	private void Awake()
	{
		addbutton.onClick.AddListener(Add);
	}
	public void Add()
    {
        ResourceManager.Instance.AddResource(resourceType, 500);
    }
}
