using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameColor
{
	RED,
	GREEN,
	BLUE,
	PURPLE,
	YELLOW,
	BLACK,
	REAL_BLACK,
}

[Serializable]
public class GameColorInfo
{
	public Color color;
	public GameColor name;
}

[Serializable]
public class LevelInfo
{
	public AnimationCurve nbFloorsPerLevel;
	public AnimationCurve nbColorsPerLevel;
	public AnimationCurve nbBallsPerLevel;
}

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	public CameraManager mainCamera;
	public Tower tower;
	public SaveAndLoadManager saveAndLoad;
	public UIManager ui;


	[Header("Game Infos")]
	public int targetFrameRate = 60;
	public float introSpeed = 0.2f;
	public float levelIntroRotationSpeed = 10f;
	public List<GameColorInfo> gameColors;
	public int nbFloorsMaxToWinLevel = 3;
	public LevelInfo levelInfo;
	public List<Mesh> skins;
	public Mesh defaultSkin;
	public int skinsPrice = 150;
	public float waitLevelEndTime = 5f;
	[SerializeField] private int _comboToActivateSuperBall = 5;


	public int ballsLeft { get; private set; }

	[HideInInspector] public int totalBalls;
	[HideInInspector] public int levelNumber;
	[HideInInspector] public Ball currentBall;
	[HideInInspector] public GameColorInfo lastBallColor;
	[HideInInspector] public List<GameColorInfo> validBallColors;
	[HideInInspector] public List<int> unlockedSkins;
	[HideInInspector] public int currentSkin;
	[HideInInspector] public bool canThrowBall;
	[HideInInspector] public bool activateSuperBall;
	[HideInInspector] public float levelProgression;
	[HideInInspector] public List<GameColorInfo> levelGameColors;
	[HideInInspector] public bool hasWon;
	[HideInInspector] public bool levelWillEnd;
	[HideInInspector] public bool canRemoveBlocksFromTower;

	[HideInInspector]
	public int DiamondNumber
	{
		get { return _diamondNumber; }
		set
		{
			if (_diamondNumber == value) return;

			_diamondNumber = value;

			StartCoroutine(saveAndLoad.SaveGame());
		}
	}

	[HideInInspector]
	public bool VibrationEnabled
	{
		get { return _vibrationEnabled; }
		set
		{
			if (_vibrationEnabled == value) return;

			_vibrationEnabled = value;

			StartCoroutine(saveAndLoad.SaveGame());
		}
	}
	public int ComboCounter { get; private set; }

	#region GameStates

	public GameBaseState LastState { get; private set; }
	public GameBaseState CurrentState { get; private set; }

	[HideInInspector] public GameLoadingState LoadingState;
	[HideInInspector] public GameResetState ResetState;
	[HideInInspector] public GameLevelIntroState LevelIntroState;
	[HideInInspector] public GameRunningState RunningState;
	[HideInInspector] public GameLevelOverState LevelOverState;

	#endregion

	private int _diamondNumber;
	private bool _vibrationEnabled;
	private GameObject _lastBall;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);

			Application.targetFrameRate = targetFrameRate;

			LoadingState = new GameLoadingState(this);
			ResetState = new GameResetState(this);
			LevelIntroState = new GameLevelIntroState(this);
			RunningState = new GameRunningState(this);
			LevelOverState = new GameLevelOverState(this);

			levelGameColors = new List<GameColorInfo>();
		}
		else
			Destroy(this);
	}

	public void TransitionToState(GameBaseState state)
	{
		LastState = CurrentState;
		CurrentState = state;
		CurrentState.EnterState();
	}

	private void Start()
	{
		TransitionToState(LoadingState);
	}

	private void Update()
	{
		if (CurrentState != null)
			CurrentState.Update();
	}

	public void StartGame()
	{
		TransitionToState(LevelIntroState);
	}

	public void NoGameSavedResetValues()
	{
		levelNumber = 0;
		_diamondNumber = 0;
		_vibrationEnabled = true;
		unlockedSkins = new List<int>();
		currentSkin = -1;
	}

	public void ShowNextBall(bool hideImmediately = true)
	{
		_lastBall = null;
		if (currentBall != null)
			_lastBall = currentBall.gameObject;

		if (ballsLeft > 0)
		{
			currentBall = PoolManager.Instance.GetPoolObject(PoolObjectType.Ball).GetComponent<Ball>();
			currentBall.ResetBall();
		}

		if (_lastBall != null)
			Invoke("HideLastBall", hideImmediately ? 0f : 1f);

		canThrowBall = ballsLeft > 0;
	}

	public void HideLastBall()
	{
		PoolManager.Instance.CoolObject(_lastBall, PoolObjectType.Ball);
	}

	public IEnumerator ThrowBall(GameObject target)
	{
		if (currentBall.ColorName != GameColor.BLACK)
			ui.levelMenu.levelProgression.TransitionColor();

		yield return StartCoroutine(currentBall.Throw(target.transform.position));

		ballsLeft--;

		if (ballsLeft == 1)
			ui.levelMenu.PlayMessage(LevelMessage.ONE_BALL_LEFT);
		ui.levelMenu.currentBallNb.UpdateText();

		if (target.tag == "towerBlock")
		{
			TowerBlock block = target.GetComponent<TowerBlock>();

			if (block.colorInfo.name == currentBall.ColorName || currentBall.ColorName == GameColor.BLACK)
			{
				tower.Floors[block.floorNb].TryToUnlockHighestHiddenFloor();
				Destroy(target);
				ShowNextBall();

				if (currentBall.ColorName != GameColor.BLACK)
					AddCombo();

				Vibration.Vibrate(50);
				mainCamera.Shake();
			}
			else
			{
				ResetCombo();
				currentBall.rb.useGravity = true;
				currentBall.rb.AddForce((mainCamera.transform.position - currentBall.transform.position) * 0.1f);
				ShowNextBall(false);

				Vibration.Vibrate(100);
			}

			if (!levelWillEnd)
				mainCamera.isSpinning = true;
		}
	}

	private void AddCombo()
	{
		ComboCounter++;

		ui.levelMenu.ShowCombo();

		if (ComboCounter >= _comboToActivateSuperBall)
		{
			activateSuperBall = true;
			ComboCounter = 0;
		}
	}

	private void ResetCombo()
	{
		ComboCounter = 0;
		activateSuperBall = false;
	}

	public IEnumerator ResetValues()
	{
		levelWillEnd = false;

		mainCamera.isSpinning = false;
		ResetCombo();

		levelGameColors.Clear();
		levelGameColors = new List<GameColorInfo>(gameColors);

		int nbColorsToRemove = levelGameColors.Count - 1 - Mathf.FloorToInt(levelInfo.nbColorsPerLevel.Evaluate(levelNumber));
		for (int i = 0; i < nbColorsToRemove; i++)
			levelGameColors.RemoveAt(UnityEngine.Random.Range(0, levelGameColors.Count - 1));

		lastBallColor = null;
		validBallColors = new List<GameColorInfo>();

		totalBalls = (int) levelInfo.nbBallsPerLevel.Evaluate(levelNumber);
		ballsLeft = totalBalls;

		currentBall = null;

		levelProgression = 0;
		hasWon = false;

		foreach (GameColorInfo gameColor in levelGameColors)
		{
			if (gameColor.name != GameColor.BLACK)
				validBallColors.Add(gameColor);
		}

		yield return StartCoroutine(tower.ResetValues());

		mainCamera.ResetValues();
	}
}
