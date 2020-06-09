using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondToAdd : MonoBehaviour
{
	private RectTransform _rect;

	public IEnumerator Move(Vector3 startPosition, Vector3 endPosition)
	{
		_rect = GetComponent<RectTransform>();

		float time = 0f;
		float duration = 2f;
		while (time <= duration)
		{
			time += Time.deltaTime;
			float normalizedValue = time / duration; // we normalize our time 

			_rect.anchoredPosition = Vector3.Lerp(startPosition, endPosition, normalizedValue);
			yield return null;
		}

		Destroy(gameObject);
	}
}
