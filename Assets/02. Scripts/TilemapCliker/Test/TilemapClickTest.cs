using UnityEngine;
using UnityEngine.EventSystems;

public class TilemapClickTest : Singleton<TilemapClickTest>
{
    [SerializeField]
    private GameObject _tower;

    public void SpawnTower(Vector3 tilePosition)
    {
        Instantiate(_tower, tilePosition, Quaternion.identity);
    }
}
