using UnityEngine;

[CreateAssetMenu(fileName = "ResourceIconSO", menuName = "Scriptable Objects/ResourceIconSO")]
public class ResourceIconSO : ScriptableObject
{
    public Sprite WoodIcon;
    public Sprite StoneIcon;
    public Sprite MetalIcon;

    public Sprite GetIcon(ResourceType type)
    {
        return type switch
        {
            ResourceType.Wood => WoodIcon,
            ResourceType.Stone => StoneIcon,
            ResourceType.Metal => MetalIcon,
            _ => null,
        };
    }
}
