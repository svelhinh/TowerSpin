using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageFader : MonoBehaviour
{
	public AnimationCurve curve;

	public IEnumerator FadeOut(Image img, AnimationCurve usedCurve = null)
	{
		if (usedCurve == null)
			usedCurve = curve;

		img.gameObject.SetActive(true);

		// Evaluate curve at time t to change image alpha over time
		var t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime;
			var a = usedCurve.Evaluate(t);
			img.color = new Color(img.color.r, img.color.g, img.color.b, a);
			yield return 0;
		}
	}
}
