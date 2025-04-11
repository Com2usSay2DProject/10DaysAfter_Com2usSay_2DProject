using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WatchTower : TowerRoot
{
    [Header("# Light")]
    [SerializeField] private Light2D _light;
    [SerializeField] private float _rotateSpeed;

    protected override void OnPlaced()
    {
        base.OnPlaced();
    }

    private void Update()
    {
        RotateLight();
    }

    private void RotateLight()
    {
        if (!IsPlaced)
        {
            return;
        }
        if(!_light.gameObject.activeSelf)
        {
            _light.gameObject.SetActive(true);
            _light.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        }
        
        float z = _light.transform.eulerAngles.z;
        z += _rotateSpeed * Time.deltaTime;
        z %= 360f;
        _light.transform.rotation = Quaternion.Euler(0f, 0f, z);
    }
}