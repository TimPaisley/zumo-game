using UnityEngine;
using InControl;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class ReadyUpMenu : VirtualScene {
    private int MIN_PLAYERS = 2;

    public Canvas canvas;
    public ReadyUpController baseControllerView;
    public PlayerController basePlayerController;
    public GameObject startIndicator;
    public GameManager gameManager;

    private RectTransform canvasTransform;
    private List<PlayerController> playerControllers;

	void Start () {
        canvasTransform = canvas.GetComponent<RectTransform>();
        playerControllers = new List<PlayerController>(InputManager.Devices.Count * 2 + 2);

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
                gameManager.inGameScene.Prepare(players);
                gameManager.inGameScene.Activate();
                gameObject.SetActive(false);
            }
        }
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
