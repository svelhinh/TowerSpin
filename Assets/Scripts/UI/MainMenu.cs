using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	[SerializeField] private Animator _animator;

	private void Start()
	{
		Enter();
	}

	public void Enter()
	{
		_animator.SetTrigger("enter");
	}

	public void DisableAnimator()
	{
		_animator.enabled = false;
	}

	public void EnableAnimator()
	{
		_animator.enabled = true;
	}

	public IEnumerator Disappear()
	{
		EnableAnimator();
		_animator.SetTrigger("exit");

		yield return new WaitForSeconds(1f);

		gameObject.SetActive(false);
	}
}
