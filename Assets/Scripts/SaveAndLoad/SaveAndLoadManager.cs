using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveAndLoadManager : MonoBehaviour
{
	[HideInInspector]
	public bool LoadingComplete { get; private set; }

	private GameManager _game;
	private string _saveFilePath;

	private void Awake()
	{
		_saveFilePath = Application.persistentDataPath + "/gamesave.dat";
	}

	private void Start()
	{
		_game = GameManager.Instance;
	}

	// Create save game object and set its variables
	private Save CreateSaveGameObject()
	{
		Save save = new Save()
		{
			levelNumber = _game.levelNumber,
				diamonds = _game.DiamondNumber,
				vibrationEnabled = _game.VibrationEnabled,
				unlockedSkins = _game.unlockedSkins,
				selectedSkin = _game.currentSkin
		};

		return save;
	}

	public IEnumerator SaveGame()
	{
		yield return new WaitUntil(() => LoadingComplete);

		// Create save game object
		var save = CreateSaveGameObject();

		// Create and write save file
		var bf = new BinaryFormatter();
		var file = File.Create(_saveFilePath);
		bf.Serialize(file, save);

		// Close save file
		file.Close();

		Debug.Log("Game Saved");
		yield return null;
	}

	public void LoadGame()
	{
		LoadingComplete = false;

		// If save file exists, then we can load, else, game was never saved or save file was deleted
		if (File.Exists(_saveFilePath))
		{
			// Open and read save file
			var bf = new BinaryFormatter();
			var file = File.Open(_saveFilePath, FileMode.Open);
			var save = (Save) bf.Deserialize(file);

			// Close save file
			file.Close();

			_game.levelNumber = save.levelNumber;
			_game.DiamondNumber = save.diamonds;
			_game.VibrationEnabled = save.vibrationEnabled;
			_game.unlockedSkins = save.unlockedSkins;
			_game.currentSkin = save.selectedSkin;

			Debug.Log("Game Loaded");
		}
		else
		{
			_game.NoGameSavedResetValues();
			Debug.Log("No game saved!");
		}

		LoadingComplete = true;
	}
}
