using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WatchTower : TowerRoot
{
    [Header("# Light")]
    [SerializeField] private Light2D _light;
    [SerializeField] private float _rotateSpeed;

    protected override void OnEnable()
    {
        base.OnEnable();
        _light.gameObject.SetActive(false);
    }

    protected override void OnPlaced()
    {
        base.OnPlaced();
        if(_light != null)
        {
            _light.gameObject.SetActive(true);
        }
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

        float z = _light.transform.eulerAngles.z;
        z += _rotateSpeed * Time.deltaTime;
        z %= 360f;
        _light.transform.rotation = Quaternion.Euler(0f, 0f, z);
    }
}