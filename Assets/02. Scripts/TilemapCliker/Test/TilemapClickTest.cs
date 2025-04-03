using UnityEngine;
using UnityEngine.EventSystems;

public class TilemapClickTest : MonoBehaviour
{
    [SerializeField]
    private GameObject _tower;

    public void SpawnTower(Vector3 tilePosition)
    {
        Instantiate(_tower, tilePosition, Quaternion.identity);
    }
}
