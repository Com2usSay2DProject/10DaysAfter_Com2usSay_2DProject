using UnityEngine;
using UnityEngine.EventSystems;

public class TowerSpawner : Singleton<TowerSpawner>
{
    [SerializeField]
    private GameObject _tower;

    public Vector3 TowerOffset;

    public void SpawnTower(Vector3 tilePosition)
    {
        Instantiate(_tower, tilePosition + TowerOffset, Quaternion.identity);
    }
}
