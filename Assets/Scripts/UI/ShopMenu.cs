using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopMenu : MonoBehaviour
{
	[HideInInspector] public List<Skin> lockedSkins;

	[SerializeField] private CurrentDiamonds _diamondsCounter;
	[SerializeField] private List<Skin> _skins;
	[SerializeField] private MeshFilter _skinPreview;
	[SerializeField] private VibrateButton _unlockButton;
	[SerializeField] private float _skinPreviewHorizontalSpeed = 1;

	private GameManager _game;
	private bool _noMoreLockedSkins;
	private int _lastSelectedId;

	private void Start()
	{
		_game = GameManager.Instance;
		lockedSkins = new List<Skin>(_skins);
		_lastSelectedId = -1;

		SelectSkin(_game.currentSkin);
	}

	private void OnEnable()
	{
		if (_game == null)
			_game = GameManager.Instance;

		SetUnlockButton();
	}

	private void Update()
	{
		if (Input.touchCount == 1)
		{
			Touch touch = Input.GetTouch(0);

			if (touch.phase == TouchPhase.Moved)
			{
				float h = _skinPreviewHorizontalSpeed * touch.deltaPosition.x;
				_skinPreview.transform.Rotate(0, -h, 0);
			}
		}
	}

	public void SelectSkin(int id)
	{
		if (_lastSelectedId > -1)
			_skins[_lastSelectedId].equipped.SetActive(false);

		if (id != _lastSelectedId)
		{
			_skins[id].equipped.SetActive(true);

			_lastSelectedId = id;

			_game.currentSkin = id;
			UpdatePreview();
		}
		else
		{
			_game.currentSkin = -1;
			_lastSelectedId = -1;
			UpdatePreview(true);
		}
	}

	public void ReturnToMainMenu()
	{
		_game.ui.ShowMainMenu();
	}

	public void UnlockRandomSkin()
	{
		int randomSkin = Random.Range(0, lockedSkins.Count);
		lockedSkins[randomSkin].LockSkin(false);

		_game.currentSkin = lockedSkins[randomSkin].id;
		_game.unlockedSkins.Add(lockedSkins[randomSkin].id);

		UpdatePreview();

		lockedSkins.RemoveAt(randomSkin);

		if (lockedSkins.Count == 0)
			_noMoreLockedSkins = true;

		SetUnlockButton();
		SelectSkin(_game.currentSkin);
	}

	private void SetUnlockButton()
	{
		_unlockButton.interactable = _game.DiamondNumber >= _game.skinsPrice && !_noMoreLockedSkins;

		_game.DiamondNumber -= _game.skinsPrice;
		_diamondsCounter.UpdateText();
	}

	private void UpdatePreview(bool defaultSkin = false)
	{
		_skinPreview.mesh = defaultSkin ? _game.defaultSkin : _game.skins[_game.currentSkin];

	}
}
