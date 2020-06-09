using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelIntroState : GameBaseState
{
	private readonly GameManager _game;

	public GameLevelIntroState(GameManager parent) : base(parent)
	{
		_game = parent;
	}

	public override void EnterState()
	{
		_game.StartCoroutine(IntroAnimation());
	}

	public override void Update() { }

	private IEnumerator IntroAnimation()
	{
		yield return _game.StartCoroutine(_game.ui.mainMenu.Disappear());

		_game.mainCamera.isSpinning = true;
		_game.mainCamera.speedMultiplier = _game.levelIntroRotationSpeed;

		Transform mainCameraTransform = _game.mainCamera.transform;
		float targetPositionY = mainCameraTransform.position.y + _game.tower.floorHeight * _game.tower.limitFloorToHide;

		while (mainCameraTransform.position.y < targetPositionY)
		{
			mainCameraTransform.position += Vector3.up * Time.deltaTime * _game.introSpeed * _game.tower.nbFloors * 0.05f;
			yield return null;
		}
		_game.TransitionToState(_game.RunningState);

		_game.mainCamera.speedMultiplier = 1f;
	}
}
