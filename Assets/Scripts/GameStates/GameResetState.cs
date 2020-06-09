using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResetState : GameBaseState
{
	private readonly GameManager _game;

	public GameResetState(GameManager parent) : base(parent)
	{
		_game = parent;
	}

	public override void EnterState()
	{
		_game.StartCoroutine(ResetValues());
	}

	private IEnumerator ResetValues()
	{
		yield return _game.StartCoroutine(_game.ResetValues());

		_game.ui.ShowMainMenu();
		_game.ui.mainMenu.Enter();
	}

	public override void Update() { }
}
