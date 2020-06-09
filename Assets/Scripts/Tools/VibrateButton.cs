using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VibrateButton : Button
{
	private new void Start()
	{
		onClick.AddListener(() => Vibration.Vibrate(50));
	}
}
