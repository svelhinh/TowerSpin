using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ball : MonoBehaviour
{
	public float firingAngle = 45.0f;
	public float gravity = 9.8f;
	public Rigidbody rb;

	public GameColor ColorName { get; private set; }

	[SerializeField] private Renderer _renderer;
	[SerializeField] private MeshFilter _mesh;
	[SerializeField] private Rigidbody _meshRb;
	[SerializeField] private Material _defaultMaterial;
	[SerializeField] private Material _superMaterial;
	[SerializeField] private Collider _collider;

	private Vector3 _baseLocalPos;
	private Quaternion _meshBaseRotation;

	private bool _stopThrow;

	private GameManager _game;

	private void Awake()
	{
		_game = GameManager.Instance;
	}

	private void Start()
	{
		_baseLocalPos = PoolManager.Instance.GetPoolByType(PoolObjectType.Ball).defaultPos;
		_meshBaseRotation = _mesh.transform.rotation;

		ResetBall();
	}

	public IEnumerator Throw(Vector3 targetPosition)
	{
		_collider.enabled = true;

		Utils.SetKinematic(rb, false);

		// Rotate mesh randomly
		_meshRb.AddTorque(Random.Range(-100.0f, 100.0f), Random.Range(-100.0f, 100.0f), Random.Range(-100.0f, 100.0f));

		// Calculate distance to target
		float target_Distance = Vector3.Distance(transform.position, targetPosition);

		// Calculate the velocity needed to throw the object to the target at specified angle.
		float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

		// Extract the X & Y componenent of the velocity
		float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
		float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

		// Calculate flight time.
		float flightDuration = target_Distance / Vx;

		// Rotate projectile to face the target.
		transform.rotation = Quaternion.LookRotation(targetPosition - transform.position);
		float elapse_time = 0;
		_stopThrow = false;
		while (elapse_time < flightDuration && !_stopThrow)
		{
			transform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);
			elapse_time += Time.deltaTime;
			yield return null;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "towerBlock")
			_stopThrow = true;
	}

	public void SetColor(Color color, GameColor name)
	{
		_renderer.material.color = color;
		ColorName = name;
	}

	private void SetRandomColor()
	{
		if (_game.validBallColors.Count == 0)
			_game.validBallColors.Add(_game.lastBallColor);

		GameColorInfo randomColor = _game.validBallColors[Random.Range(0, _game.validBallColors.Count)];

		SetColor(randomColor.color, randomColor.name);

		if (_game.lastBallColor != null)
			_game.validBallColors.Add(_game.lastBallColor);

		_game.validBallColors.Remove(randomColor);
		_game.lastBallColor = randomColor;
	}

	public void ResetBall()
	{
		Utils.SetKinematic(rb);
		if (_game.currentSkin != -1)
			_mesh.mesh = _game.skins[_game.currentSkin];
		else
			_mesh.mesh = _game.defaultSkin;

		rb.useGravity = false;
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		transform.localPosition = _baseLocalPos;

		_meshRb.velocity = Vector3.zero;
		_meshRb.angularVelocity = Vector3.zero;
		_mesh.transform.rotation = _meshBaseRotation;

		_collider.enabled = false;

		if (_game.activateSuperBall)
		{
			_game.activateSuperBall = false;
			_renderer.material = _superMaterial;
			ColorName = GameColor.BLACK;
		}
		else
		{
			_renderer.material = _defaultMaterial;
			SetRandomColor();
		}
	}

	public Color GetColor()
	{
		return _renderer.material.color;
	}
}
