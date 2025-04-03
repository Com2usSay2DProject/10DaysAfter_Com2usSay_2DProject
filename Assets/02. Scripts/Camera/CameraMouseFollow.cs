using UnityEngine;

public class CameraMouseFollow : MonoBehaviour
{
    private Vector3 _mousePosition;
	private Camera _mainCamera;

	public float MoveSpeed = 10f;
	public int EdgeSize = 20;					   //감지할 끄트머리 범위

	public Vector2 xBounds = new Vector2(-30, 30); // 카메라 X 이동 제한
	public Vector2 yBounds = new Vector2(-30, 30); // 카메라 Y 이동 제한

	private void Awake()
	{
		//메인 카메라 할당
		if (_mainCamera == null)
			_mainCamera = Camera.main;
	}
	private void Update()
	{
		Vector3 moveDirection = Vector3.zero;
		_mousePosition = Input.mousePosition;

		if(_mousePosition.x < EdgeSize)
		{
			moveDirection.x = -1;
		} else if(_mousePosition.x > Screen.width - EdgeSize)
		{
			moveDirection.x = 1;
		}

		if (_mousePosition.y < EdgeSize)
		{
			moveDirection.y = -1;
		}
		else if (_mousePosition.y > Screen.height - EdgeSize)
		{
			moveDirection.y = 1;
		}

		Vector3 newPosition = _mainCamera.transform.position + moveDirection.normalized * MoveSpeed * Time.deltaTime;

		newPosition.x = Mathf.Clamp(newPosition.x, xBounds.x, xBounds.y);
		newPosition.y = Mathf.Clamp(newPosition.y, yBounds.x, yBounds.y);

		_mainCamera.transform.position= new Vector3(newPosition.x, newPosition.y, _mainCamera.transform.position.z);
	}
}
