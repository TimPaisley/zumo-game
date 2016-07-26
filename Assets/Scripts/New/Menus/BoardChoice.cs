using UnityEngine;
using System.Linq;
using System.Collections;

namespace Zumo {
    public class BoardChoice : MonoBehaviour {
		const float CHOICE_THRESHOLD = 0.7f;

		public Camera sceneCamera;
		public BoardChoiceSpinner choiceSpinner;
		public GameObject nextSceneText;

		GameManager gm;
		ChoosableBoard[] choosableBoards;
		Coroutine choosingBoard;

		void Awake () {
			gm = FindObjectOfType<GameManager>();
			choosableBoards = FindObjectsOfType<ChoosableBoard>();

			nextSceneText.gameObject.SetActive(false);
			sceneCamera.gameObject.SetActive(false);
		}

		void Start () {
			gm.cameraManager.Use(sceneCamera);
            gm.musicManager.Play(gm.musicManager.menuSong);
        }

		void Update () {
			foreach (var player in gm.state.readyPlayers) {
				var desiredBoard = findDesiredBoard(player);

				if (desiredBoard != null && !desiredBoard.VotedBy(player)) {
					unvote(player);
					desiredBoard.Vote(player);
				}
			}

			if (allPlayersChosen()) {
				nextSceneText.gameObject.SetActive(true);

				if (choosingBoard == null &&
						gm.state.readyPlayers.Any(player => player.input.confirm.isPressed)) {
					choosingBoard = StartCoroutine(chooseBoardAndContinue());
				}
			}
		}

		IEnumerator chooseBoardAndContinue () {
			var chosenBoard = chooseBoard();
			gm.state.chosenBoard = chosenBoard.board;

            foreach (var board in choosableBoards) {
                board.HideSelectionIndicator();
            }

			yield return choiceSpinner.SpinToBoard(chosenBoard);

            foreach (var board in choosableBoards) {
                board.Reset();
            }

            choosingBoard = null;
			gm.SwitchScene(gm.deathmatchScene);
		}

		ChoosableBoard chooseBoard () {
			var luckyPlayer = gm.state.readyPlayers.ElementAt(
				Random.Range(0, gm.state.readyPlayers.Count())
			);

			return choosableBoards.First(board => board.VotedBy(luckyPlayer));
		}

		ChoosableBoard findDesiredBoard (Player player) {
			var inputPosition = player.input.joystick.position;

			if (inputPosition.magnitude > CHOICE_THRESHOLD) {
				return choosableBoards.OrderBy(chooser => chooser.DistanceFrom(inputPosition)).First();
			}

			return null;
		}

		void unvote (Player player) {
			var existingChoice = choosableBoards.FirstOrDefault(board => board.VotedBy(player));

			if (existingChoice != null) {
				existingChoice.RemoveVote(player);
			}
		}

		bool allPlayersChosen () {
			return chosenPlayerCount() == gm.state.readyPlayers.Count();
		}

		int chosenPlayerCount () {
			return choosableBoards.Sum(board => board.votes);
		}
    }
}
