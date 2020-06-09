using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoadingState : GameBaseState
{
	private readonly GameManager game;

	public GameLoadingState(GameManager parent) : base(parent)
	{
		game = parent;
	}

	public override void EnterState()
	{
		game.saveAndLoad.LoadGame();
	}

	public override void Update()
	{
		if (game.saveAndLoad.LoadingComplete)
			game.TransitionToState(game.ResetState);
	}
}
