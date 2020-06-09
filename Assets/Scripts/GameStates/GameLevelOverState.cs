using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelOverState : GameBaseState
{
	private readonly GameManager _game;

	public GameLevelOverState(GameManager parent) : base(parent)
	{
		_game = parent;
	}

	public override void EnterState()
	{
		_game.StartCoroutine(EndLevel());
	}

	public override void Update() { }

	private IEnumerator EndLevel()
	{
		if (_game.hasWon)
		{
			_game.StartCoroutine(_game.mainCamera.Unzoom());
			_game.ui.levelMenu.PlayMessage(LevelMessage.LEVEL_COMPLETE);
			Vibration.Vibrate(1000);
			_game.tower.ShowConfetti();
		}
		else
		{
			_game.ui.levelMenu.PlayMessage(LevelMessage.TRY_AGAIN);
			Vibration.Vibrate(500);
		}

		yield return new WaitForSecondsRealtime(3f);

		yield return _game.ui.levelMenu.EndLevel();

		if (_game.hasWon)
			_game.levelNumber++;

		_game.StartCoroutine(_game.saveAndLoad.SaveGame());

		_game.TransitionToState(_game.ResetState);
	}
}
