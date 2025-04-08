using UnityEngine;
using System;

public class MapScroll : Singleton<MapScroll>		//카메라가 orthographic일 때만 가능
{
    public float ScrollSpeed = 10f;

    public float MaxZoomIn = 5f;
    public float MaxZoomOut = 30f;

	private Camera _mainCamera;
	public Action OnCameraScroll;

	private void Awake()
	{
		if (_mainCamera == null)
			_mainCamera = Camera.main;
	}

	private void Update()
	{
		float scroll = Input.GetAxis("Mouse ScrollWheel") * 10f;

		if (scroll != 0) OnCameraScroll?.Invoke();

		float size = _mainCamera.orthographicSize - scroll;
		_mainCamera.orthographicSize = Mathf.Clamp(size, MaxZoomIn, MaxZoomOut);
	}
}