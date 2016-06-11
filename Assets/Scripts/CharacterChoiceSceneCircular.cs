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
            assignAnimals();

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
            player.animal = null;
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

        sceneBase.SetActive(false);
    }

    private void choose(PlayerController player, RectTransform closestAnimalIndicator) {
        var selectionIndicator = playerSelectionIndicators[player];

        if (playerChoiceIndicators.ContainsKey(player)) {
            unchoose(player);
        }
        
        if (!playerChoiceIndicators.ContainsValue(closestAnimalIndicator)) {
            closestAnimalIndicator.gameObject.SetActive(true);
            closestAnimalIndicator.GetComponentInChildren<Text>().text = player.shortName;
            closestAnimalIndicator.GetComponentInChildren<Image>().color = player.color;
            playerChoiceIndicators[player] = closestAnimalIndicator;

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
            playerChoiceIndicators[player].gameObject.SetActive(false);
            playerChoiceIndicators.Remove(player);
        }

        playerSelectionIndicators[player].anchoredPosition = Vector2.zero;
        playerSelectionIndicators[player].gameObject.SetActive(true);
    }

    private void assignAnimals() {
        foreach (var indicator in playerChoiceIndicators) {
            var index = Array.IndexOf(animalChoiceIndicators, indicator.Value);

            indicator.Key.animal = animals[index];
        }
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

    private Vector2 normalizedAxisInput(PlayerController player) {
        var input = new Vector2(player.input.xAxis.Value, player.input.yAxis.Value);
        
        if (input.magnitude > 1) {
            return input.normalized;
        } else {
            return input;
        }
    }
}
