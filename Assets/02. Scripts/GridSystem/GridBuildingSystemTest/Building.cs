using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    [Header("# Grid Settings")]
    public BoundsInt Area;
    [SerializeField] protected Vector3 _buildingOffset = new Vector3(0.5f, 1.8f, 0f);
    protected Vector3 _inverseBuildingOffset = new Vector3(-0.5f, -1.8f, 0f);
    
    public bool IsPlaced { get; protected set; }

    private HashSet<Collider2D> _overlappingColliders = new HashSet<Collider2D>();
    protected SpriteRenderer _spriteRenderer;
    protected Collider2D _collider;
    protected Color _tempColor = new Color(1, 1, 1, 0.5f);

    public bool HasEnemyOverlap => _overlappingColliders.Count > 0;

    protected virtual void Awake()
    {
        _inverseBuildingOffset = _buildingOffset * -1;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }

    protected virtual void OnEnable()
    {
        _collider.enabled = true;  // 충돌 감지를 위해 활성화
        _spriteRenderer.color = _tempColor;
        IsPlaced = false;
        _overlappingColliders.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            _overlappingColliders.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            _overlappingColliders.Remove(collision);
        }
    }

    #region Build
    protected abstract void OnPlaced();

    public bool CanBePlaced()
    {
        return GridBuildingSystem.Instance.CanTakeArea(GetGridArea()) && !HasEnemyOverlap;
    }

    public void Place()
    {
        if (!CanBePlaced()) return;
        
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
    #endregion
}
