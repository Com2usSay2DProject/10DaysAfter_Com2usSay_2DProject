using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StartCamera : MonoBehaviour
{
    [SerializeField]
    private Light2D _light;
    [SerializeField]
    private float _targetLightOuterRadius;

    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        _mainCamera.DOOrthoSize(10f, 3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            gameObject.AddComponent<CameraMouseFollow>();
            gameObject.AddComponent<MapScroll>();
        });


        _light.pointLightOuterRadius = 0f;
        DOTween.To(() => _light.pointLightOuterRadius,
                   x => _light.pointLightOuterRadius = x,
                   _targetLightOuterRadius,
                   3f)
               .SetEase(Ease.Linear);
    }
}
