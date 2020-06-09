using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skin : MonoBehaviour
{
	public int id;
	public GameObject equipped;

	[SerializeField] private GameObject _lockedSkin;
	[SerializeField] private GameObject _unLockedSkin;

	private void Start()
	{
		LockSkin(!GameManager.Instance.unlockedSkins.Contains(id));

		equipped.SetActive(GameManager.Instance.currentSkin == id);
	}

	public void LockSkin(bool value = true)
	{
		_lockedSkin.SetActive(value);
		_unLockedSkin.SetActive(!value);
	}
}
