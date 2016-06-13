using UnityEngine;
using InControl;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

public class CharacterChoiceSceneCircular : VirtualScene {
    public GameObject sceneBase;
    public RectTransform baseSelectionIndicator;
    public RectTransform selectionHelpIndicator;
    public float choiceCircleRadius;

    public AnimalController[] animals;
    public RectTransform[] animalChoiceIndicators;
    public RectTransform indicatorContainer;
	public RectTransform baseChoiceIndicator;
	public RectTransform choiceIndicatorsCanvas;
    public RectTransform startGameButton;
    
    private GameManager gameManager;
    private CameraManager cameraManager;
    private MenuBackgroundManager menuBackgroundManager;

    private PlayerController[] players;
    private Dictionary<PlayerController, RectTransform> playerSelectionIndicators = new Dictionary<PlayerController, RectTransform>();
    private Dictionary<PlayerController, RectTransform> playerChoiceIndicators = new Dictionary<PlayerController, RectTransform>();

	void Awake () {
        gameManager = FindObjectOfType<GameManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        menuBackgroundManager = FindObjectOfType<MenuBackgroundManager>();

        baseSelectionIndicator.gameObject.SetActive(false);
		baseChoiceIndicator.gameObject.SetActive(false);
        startGameButton.gameObject.SetActive(false);
        sceneBase.SetActive(false);

        foreach (var indicator in animalChoiceIndicators) {
            indicator.gameObject.SetActive(false);
        }
	}
	
	void Update () {
        foreach (var indicator in playerSelectionIndicators) {
            var player = indicator.Key;
            var input = normalizedAxisInput(player);
            var desiredPosition = input * choiceCircleRadius;

            var closestAnimalIndicator = animalChoiceIndicators
                .OrderBy(ind => Vector2.Distance(ind.anchoredPosition, desiredPosition))
                .First();

            indicator.Value.anchoredPosition = closestAnimalIndicator.anchoredPosition * input.magnitude;

            if (input.magnitude == 1) {
                choose(player, closestAnimalIndicator);
            }
        }

        if (playerChoiceIndicators.Count == players.Length && players.Any(player => player.input.actionButton.IsPressed)) {
            if (gameManager.boardChoiceScene) { //TODO board choice
                gameManager.boardChoiceScene.Prepare(players);
                gameManager.boardChoiceScene.Activate();
            } else {
                gameManager.inGameScene.Prepare(players);
                gameManager.inGameScene.Activate();
            }

            Deactivate();
        }
	}

    public override void Prepare (PlayerController[] readyPlayers) {
        players = readyPlayers;

        foreach (var player in players) {
            player.baseAnimal = null;
            playerSelectionIndicators[player] = createSelectionIndicator(player);
        }

        cameraManager.Use(cameraManager.tiltedMenuCamera, 0.5f);
    }

    public override void Activate () {
        base.Activate();

        menuBackgroundManager.ShowForCharacterChoice();
        sceneBase.SetActive(true);
    }

    public override void Deactivate () {
        base.Deactivate();

		foreach (var indicator in playerSelectionIndicators.Values) {
			Destroy(indicator.gameObject);
		}

		playerSelectionIndicators.Clear();

        sceneBase.SetActive(false);
    }

    private void choose(PlayerController player, RectTransform closestAnimalIndicator) {
        var selectionIndicator = playerSelectionIndicators[player];
		var chosenAnimal = animals[Array.IndexOf(animalChoiceIndicators, closestAnimalIndicator)];

		if (chosenAnimal == player.animal) {
			return;
		}

		if (playerChoiceIndicators.ContainsKey(player)) {
            unchoose(player);
        }

		player.baseAnimal = chosenAnimal;
        
        if (!playerChoiceIndicators.ContainsValue(closestAnimalIndicator)) {
            closestAnimalIndicator.gameObject.SetActive(true);
			closestAnimalIndicator.GetComponentInChildren<Image>().color = player.color;

			var choiceIndicator = createChoiceIndicator(closestAnimalIndicator);
			choiceIndicator.GetComponentInChildren<Text>().color = player.color;
            choiceIndicator.GetComponentInChildren<Text>().text = player.shortName;
            playerChoiceIndicators[player] = choiceIndicator;

            selectionIndicator.gameObject.SetActive(false);
        }

        if (playerChoiceIndicators.Count == players.Length) {
            startGameButton.gameObject.SetActive(true);
        } else {
            startGameButton.gameObject.SetActive(false);
        }
    }

    private void unchoose (PlayerController player) {
		if (playerChoiceIndicators.ContainsKey(player)) {
			Destroy(playerChoiceIndicators[player].gameObject);
            playerChoiceIndicators.Remove(player);

			animalChoiceIndicators[Array.IndexOf(animals, player.baseAnimal)].gameObject.SetActive(false);
        }

		player.baseAnimal = null;

        playerSelectionIndicators[player].anchoredPosition = Vector2.zero;
        playerSelectionIndicators[player].gameObject.SetActive(true);
    }

    private RectTransform createSelectionIndicator (PlayerController player) {
        var indicator = Instantiate(baseSelectionIndicator);
        indicator.gameObject.SetActive(true);
        indicator.transform.SetParent(indicatorContainer.transform, false);
        indicator.anchoredPosition = Vector2.zero;

        indicator.GetComponentInChildren<Text>().text = player.shortName;
        indicator.GetComponentInChildren<Image>().color = player.color;
        indicator.gameObject.SetActive(true);

        selectionHelpIndicator.SetAsLastSibling();

        return indicator;
    }

	private RectTransform createChoiceIndicator (RectTransform animalIndicator) {
		var choiceIndicator = Instantiate(baseChoiceIndicator);
		var animalPos = cameraManager.mainCamera.WorldToViewportPoint(animalIndicator.transform.position);

		var screenPos = new Vector2(
			((animalPos.x * choiceIndicatorsCanvas.sizeDelta.x) - (choiceIndicatorsCanvas.sizeDelta.x * 0.5f)),
			((animalPos.y * choiceIndicatorsCanvas.sizeDelta.y) - (choiceIndicatorsCanvas.sizeDelta.y * 0.5f)) + 40
		);

		choiceIndicator.SetParent(choiceIndicatorsCanvas, false);
		choiceIndicator.anchoredPosition = screenPos;
		choiceIndicator.gameObject.SetActive(true);

		return choiceIndicator;
	}

    private Vector2 normalizedAxisInput(PlayerController player) {
        var input = new Vector2(player.input.xAxis.Value, player.input.yAxis.Value);
        
        if (input.magnitude > 1) {
            return input.normalized;
        } else {
            return input;
        }
    }
}
