using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRunningState : GameBaseState
{
	private readonly GameManager _game;

	public GameRunningState(GameManager parent) : base(parent)
	{
		_game = parent;
	}

	public override void EnterState()
	{
		_game.canRemoveBlocksFromTower = true;
		_game.canThrowBall = true;

		_game.ui.ShowLevelMenu();

		_game.tower.HideFloors();
		_game.tower.EnableFloorsPhysics();

		_game.ShowNextBall();
		_game.mainCamera.isSpinning = true;

		Vibration.Vibrate(new long[] { 0, 200, 100 }, 2);

		if (_game.levelNumber == 0)
			_game.ui.callToAction.Play();
	}

	public override void Update()
	{
		if (_game.CurrentState == this && !_game.levelWillEnd)
		{
			if (_game.levelProgression >= 100)
			{
				_game.hasWon = true;
				_game.StartCoroutine(LevelWillEnd());
			}
			else if (_game.ballsLeft <= 0)
				_game.StartCoroutine(LevelWillEnd());
		}
	}

	private IEnumerator LevelWillEnd()
	{
		_game.canRemoveBlocksFromTower = false;
		_game.levelWillEnd = true;
		_game.canThrowBall = false;
		_game.mainCamera.isSpinning = false;

		if (_game.currentBall != null)
			PoolManager.Instance.CoolObject(_game.currentBall.gameObject, PoolObjectType.Ball);

		yield return _game.ui.levelMenu.WaitLevelEnd();

		_game.TransitionToState(_game.LevelOverState);
	}
}
