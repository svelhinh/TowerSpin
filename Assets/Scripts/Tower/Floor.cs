using System.Collections.Generic;
using UnityEngine;

public class Floor
{
	public int Nb
	{
		get { return _nb; }
		set
		{
			_nb = value;

			foreach (TowerBlock block in _blocks)
				block.floorNb = value;
		}
	}
	public bool isOnTower;

	public List<TowerBlock> _blocks { get; private set; }

	public int floorBelowNb;

	private int _nb;

	public Floor()
	{
		isOnTower = true;
	}

	public void AddBlock(TowerBlock block)
	{
		if (_blocks == null)
			_blocks = new List<TowerBlock>();

		_blocks.Add(block);
	}

	public void Enable(bool value = true, bool onlyPhisics = false, bool withParticles = true)
	{
		foreach (TowerBlock block in _blocks)
			block.Enable(value, onlyPhisics, withParticles);
	}

	public void TryToUnlockHighestHiddenFloor()
	{
		bool floorHasABlockInTower = false;
		foreach (TowerBlock block in _blocks)
		{
			if (block != null && block.IsOnTower)
			{
				floorHasABlockInTower = true;
				return;
			}
		}

		if (!floorHasABlockInTower)
			isOnTower = false;

		GameManager.Instance.tower.UnlockHighestHiddenFloor();
	}

	public void Clean()
	{
		foreach (TowerBlock block in _blocks)
		{
			if (block != null)
				block.Clean();
		}

		_blocks.Clear();
	}
}
