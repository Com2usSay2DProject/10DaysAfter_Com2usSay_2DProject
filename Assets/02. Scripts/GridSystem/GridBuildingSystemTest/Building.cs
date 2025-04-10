using UnityEngine;

public abstract class Building : MonoBehaviour
{
    [Header("# Grid Settings")]
    public BoundsInt Area;
    [SerializeField] protected Vector3 _buildingOffset = new Vector3(0.5f, 1.8f, 0f);
    [SerializeField] protected Vector3 _inverseBuildingOffset = new Vector3(-0.5f, -1.8f, 0f);
    
    public bool IsPlaced { get; protected set; }
    
    protected virtual void OnPlaced() { }
    protected virtual bool ValidatePlacement() => true;

    public bool CanBePlaced()
    {
        if (!ValidatePlacement()) return false;
        return GridBuildingSystem.Instance.CanTakeArea(GetGridArea());
    }

    public void Place()
    {
        if (!CanBePlaced()) return;
        
        //IsPlaced = true;
        GridBuildingSystem.Instance.TakeArea(GetGridArea());
        OnPlaced();
    }

    public BoundsInt GetGridArea()
    {
        Vector3 basePosition = transform.position + _inverseBuildingOffset;
        Vector3Int positionInt = GridBuildingSystem.Instance.GridLayout.WorldToCell(basePosition);
        BoundsInt areaTemp = Area;
        areaTemp.position = positionInt;
        return areaTemp;
    }

    public void SetGridPosition(Vector3Int cellPos)
    {
        Vector3 basePosition = GridBuildingSystem.Instance.GridLayout.CellToWorld(cellPos);
        transform.position = basePosition + _buildingOffset;
    }
}
