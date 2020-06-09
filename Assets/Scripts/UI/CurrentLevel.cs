using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrentLevel : MonoBehaviour
{
	[SerializeField] private TMP_Text _text;

	private int _lastLevelNb;

	public void OnEnable()
	{
		int levelNb = GameManager.Instance.levelNumber + 1;

		if (_lastLevelNb != levelNb)
		{
			_text.text = $"LEVEL {levelNb}";
			_lastLevelNb = levelNb;
		}
	}
}
