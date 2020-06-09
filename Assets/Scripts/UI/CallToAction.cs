using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CallToAction : MonoBehaviour
{
	[SerializeField] private Animator _animator;
	[SerializeField] private Image _image;

	void Update()
	{
		if (Input.touchCount == 1)
		{
			Touch touch = Input.GetTouch(0);

			if (touch.phase == TouchPhase.Ended)
				Stop();
		}
	}

	public void Play()
	{
		_image.enabled = true;
		_animator.enabled = true;
	}

	public void Stop()
	{
		_image.enabled = false;
		_animator.enabled = false;
	}
}
