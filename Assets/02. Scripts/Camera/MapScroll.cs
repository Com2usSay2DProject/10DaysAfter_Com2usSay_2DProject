using UnityEngine;

public class MapScroll : MonoBehaviour
{
    public float ScrollSpeed = 10f;

    public float MaxZoomIn = 5f;
    public float MaxZoomOut = 30f;

	private Camera _mainCamera;

	private void Awake()
	{
		if (_mainCamera == null)
			_mainCamera = Camera.main;
	}

	private void Update()
	{
		float scroll = Input.GetAxis("Mouse ScrollWheel") * 10f;

		float size = _mainCamera.orthographicSize - scroll;
		_mainCamera.orthographicSize = Mathf.Clamp(size, MaxZoomIn, MaxZoomOut);
	}
}