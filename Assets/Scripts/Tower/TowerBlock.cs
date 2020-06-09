using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBlock : MonoBehaviour
{
	public GameColorInfo disabledColor;
	public Rigidbody rb;

	[HideInInspector] public GameColorInfo baseColor;
	[HideInInspector] public GameColorInfo colorInfo;
	public bool IsOnTower { get; private set; }

	[HideInInspector] public int floorNb;

	[SerializeField] private Renderer _renderer;
	[SerializeField] private float _overlapSphereRadius = 0.5f;
	[SerializeField] private Buoyancy _buoyancy;

	private bool _canCheckFall;

	private GameManager _game;

	private void Start()
	{
		_game = GameManager.Instance;

		IsOnTower = true;
		_buoyancy.enabled = false;
	}

	public void SetColor(GameColorInfo colorInfo)
	{
		_renderer.material.color = colorInfo.color;
		this.colorInfo = colorInfo;
	}

	public void Enable(bool value = true, bool onlyPhysics = false, bool withParticles = true)
	{
		if (value)
		{
			if (withParticles && _game.canRemoveBlocksFromTower)
				ShowParticles(PoolObjectType.UnlockParticles);

			Utils.SetKinematic(rb, false);
			if (!onlyPhysics)
				SetColor(baseColor);
		}
		else
		{
			Utils.SetKinematic(rb);
			if (!onlyPhysics)
				SetColor(disabledColor);
		}
	}

	private void OnMouseDown()
	{
		if (_game.canThrowBall && colorInfo.name != GameColor.REAL_BLACK && IsOnTower)
		{
			_game.canThrowBall = false;
			_game.mainCamera.isSpinning = false;
			StartCoroutine(_game.ThrowBall(gameObject));
		}
	}

	private void OnDestroy()
	{
		if (!_game.canRemoveBlocksFromTower)
			return;

		ShowParticles(PoolObjectType.DestroyParticles);
		RemoveFromTower();

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, _overlapSphereRadius);

		for (int i = 0; i < hitColliders.Length; i++)
		{
			if (hitColliders[i].tag == "towerBlock")
			{
				TowerBlock towerBlock = hitColliders[i].GetComponentInParent<TowerBlock>();
				if (towerBlock.colorInfo.name == colorInfo.name)
					Destroy(towerBlock.gameObject);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, _overlapSphereRadius);
	}

	private void Update()
	{
		if (_game.CurrentState == _game.RunningState && IsOnTower && colorInfo.name != GameColor.REAL_BLACK && transform.position.y < _game.tower.hiddenFloorsLimit)
			RemoveFromTower();
	}

	private void RemoveFromTower()
	{
		IsOnTower = false;

		_game.tower.nbRemainingBlocks--;
		if (_game.levelProgression < 100)
		{
			_game.levelProgression = (1 - (float) _game.tower.nbRemainingBlocks / _game.tower.nbStartRemainingBlocks) * 100;
			if (_game.levelProgression >= 100)
				_game.levelProgression = 100;
		}

		_game.tower.Floors[floorNb].TryToUnlockHighestHiddenFloor();
		EnableBuoyancy();
	}

	public void Clean()
	{
		Destroy(gameObject);
	}

	public void ShowParticles(PoolObjectType type)
	{
		GameObject destroyUnlockParticles = PoolManager.Instance.GetPoolObject(type);
		destroyUnlockParticles.transform.position = transform.position;
		destroyUnlockParticles.GetComponent<TowerParticlesManager>().SetColor(_renderer.material.color);
	}

	private void EnableBuoyancy(bool enable = true)
	{
		_buoyancy.enabled = enable;
	}
}
