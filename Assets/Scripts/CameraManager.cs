using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	public Transform target;
	public bool isSpinning;

	[HideInInspector] public float speedMultiplier = 1f;

	[SerializeField] private GameObject _camera;
	[SerializeField] private float _baseSpeed = 40f;
	[SerializeField] private float maxSpinSpeed = 100f;
	[SerializeField] private float _shakeDuration;
	[SerializeField] private float _shakeAmount = 0.05f;
	[SerializeField] private float _unzoomTime = 1f;
	[SerializeField] private float _unzoomSpeed = 1f;

	private float _speed = 1f;
	private Vector3 _cameraOriginalPos;
	private Quaternion _cameraOriginalRotation;
	private Vector3 _cameraOriginalLocalPos;

	private void Start()
	{
		isSpinning = true;
		_cameraOriginalPos = transform.position;
		_cameraOriginalRotation = transform.rotation;
	}

	private void Update()
	{
		if (isSpinning)
			transform.RotateAround(target.transform.position, Vector3.up, -_speed * Time.deltaTime * speedMultiplier);
	}

	public void GoDownOneFloor(float floorHeight)
	{
		transform.position = new Vector3(transform.position.x, transform.position.y - floorHeight, transform.position.z);
	}

	public void Accelerate()
	{
		if (_speed < maxSpinSpeed)
			_speed++;
	}

	public void ResetValues()
	{
		_speed = _baseSpeed;
		transform.position = _cameraOriginalPos;
		transform.rotation = _cameraOriginalRotation;
	}

	public void Shake()
	{
		StartCoroutine(DoShake());
		Vibration.Vibrate(75);
	}

	private IEnumerator DoShake()
	{
		_cameraOriginalLocalPos = _camera.transform.localPosition;
		var elapsed = 0f;

		while (elapsed < _shakeDuration)
		{
			_camera.transform.localPosition = _cameraOriginalLocalPos + Random.insideUnitSphere * _shakeAmount;
			elapsed += Time.deltaTime;
			yield return 0;
		}
		_camera.transform.localPosition = _cameraOriginalLocalPos;
	}

	public IEnumerator Unzoom()
	{
		isSpinning = true;

		float t = 0f;
		while (t < _unzoomTime)
		{
			transform.Translate(Vector3.back * _unzoomSpeed * Time.deltaTime);
			t += Time.deltaTime;
			yield return null;
		}
	}
}
