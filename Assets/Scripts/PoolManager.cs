using System;
using System.Collections.Generic;
using UnityEngine;

public enum PoolObjectType
{
	Ball,
	DestroyParticles,
	UnlockParticles,
}

[Serializable]
public class PoolInfo
{
	public PoolObjectType type;
	public int amount;
	public GameObject prefab;
	public GameObject container;
	public Vector3 defaultPos = Vector3.zero;

	[HideInInspector]
	public List<GameObject> pool = new List<GameObject>();
}

public class PoolManager : MonoBehaviour
{
	public static PoolManager Instance;

	[SerializeField]
	private List<PoolInfo> listOfPool;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
			Destroy(this);
	}

	private void Start()
	{
		for (int i = 0; i < listOfPool.Count; i++)
			FillPool(listOfPool[i]);
	}

	private void FillPool(PoolInfo info)
	{
		for (int i = 0; i < info.amount; i++)
		{
			GameObject objInstance = Instantiate(info.prefab, info.container.transform);
			objInstance.gameObject.SetActive(false);
			info.pool.Add(objInstance);
		}
	}

	public GameObject GetPoolObject(PoolObjectType type)
	{
		PoolInfo selected = GetPoolByType(type);
		List<GameObject> pool = selected.pool;

		GameObject objInstance;
		if (pool.Count > 0)
		{
			objInstance = pool[pool.Count - 1];
			pool.Remove(objInstance);
		}
		else
			objInstance = Instantiate(selected.prefab, selected.container.transform);

		objInstance.SetActive(true);

		return objInstance;
	}

	public void CoolObject(GameObject obj, PoolObjectType type)
	{
		obj.SetActive(false);

		PoolInfo selected = GetPoolByType(type);
		List<GameObject> pool = selected.pool;

		if (!pool.Contains(obj))
			pool.Add(obj);
	}

	public PoolInfo GetPoolByType(PoolObjectType type)
	{
		for (int i = 0; i < listOfPool.Count; i++)
		{
			if (type == listOfPool[i].type)
				return listOfPool[i];
		}

		return null;
	}
}
