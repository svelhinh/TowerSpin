using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusDiamondsToAdd : MonoBehaviour
{
	[SerializeField] private DiamondToAdd _diamond;
	[SerializeField] private RectTransform _target;

	private List<DiamondToAdd> _diamonds;

	private Vector3 _startPosition;
	private Vector3 _endPosition;

	public IEnumerator Add(int number)
	{
		if (_diamonds == null)
			_diamonds = new List<DiamondToAdd>();

		_startPosition = GetComponent<RectTransform>().position;
		_endPosition = _target.position;

		for (int i = 0; i < number; i++)
		{
			DiamondToAdd diamond = Instantiate(_diamond.gameObject).GetComponent<DiamondToAdd>();
			StartCoroutine(diamond.Move(_startPosition, _endPosition));
			_diamonds.Add(diamond);
		}

		while (!AllDiamondsReachedDestination())
			yield return null;

		_diamonds.Clear();
	}

	private bool AllDiamondsReachedDestination()
	{
		foreach (DiamondToAdd diamond in _diamonds)
		{
			if (diamond != null)
				return false;
		}

		return true;
	}
}
