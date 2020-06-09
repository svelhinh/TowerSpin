using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
	[SerializeField] private Animator _animator;

	[Header("Vibration")]
	[SerializeField] private GameObject _vibration;
	[SerializeField] private GameObject _vibrationEnabled;
	[SerializeField] private GameObject _vibrationDisabled;

	private bool _isOpened;

	private GameManager _game;

	private void Start()
	{
		_game = GameManager.Instance;

		EnableVibration();
	}

	public void ToggleSettings()
	{
		_isOpened = !_isOpened;

		_animator.SetTrigger(_isOpened ? "Open" : "Close");

		if (_isOpened)
			EnableVibration(_game.VibrationEnabled);
	}

	public void ToggleVibration()
	{
		_game.VibrationEnabled = !_game.VibrationEnabled;

		EnableVibration(_game.VibrationEnabled);
	}

	private void EnableVibration(bool value = true)
	{
		_vibrationEnabled.SetActive(value);
		_vibrationDisabled.SetActive(!value);
	}
}
