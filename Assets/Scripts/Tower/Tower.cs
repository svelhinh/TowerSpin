using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
	public List<Floor> Floors { get; private set; }
	public float floorHeight { get; private set; }

	[HideInInspector] public int nbFloors = 3;
	[HideInInspector] public int limitFloorToHide;
	[HideInInspector] public float hiddenFloorsLimit;
	[HideInInspector] public int nbStartRemainingBlocks;
	[HideInInspector] public int nbRemainingBlocks;

	private GameManager _game;

	[SerializeField] private GameObject _block;
	[SerializeField] private float _radius = 2;
	[SerializeField] private int _nbVisibleBlocks = 8;
	[SerializeField] private int _nbBlocksPerFloor = 15;
	[SerializeField] private GameObject _confetti;
	private int _highestHiddenFloorNb;
	private bool _hasBeenCreatedBefore;

	private void Start()
	{
		_game = GameManager.Instance;

		_hasBeenCreatedBefore = false;
	}

	private void GenerateLevel()
	{
		Floors = new List<Floor>();

		for (int i = 0; i < nbFloors; i++)
		{
			Floor newFloor = GenerateFloor(_block, transform, _nbBlocksPerFloor, new Vector3(transform.position.x, transform.position.y + 0.6f * i, transform.position.z), 0.5f * i);
			newFloor.Nb = i;

			if (i > 0)
				newFloor.floorBelowNb = i - 1;

			Floors.Add(newFloor);
		}

		floorHeight = Floors[Floors.Count - 1]._blocks[0].transform.position.y - Floors[Floors.Count - 2]._blocks[0].transform.position.y;
	}

	private Floor GenerateFloor(GameObject go, Transform parent, int numberOfObjects, Vector3 positionOffset, float floorOffset = 0f)
	{
		Floor newFloor = new Floor();

		for (var i = 0; i < numberOfObjects; i++)
		{
			float angle = (i + floorOffset) * Mathf.PI * 2 / numberOfObjects;

			GameObject block = Instantiate(go, new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * _radius + positionOffset, Quaternion.identity, parent);
			TowerBlock towerBlock = block.GetComponent<TowerBlock>();

			towerBlock.baseColor = _game.levelGameColors[Random.Range(0, _game.levelGameColors.Count)];
			towerBlock.SetColor(towerBlock.baseColor);

			newFloor.AddBlock(towerBlock);
		}

		return newFloor;
	}

	public void EnableFloorsPhysics(bool value = true)
	{
		if (value)
		{
			foreach (Floor floor in Floors)
			{
				if (floor.Nb >= limitFloorToHide)
					floor.Enable(true, true, false);
			}
		}
		else
		{
			foreach (Floor floor in Floors)
				floor.Enable(false, true);
		}
	}

	public void HideFloors()
	{
		_highestHiddenFloorNb = 0;
		foreach (Floor floor in Floors)
		{
			if (floor.Nb < limitFloorToHide)
				floor.Enable(false);
			else
			{
				hiddenFloorsLimit = Floors[_highestHiddenFloorNb]._blocks[0].transform.position.y;
				break;
			}

			_highestHiddenFloorNb = floor.Nb;
		}
	}

	public void UnlockHighestHiddenFloor()
	{
		_game.mainCamera.Accelerate();

		_highestHiddenFloorNb--;
		if (_highestHiddenFloorNb >= -1)
		{
			Floors[_highestHiddenFloorNb + 1].Enable();
			if (_highestHiddenFloorNb > -1)
				hiddenFloorsLimit = Floors[_highestHiddenFloorNb]._blocks[0].transform.position.y;

			_game.mainCamera.GoDownOneFloor(floorHeight);
		}

		Vibration.Vibrate(50);
	}

	public IEnumerator CleanLastTower()
	{
		foreach (Floor floor in Floors)
			floor.Clean();

		yield return null;
	}

	public IEnumerator ResetValues()
	{
		if (_hasBeenCreatedBefore)
			yield return StartCoroutine(CleanLastTower());

		nbFloors = (int) _game.levelInfo.nbFloorsPerLevel.Evaluate(_game.levelNumber);
		limitFloorToHide = nbFloors - _nbVisibleBlocks;

		GenerateLevel();
		_hasBeenCreatedBefore = true;

		nbStartRemainingBlocks = (nbFloors - _game.nbFloorsMaxToWinLevel) * _nbBlocksPerFloor;
		nbRemainingBlocks = nbStartRemainingBlocks;
		EnableFloorsPhysics(false);
	}

	public void ShowConfetti()
	{
		_confetti.SetActive(true);
		Invoke("DisableConfetti", 2f);
	}

	private void DisableConfetti()
	{
		_confetti.SetActive(false);
	}
}
