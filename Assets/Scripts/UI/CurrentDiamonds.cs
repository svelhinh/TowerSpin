using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentDiamonds : MonoBehaviour
{
	[SerializeField] private TMP_Text _text;

	private int _lastDiamondNb;

	public void OnEnable()
	{
		_lastDiamondNb = -1;
		UpdateText();
	}

	public void UpdateText()
	{
		int diamondNb = GameManager.Instance.DiamondNumber;

		if (_lastDiamondNb != diamondNb)
		{
			_text.text = diamondNb.ToString();
			_lastDiamondNb = diamondNb;
		}
	}
}
