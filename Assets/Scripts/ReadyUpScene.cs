using UnityEngine;
using InControl;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class ReadyUpScene : VirtualScene {
    private int MIN_PLAYERS = 2;

    public Canvas canvas;
    public ReadyUpController baseControllerView;
    public PlayerController basePlayerController;
    public GameObject startIndicator;
	public AudioSource nextSceneEffect;

	private GameManager gameManager;
    private MusicManager musicManager;
    private MenuBackgroundManager menuBackgroundManager;
    private RectTransform canvasTransform;
    private List<PlayerController> playerControllers;

	void Awake () {
		gameManager = FindObjectOfType<GameManager>();
        musicManager = FindObjectOfType<MusicManager>();
        menuBackgroundManager = FindObjectOfType<MenuBackgroundManager>();
        canvasTransform = canvas.GetComponent<RectTransform>();
        playerControllers = new List<PlayerController>(InputManager.Devices.Count * 2 + 2);

        canvas.enabled = false;
        startIndicator.SetActive(false);

        var yOffset = -9.5f;

	    for (var i = 0; i < InputManager.Devices.Count; i++) {
            createControllerAndPlayers(i, yOffset);

            yOffset -= 8;
        }

        if (Debug.isDebugBuild) {
            // Create hidden keyboard controller - helpful for testing
            createControllerAndPlayers(InputManager.Devices.Count, yOffset);
        }

        baseControllerView.gameObject.SetActive(false);
        basePlayerController.gameObject.SetActive(false);
	}
	
	void Update () {
        var players = readyPlayers();

        if (players.Length >= MIN_PLAYERS) {
            if (!startIndicator.activeSelf) {
                startIndicator.SetActive(true);
            }

            if (playerActionButtonPressed()) {
				nextSceneEffect.PlayOneShot(nextSceneEffect.clip);

                gameManager.characterChoiceScene.Prepare(players);
                gameManager.characterChoiceScene.Activate();
                Deactivate();
            }
        }
	}

    public override void Activate () {
        base.Activate();

        canvas.enabled = true;
        musicManager.Play(musicManager.menuSong);
        menuBackgroundManager.ShowForReadyUp();
    }

    public override void Deactivate () {
        base.Deactivate();

        canvas.enabled = false;
    }

    private void createControllerAndPlayers (int deviceIndex, float yOffset) {
        var controllerView = Instantiate(baseControllerView.gameObject);
        var viewTransform = controllerView.GetComponent<RectTransform>();
        var controllerComponent = controllerView.GetComponent<ReadyUpController>();

        viewTransform.SetParent(canvasTransform, false);
        viewTransform.localEulerAngles = Vector3.zero;
        viewTransform.anchoredPosition = new Vector2(0, yOffset);

        controllerComponent.basePlayerController = basePlayerController;
        controllerComponent.SetupPlayers(deviceIndex * 2);

        playerControllers.Add(controllerComponent.leftPlayer);
        playerControllers.Add(controllerComponent.rightPlayer);
    }

    private PlayerController[] readyPlayers() {
        return playerControllers.Where(player => player.isReady).ToArray();
    }

    private bool playerActionButtonPressed() {
        foreach (var player in playerControllers) {
            if (player.input.actionButton.IsPressed) {
                return true;
            }
        }

        return false;
    }
}
