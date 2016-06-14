using UnityEngine;
using InControl;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

public class BoardChoiceScene : VirtualScene {
    private class SpinnerSpin {
        private RectTransform baseSpinner;
        private RectTransform[] spinners;
        private List<float> pastFewAngles;
        private float finalAngle;
        private float startTime;
		private float endTime = -1;

        public SpinnerSpin(RectTransform spinner, float targetAngle) {
            baseSpinner = spinner;
            pastFewAngles = new List<float>(new float[] { 0, 0, 0, 0, 0 });
            finalAngle = 720f + targetAngle;
            startTime = Time.fixedTime;
            
            spinners = new RectTransform[] {
                baseSpinner, createSpinnerShadow(0.8f), createSpinnerShadow(0.6f), createSpinnerShadow(0.4f), createSpinnerShadow(0.2f)
            };
        }

        public bool inProgress {
			get { return endTime < 0 || endTime + 1 < Time.fixedTime; }
        }

        public void Update() {
            pastFewAngles.Insert(0, expoEase(0, finalAngle, Time.fixedTime - startTime));
            pastFewAngles.RemoveAt(pastFewAngles.Count - 1);

            for (var i = 0; i < spinners.Length; i++) {
                spinners[i].localEulerAngles = new Vector3(0, 0, pastFewAngles[i] % 360);
            }

			if (pastFewAngles.Last() >= finalAngle) {
				endTime = Time.fixedTime;
			}
        }

        public void Cleanup() {
            foreach (var spinner in spinners.Skip(1)) {
                Destroy(spinner.gameObject);
            }
			baseSpinner.gameObject.SetActive (false);
        }

        private RectTransform createSpinnerShadow (float opacity) {
            var shadow = Instantiate(baseSpinner);
            shadow.transform.SetParent(baseSpinner.parent, false);
            shadow.anchoredPosition = Vector2.zero;

            var image = shadow.GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, opacity);

            return shadow;
        }

        private float expoEase (float from, float to, float t) {
            var duration = 1f;
            var change = to - from;

            return (t >= duration) ?
                from + change :
                change * (-Mathf.Pow(2, -10 * t / duration) + 1) + from;
        }
    }

    public GameObject sceneBase;
    public RectTransform baseSelectionIndicator;
    public RectTransform selectionHelpIndicator;
    public float choiceCircleRadius;

	[Header("Boards")]
    public BoardController[] boards;
    public Transform[] boardPositions;
	public RectTransform[] boardMarkers;

	[Header("Voting")]
    public RectTransform baseVoteIndicator;
	public RectTransform voteIndicatorsCanvas;
    public RectTransform indicatorContainer;

	[Header("Other")]
    public RectTransform startGameButton;
	public RectTransform rouletteSpinner;
    public AudioSource voteSound;
	public AudioSource spinSound;

    private GameManager gameManager;
    private CameraManager cameraManager;
    private MenuBackgroundManager menuBackgroundManager;

    private BoardController[] boardPreviews;
    private PlayerController[] players;
    private Dictionary<PlayerController, RectTransform> playerSelectionIndicators = new Dictionary<PlayerController, RectTransform>();
    private Dictionary<PlayerController, BoardController> playerChoices = new Dictionary<PlayerController, BoardController>();
	private Dictionary<BoardController, RectTransform> boardVoteIndicators = new Dictionary<BoardController, RectTransform>();
    private SpinnerSpin spin;

    void Awake () {
        gameManager = FindObjectOfType<GameManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        menuBackgroundManager = FindObjectOfType<MenuBackgroundManager>();

        baseSelectionIndicator.gameObject.SetActive(false);
        startGameButton.gameObject.SetActive(false);
        rouletteSpinner.gameObject.SetActive(false);
        sceneBase.SetActive(false);

        boardPreviews = new BoardController[boards.Length];
        for (var i = 0; i < boards.Length; i++) {
            boards[i].gameObject.SetActive(false);
            boardPreviews[i] = createBoardPreview(boards[i], boardPositions[i].position);
        }

		baseVoteIndicator.gameObject.SetActive(false);
    }

    void Update () {
        if (spin != null) {
            if (spin.inProgress) {
                spin.Update();
            } else {
                spin.Cleanup();
                spin = null;

                foreach (var preview in boardPreviews) {
                    preview.gameObject.SetActive(false);
                }

                gameManager.currentBoard.gameObject.SetActive(true);
                gameManager.inGameScene.Prepare(players);
                gameManager.inGameScene.Activate();
                Deactivate();
            }

            return;
        }

        if (playerChoices.Count == players.Length && players.Any(player => player.input.actionButton.IsPressed)) {
            chooseBoard();

            return;
        }

        foreach (var indicator in playerSelectionIndicators) {
            var player = indicator.Key;
            var input = normalizedAxisInput(player);
            var desiredPosition = input * choiceCircleRadius;

            var closestMarker = boardMarkers
                .OrderBy(ind => Vector2.Distance(ind.anchoredPosition, desiredPosition))
                .First();

			indicator.Value.anchoredPosition = closestMarker.anchoredPosition * input.magnitude;

			if (input.magnitude == 1) {
				vote(player, closestMarker);
				indicator.Value.localScale = new Vector2 (2, 2);
				indicator.Value.gameObject.SetActive(true);
			} else {
				indicator.Value.localScale = new Vector2 (1, 1);
				indicator.Value.gameObject.SetActive(!playerChoices.ContainsKey(player));
			}
        }
    }

    public override void Prepare (PlayerController[] readyPlayers) {
        players = readyPlayers;

        foreach (var player in players) {
            playerSelectionIndicators[player] = createSelectionIndicator(player);
        }

        cameraManager.Use(cameraManager.tiltedMenuCamera, 0.5f);
    }

    public override void Activate () {
        base.Activate();

        menuBackgroundManager.ShowForBoardChoice();
        foreach (var preview in boardPreviews) {
            preview.gameObject.SetActive(true);
        }

        sceneBase.SetActive(true);
    }

    public override void Deactivate () {
        base.Deactivate();

		foreach (var indicator in playerSelectionIndicators.Values) {
			Destroy(indicator.gameObject);
		}

		foreach (var indicator in boardVoteIndicators.Values) {
			Destroy (indicator.gameObject);
		}

		playerSelectionIndicators.Clear ();
		boardVoteIndicators.Clear ();
		playerChoices.Clear ();


        sceneBase.SetActive(false);
    }

    private void vote (PlayerController player, RectTransform closestMarker) {
        var board = boards[Array.IndexOf(boardMarkers, closestMarker)];

        if (playerChoices.ContainsKey(player) && playerChoices[player] == board) {
            return;
        }

        if (playerChoices.ContainsKey(player)) {
            unvote(player);
        }

        playerChoices.Add(player, board);

        updateBoardVotes(board);
        voteSound.PlayOneShot(voteSound.clip);

        if (playerChoices.Count == players.Length) {
            startGameButton.gameObject.SetActive(true);
        }
    }

    private void unvote (PlayerController player) {
        var board = playerChoices[player];
        playerChoices.Remove(player);

        updateBoardVotes(board);
    }
    
    private void updateBoardVotes (BoardController board) {
		if (!boardVoteIndicators.ContainsKey(board)) {
			boardVoteIndicators.Add(board, createVoteIndicator(boardPositions[Array.IndexOf(boards, board)]));
		}

        var votes = playerChoices.Count(pair => pair.Value == board);
        var voteIndicator = boardVoteIndicators[board];

		voteIndicator.GetComponentInChildren<Text>().text = "x" + votes;

        if (votes > 0) {
            voteIndicator.gameObject.SetActive(true);
        } else {
            voteIndicator.gameObject.SetActive(false);
        }
    }

    private void chooseBoard () {
        startGameButton.gameObject.SetActive(false);
        rouletteSpinner.gameObject.SetActive(true);

        var board = playerChoices.Values.ElementAt(
            new System.Random().Next(0, playerChoices.Count)
        );
        var marker = boardMarkers[Array.IndexOf(boards, board)];

		var dy = marker.anchoredPosition.y - rouletteSpinner.anchoredPosition.y;
		var dx = marker.anchoredPosition.x - rouletteSpinner.anchoredPosition.x;
        var finalAngle = Mathf.Atan2(dy, dx) * (180 / Mathf.PI) + 270f;

        spin = new SpinnerSpin(rouletteSpinner, finalAngle);
        gameManager.currentBoard = Instantiate(board);

		spinSound.PlayOneShot(spinSound.clip);
    }

    private BoardController createBoardPreview (BoardController board, Vector3 position) {
        var preview = Instantiate(board);
        preview.transform.position = position;
        preview.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        return preview;
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
        rouletteSpinner.SetAsLastSibling();

        return indicator;
    }

	private RectTransform createVoteIndicator (Transform board) {
		var voteIndicator = Instantiate(baseVoteIndicator);
		var boardPos = cameraManager.mainCamera.WorldToViewportPoint(board.position);

		var screenPos = new Vector2(
			((boardPos.x * voteIndicatorsCanvas.sizeDelta.x) - (voteIndicatorsCanvas.sizeDelta.x * 0.5f)),
			((boardPos.y * voteIndicatorsCanvas.sizeDelta.y) - (voteIndicatorsCanvas.sizeDelta.y * 0.5f)) + 40
		);

		voteIndicator.SetParent(voteIndicatorsCanvas, false);
		voteIndicator.anchoredPosition = screenPos;
		voteIndicator.gameObject.SetActive(true);

		return voteIndicator;
	}

    private Vector2 normalizedAxisInput (PlayerController player) {
        var input = new Vector2(player.input.xAxis.Value, player.input.yAxis.Value);

        if (input.magnitude > 1) {
            return input.normalized;
        } else {
            return input;
        }
    }

    private Vector2 xzValues(Vector3 input) {
        return new Vector2(input.x, input.z);
    }
}
