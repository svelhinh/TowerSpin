using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgression : MonoBehaviour
{
	[SerializeField] private Slider _slider;
	[SerializeField] private Image _fill;
	[SerializeField] private TMP_Text _text;

	private float _lastLevelProgression;

	private GameManager _game;

	private void Start()
	{
		_game = GameManager.Instance;
	}

	private void OnEnable()
	{
		_slider.value = 0;
		_text.text = "0%";
		_lastLevelProgression = 0;
	}

	private void Update()
	{
		if (_lastLevelProgression != _game.levelProgression)
		{
			_slider.value = _game.levelProgression * 0.01f;

			float roundedLevelProgression = Mathf.Floor(_game.levelProgression);
			_text.text = $"{roundedLevelProgression}%";

			_lastLevelProgression = roundedLevelProgression;
		}
	}

	public void TransitionColor()
	{
		StartCoroutine(Utils.ImageColorTransition(_fill, _fill.color, _game.currentBall.GetColor(), 2, 1));
	}
}
