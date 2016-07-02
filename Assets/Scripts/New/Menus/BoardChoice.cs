using UnityEngine;
using System.Linq;

namespace Zumo {
    class BoardChoice : MonoBehaviour {
		const float CHOICE_THRESHOLD = 0.7f;

		public Camera sceneCamera;
		public GameObject nextSceneText;

		GameManager gm;
		ChoosableBoard[] choosableBoards;

		void Awake () {
			gm = FindObjectOfType<GameManager>();
			choosableBoards = FindObjectsOfType<ChoosableBoard>();

			nextSceneText.gameObject.SetActive(false);
			sceneCamera.gameObject.SetActive(false);
		}

		void Start () {
			gm.cameraManager.Use(sceneCamera);
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

				if (gm.state.readyPlayers.Any(player => player.input.confirm.isPressed)) {
					gm.SwitchScene(gm.deathmatchScene);
				}
			}
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
