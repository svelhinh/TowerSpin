using UnityEngine;

public abstract class GameBaseState
{
	protected GameManager parent;

	protected GameBaseState(GameManager parent)
	{
		this.parent = parent;
	}

	public abstract void EnterState();
	public abstract void Update();
}
