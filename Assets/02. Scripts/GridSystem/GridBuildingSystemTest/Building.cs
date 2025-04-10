using UnityEngine;

public class Building : MonoBehaviour
{
    public bool Placed { get; private set; }
    public BoundsInt area;

    #region BuildMethods

    public bool CanBePlaced()
    {
        Vector3 position = transform.position;
        Vector3 offset = new Vector3(-0.5f, -1.8f, 0f);  // 건물 오프셋을 역으로 적용
        Vector3 basePosition = position + offset;
        Vector3Int positionInt = GridBuildingSystem.current.gridLayout.WorldToCell(basePosition);
        BoundsInt areaTemp = area;
        areaTemp.position = positionInt;

        return GridBuildingSystem.current.CanTakeArea(areaTemp);
    }

    public void Place()
    {
        Vector3 position = transform.position;
        Vector3 offset = new Vector3(-0.5f, -1.8f, 0f);  // 건물 오프셋을 역으로 적용
        Vector3 basePosition = position + offset;
        Vector3Int positionInt = GridBuildingSystem.current.gridLayout.WorldToCell(basePosition);
        BoundsInt areaTemp = area;
        areaTemp.position = positionInt;
        Placed = true;
        GridBuildingSystem.current.TakeArea(areaTemp);
    }
    #endregion
}
