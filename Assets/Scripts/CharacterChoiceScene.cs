using UnityEngine;
using InControl;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class CharacterChoiceScene : VirtualScene {
    public static readonly Vector2 NoPosition = new Vector2(-1, -1);

    public struct ChooserMoveOperation {
        public AnimalChooser chooser;
        public Vector2 currentPosition;
        public Vector2 newPosition;
    }

    public struct ChooserChooseOperation {
        public AnimalChooser chooser;
        public Vector2 currentPosition;
        public int animalIndex;
    }

    public GameObject sceneBase;
    public Transform noPositionMarker;
    public AnimalChooser baseChooser;

    public AnimalController[] animals;
    public GameObject[] animalChoiceIndicators;
    public int animalColumnCount = 2;
    public float pickerSpacing = 30;
    
    private GameManager gameManager;
    private CameraManager cameraManager;
    private Canvas canvas;
    private RectTransform canvasRect;

    private PlayerController[] players;
    private Dictionary<Vector2, List<AnimalChooser>> animalChoosers = new Dictionary<Vector2, List<AnimalChooser>>();
    private List<PlayerController> choiceMadePlayers = new List<PlayerController>();

    private List<ChooserMoveOperation> moveOperations = new List<ChooserMoveOperation>();
    private List<ChooserChooseOperation> chooseOperations = new List<ChooserChooseOperation>();

	void Start () {
        gameManager = FindObjectOfType<GameManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        canvas = sceneBase.GetComponentInChildren<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();

        baseChooser.gameObject.SetActive(false);
        sceneBase.SetActive(false);

        foreach (var indicator in animalChoiceIndicators) {
            indicator.SetActive(false);
        }
	}
	
	void Update () {
        moveOperations.Clear();
        chooseOperations.Clear();

        foreach (var choosers in animalChoosers) {
            var currentLocation = choosers.Key;

            foreach (var chooser in choosers.Value) {
                var desiredMoveLocation = chooser.DesiredMoveLocation(currentLocation);

                if (desiredMoveLocation != currentLocation) {
                    moveOperations.Add(
                        buildChooserMoveOperation(chooser, currentLocation, desiredMoveLocation)
                    );
                }

                if (chooser.player.input.dashButton.IsPressed) {
                    chooseOperations.Add(buildChooserChooseOperation(chooser, currentLocation));
                }
            }
        }

        foreach (var operation in moveOperations) {
            if (operation.chooser != null) {
                animalChoosers[operation.currentPosition].Remove(operation.chooser);

                if (!animalChoosers.ContainsKey(operation.newPosition)) {
                    animalChoosers.Add(operation.newPosition, new List<AnimalChooser>());
                }

                animalChoosers[operation.newPosition].Add(operation.chooser);
                recalculatePositions(operation.currentPosition);
                recalculatePositions(operation.newPosition);
            }
        }

        foreach (var operation in chooseOperations) {
            if (operation.chooser != null) {
                operation.chooser.player.animal = animals[operation.animalIndex];

                var indicator = animalChoiceIndicators[operation.animalIndex];
                indicator.SetActive(true);
                
                indicator.GetComponentInChildren<Text>().text = operation.chooser.player.playerName.ToUpper();
                indicator.GetComponentInChildren<Text>().color = operation.chooser.player.color;
                indicator.GetComponentInChildren<Image>().color = operation.chooser.player.color;

                choiceMadePlayers.Add(operation.chooser.player);
                animalChoosers[operation.currentPosition].Remove(operation.chooser);

                Destroy(operation.chooser.gameObject);
            }
        }

        if (choiceMadePlayers.Count == players.Length) {
            gameManager.inGameScene.Prepare(players);
            gameManager.inGameScene.Activate();
            Deactivate();
        }
	}

    public override void Prepare (PlayerController[] readyPlayers) {
        players = readyPlayers;
        choiceMadePlayers.Clear();

        foreach (var player in players) {
            player.animal = null;
        }

        var choosers = readyPlayers.Select(player => createAnimalChooser(player)).ToList();

        animalChoosers.Add(NoPosition, choosers);
        recalculatePositions(NoPosition);

        cameraManager.Use(cameraManager.tiltedMenuCamera, 0.5f);
    }

    public override void Activate () {
        base.Activate();

        sceneBase.SetActive(true);
    }

    public override void Deactivate () {
        base.Deactivate();

        sceneBase.SetActive(false);
    }

    private AnimalChooser createAnimalChooser (PlayerController player) {
        var chooser = Instantiate(baseChooser);
        chooser.MatchPlayer(player);
        chooser.transform.SetParent(canvas.transform, false);

        chooser.GetComponentInChildren<Text>().text = player.shortName;
        chooser.gameObject.SetActive(true);

        return chooser;
    }

    private void recalculatePositions(Vector2 position) {
        var choosers = animalChoosers[position];
        var chooserRect = baseChooser.GetComponent<RectTransform>().rect;
        
        var baseTransform = position == NoPosition ? noPositionMarker : animals[animalIndex(position)].transform;

        var viewportPos = cameraManager.mainCamera.WorldToViewportPoint(baseTransform.position);
        var baseScreenPos = new Vector2(viewportPos.x * canvasRect.sizeDelta.x, viewportPos.y * canvasRect.sizeDelta.y);

        var totalWidth = chooserRect.width * choosers.Count + pickerSpacing * (choosers.Count - 1);
        baseScreenPos.x -= totalWidth / 2;

        for (var i = 0; i < choosers.Count; i++) {
            var x = baseScreenPos.x + i * (chooserRect.width + pickerSpacing);
            choosers[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, baseScreenPos.y);
        }
    }

    private ChooserMoveOperation buildChooserMoveOperation(AnimalChooser chooser, Vector2 currentLocation, Vector2 desiredLocation) {
        if (desiredLocation.x < 0 || desiredLocation.y < 0 ||
                desiredLocation.x >= animalColumnCount || desiredLocation.y >= (animals.Length / animalColumnCount)) {
            return new ChooserMoveOperation();
        }

        return new ChooserMoveOperation { chooser = chooser, currentPosition = currentLocation, newPosition = desiredLocation };
    }

    private ChooserChooseOperation buildChooserChooseOperation(AnimalChooser chooser, Vector2 position) {
        var index = animalIndex(position);
        var animal = animals[index];

        foreach (var player in choiceMadePlayers) {
            if (player.animal == animal) {
                return new ChooserChooseOperation();
            }
        }

        return new ChooserChooseOperation { chooser = chooser, animalIndex = index, currentPosition = position };
    }
    
    private int animalIndex(Vector2 position) {
        return (int)(position.y * animalColumnCount + position.x);
    }
}
