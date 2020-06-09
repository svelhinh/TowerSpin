using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum LevelMessage
{
	[StringValue("tryAgain")]
	TRY_AGAIN, [StringValue("levelComplete")]
	LEVEL_COMPLETE, [StringValue("oneBallLeft")]
	ONE_BALL_LEFT,
}

public class LevelMenu : MonoBehaviour
{
	public CurrentBallNumber currentBallNb;
	public LevelProgression levelProgression;

	[SerializeField] private Image _overlay;
	[SerializeField] private GameObject _waitLevelEnd;
	[SerializeField] private Image _waitLevelEndImage;
	[SerializeField] private GameObject _bonusDiamonds;
	[SerializeField] private TMP_Text _bonusDiamondsText;
	[SerializeField] private BonusDiamondsToAdd _bonusDiamondsToAdd;
	[SerializeField] private GameObject _diamondsCounter;
	[SerializeField] private GameObject _ballNb;
	[SerializeField] private List<GameObject> _messages;
	[SerializeField] private Animator _comboAnim;
	[SerializeField] private TMP_Text _comboText;

	private GameManager _game;

	private void Start()
	{
		_game = GameManager.Instance;
	}

	public IEnumerator WaitLevelEnd()
	{
		_ballNb.SetActive(false);
		_comboAnim.gameObject.SetActive(false);

		_waitLevelEnd.gameObject.SetActive(true);

		float duration = GameManager.Instance.waitLevelEndTime;
		float time = 0f;
		while (time <= duration)
		{
			time += Time.deltaTime;
			_waitLevelEndImage.fillAmount = Mathf.Clamp01(time / duration);

			if (_game.levelProgression >= 100)
			{
				_game.hasWon = true;
				break;
			}

			yield return null;
		}
		_waitLevelEnd.gameObject.SetActive(false);
	}

	public IEnumerator EndLevel()
	{
		yield return _game.ui.imageFader.FadeOut(_overlay);

		int diamonds = Mathf.FloorToInt(_game.levelProgression * 0.1f);

		_diamondsCounter.SetActive(true);
		yield return new WaitForSeconds(1f);

		_bonusDiamonds.SetActive(true);
		_bonusDiamondsText.text = $"+{diamonds}";

		_bonusDiamondsToAdd.gameObject.SetActive(true);
		// yield return _bonusDiamondsToAdd.Add(diamonds);

		_diamondsCounter.GetComponent<Animator>().SetTrigger("CallToAction");
		_game.DiamondNumber += diamonds;
		_diamondsCounter.GetComponent<CurrentDiamonds>().UpdateText();

		Vibration.Vibrate(150);

		yield return new WaitForSeconds(2f);

		_overlay.gameObject.SetActive(false);
		_bonusDiamonds.SetActive(false);
		_bonusDiamondsToAdd.gameObject.SetActive(false);
		_diamondsCounter.SetActive(false);
	}

	private void OnEnable()
	{
		_ballNb.SetActive(true);
		_comboAnim.gameObject.SetActive(true);
	}

	public void PlayMessage(LevelMessage message)
	{
		_messages[(int) message].GetComponent<Animator>().SetTrigger(message.GetStringValue());
	}

	public void ShowCombo()
	{
		_comboText.text = _game.ComboCounter.ToString();
		_comboText.color = _game.currentBall.GetColor();
		_comboAnim.SetTrigger("Appear");
	}
}
