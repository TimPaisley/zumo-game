using UnityEngine;
using InControl;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class CharacterChoiceScene : VirtualScene {
    private static readonly Vector2 NoPosition = new Vector2(-1, -1);

    public struct ChooserMoveOperation {
        public AnimalChooser chooser;
        public Vector2 currentPosition;
        public Vector2 newPosition;
    }

    public struct ChooserChooseOperation {
        public AnimalChooser chooser;
        public Vector2 currentPosition;
        public AnimalController animal;
    }

    public class AnimalChooser {
        public RectTransform picker;
        public PlayerController player;
        public AxisInput xAxisInput;
        public AxisInput yAxisInput;
        public bool hasChosen = false;

        public AnimalChooser (PlayerController player, RectTransform picker) {
            this.player = player;
            this.picker = picker;
            xAxisInput = new AxisInput(player.input.xAxis);
            yAxisInput = new AxisInput(player.input.yAxis);
        }

        public Vector2 desiredMoveLocation(Vector2 currentLocation) {
            var xMove = xAxisInput.CheckInput();
            var yMove = yAxisInput.CheckInput();

            // Special case where moving from NoPosition to a position
            if (currentLocation == NoPosition && yMove < 0) {
                return new Vector2(0, 0);
            }

            var newLocation = new Vector2(currentLocation.x, currentLocation.y);
            
            if (xMove < 0) {
                newLocation.x -= 1;
            } else if (xMove > 0) {
                newLocation.x += 1;
            }

            // Handle inverted y-axis
            if (yMove > 0) {
                newLocation.y -= 1;
            } else if (yMove < 0) {
                newLocation.y += 1;
            }

            return newLocation;
        }
    }

    public GameObject sceneBase;
    public Transform noPositionMarker;
    public RectTransform basePicker;

    public AnimalController[] animals;
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

        basePicker.gameObject.SetActive(false);
        sceneBase.SetActive(false);
	}
	
	void Update () {
        moveOperations.Clear();
        chooseOperations.Clear();

        foreach (var choosers in animalChoosers) {
            var currentLocation = choosers.Key;

            foreach (var chooser in choosers.Value) {
                var desiredMoveLocation = chooser.desiredMoveLocation(currentLocation);

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
                operation.chooser.player.animal = operation.animal;
                Destroy(operation.chooser.picker.gameObject);

                choiceMadePlayers.Add(operation.chooser.player);
                animalChoosers[operation.currentPosition].Remove(operation.chooser);
            }
        }
	}

    public void Prepare (PlayerController[] readyPlayers) {
        players = readyPlayers;
        choiceMadePlayers.Clear();

        foreach (var player in players) {
            player.animal = null;
        }

        var choosers = readyPlayers.Select(player => new AnimalChooser(player, createAnimalPicker())).ToList();

        animalChoosers.Add(NoPosition, choosers);
        recalculatePositions(NoPosition);
    }

    public override void Activate () {
        base.Activate();

        sceneBase.SetActive(true);
    }

    public override void Deactivate () {
        base.Deactivate();

        sceneBase.SetActive(false);
    }

    private RectTransform createAnimalPicker () {
        var picker = Instantiate(basePicker);
        picker.transform.SetParent(canvas.transform, false);
        picker.gameObject.SetActive(true);

        return picker;
    }

    private void recalculatePositions(Vector2 position) {
        var choosers = animalChoosers[position];
        
        var baseTransform = position == NoPosition ? noPositionMarker : animals[animalIndex(position)].transform;

        var viewportPos = cameraManager.mainCamera.WorldToViewportPoint(baseTransform.position);
        var baseScreenPos = new Vector2(
            viewportPos.x * canvasRect.sizeDelta.x,
            viewportPos.y * canvasRect.sizeDelta.y
        );

        var totalWidth = basePicker.rect.width * choosers.Count + pickerSpacing * (choosers.Count - 1);
        baseScreenPos.x -= totalWidth / 2;

        for (var i = 0; i < choosers.Count; i++) {
            var x = baseScreenPos.x + i * (basePicker.rect.width + pickerSpacing);
            choosers[i].picker.anchoredPosition = new Vector2(x, baseScreenPos.y);
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
        var animal = animals[animalIndex(position)];

        foreach (var player in choiceMadePlayers) {
            if (player.animal == animal) {
                return new ChooserChooseOperation();
            }
        }

        return new ChooserChooseOperation { chooser = chooser, animal = animal, currentPosition = position };
    }
    
    private int animalIndex(Vector2 position) {
        return (int)(position.y * animalColumnCount + position.x);
    }
}
