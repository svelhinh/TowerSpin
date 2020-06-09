using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerParticlesManager : MonoBehaviour
{
	[SerializeField] private PoolObjectType _type;

	private ParticleSystemRenderer _particleSystemRenderer;

	private void OnEnable()
	{
		if (_particleSystemRenderer == null)
			_particleSystemRenderer = GetComponent<ParticleSystemRenderer>();

		Invoke("Disable", 1f);
	}

	private void Disable()
	{
		PoolManager.Instance.CoolObject(gameObject, _type);
	}

	public void SetColor(Color color)
	{
		_particleSystemRenderer.material.color = color;
	}
}
