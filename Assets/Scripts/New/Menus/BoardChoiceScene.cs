using UnityEngine;
using System.Linq;

namespace Zumo {
    class BoardChoiceScene : MonoBehaviour {
		const float CHOICE_THRESHOLD = 0.7f;

		public Camera sceneCamera;
		public GameObject nextSceneText;

		GameManager gm;
		ChooseableBoard[] chooseableBoards;

		void Awake () {
			gm = FindObjectOfType<GameManager>();
			chooseableBoards = FindObjectsOfType<ChooseableBoard>();

			nextSceneText.gameObject.SetActive(false);
			sceneCamera.gameObject.SetActive(false);
		}

		void Start () {
			gm.cameraManager.Use(sceneCamera);
		}

		void Update () {
			foreach (var player in gm.readyPlayers) {
				var desiredBoard = findDesiredBoard(player);

				if (desiredBoard != null && !desiredBoard.VotedBy(player)) {
					unvote(player);
					desiredBoard.Vote(player);
				}
			}

			if (allPlayersChosen()) {
				nextSceneText.gameObject.SetActive(true);

				if (gm.readyPlayers.Any(player => player.input.confirm.isPressed)) {
					gm.SwitchScene(gm.deathmatchScene);
				}
			}
		}

		ChooseableBoard findDesiredBoard (PlayerController player) {
			var inputPosition = player.input.joystick.position;

			if (inputPosition.magnitude > CHOICE_THRESHOLD) {
				return chooseableBoards.OrderBy(chooser => chooser.DistanceFrom(inputPosition)).First();
			}

			return null;
		}

		void unvote (PlayerController player) {
			var existingChoice = chooseableBoards.FirstOrDefault(board => board.VotedBy(player));

			if (existingChoice != null) {
				existingChoice.RemoveVote(player);
			}
		}

		bool allPlayersChosen () {
			return chosenPlayerCount() == gm.readyPlayers.Count();
		}

		int chosenPlayerCount () {
			return chooseableBoards.Sum(board => board.votes);
		}
    }
}
