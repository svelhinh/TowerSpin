using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	[Header("Menus")]
	public MainMenu mainMenu;
	public LevelMenu levelMenu;
	public ShopMenu shopMenu;

	[Space]
	public GameObject ballNb;
	public ImageFader imageFader;
	public CallToAction callToAction;

	[SerializeField] private GameObject _shopCamera;
	[SerializeField] private GameObject _newSkin;

	private GameManager _game;

	private void Start()
	{
		_game = GameManager.Instance;
	}

	public void ShowMainMenu()
	{
		levelMenu.gameObject.SetActive(false);
		shopMenu.gameObject.SetActive(false);
		_shopCamera.SetActive(false);
		ballNb.SetActive(false);

		_game.mainCamera.gameObject.SetActive(true);
		mainMenu.gameObject.SetActive(true);

		if (_game.DiamondNumber >= _game.skinsPrice && shopMenu.lockedSkins.Count > 0)
			_newSkin.SetActive(true);
	}

	public void ShowLevelMenu()
	{
		levelMenu.gameObject.SetActive(true);
	}

	public void ShowShopMenu()
	{
		mainMenu.gameObject.SetActive(false);
		_game.mainCamera.gameObject.SetActive(false);
		_newSkin.SetActive(false);

		_shopCamera.SetActive(true);
		shopMenu.gameObject.SetActive(true);
	}

	public void ShowBallNb()
	{
		ballNb.SetActive(true);
	}
}
