using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentBallNumber : MonoBehaviour
{
	[SerializeField] private TMP_Text _text;

	private int _lastBallNb;

	public void OnEnable()
	{
		_lastBallNb = -1;
		UpdateText();
	}

	public void UpdateText()
	{
		int ballNb = GameManager.Instance.ballsLeft;

		if (_lastBallNb != ballNb)
		{
			_text.text = $"X{ballNb}";
			_lastBallNb = ballNb;
		}
	}
}
