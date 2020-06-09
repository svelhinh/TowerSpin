using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Utils
{
	public static void SetKinematic(Rigidbody rigidbody, bool value = true, CollisionDetectionMode collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic)
	{
		if (value)
		{
			rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
			rigidbody.isKinematic = true;
		}
		else
		{
			rigidbody.isKinematic = false;
			rigidbody.collisionDetectionMode = collisionDetectionMode;
		}
	}

	public static IEnumerator ImageColorTransition(Image image, Color colorA, Color colorB, float speed = 1, float overrideAlpha = -1)
	{
		if (overrideAlpha != -1)
			colorB.a = overrideAlpha;

		float t = 0f;
		while (image.color != colorB)
		{
			image.color = Color.Lerp(colorA, colorB, t);
			t += Time.deltaTime * speed;
			yield return null;
		}
	}
}
