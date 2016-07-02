using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Zumo {
	class AnimalChoice : MonoBehaviour {
		const float CHOICE_THRESHOLD = 0.7f;

		public Camera sceneCamera;
		public GameObject nextSceneText;

		GameManager gm;
		ChoosableAnimal[] choosableAnimals;

		void Awake () {
			gm = FindObjectOfType<GameManager>();
			choosableAnimals = FindObjectsOfType<ChoosableAnimal>();

			nextSceneText.gameObject.SetActive(false);
			sceneCamera.gameObject.SetActive(false);
		}

		void Start () {
			gm.cameraManager.Use(sceneCamera);
		}

		void Update () {
			foreach (var player in gm.state.readyPlayers) {
				var desiredAnimal = findDesiredAnimal(player);

				if (desiredAnimal != null && !desiredAnimal.isChosen) {
					unchoose(player);
					desiredAnimal.Choose(player);
				}
			}

			if (allPlayersChosen()) {
				nextSceneText.gameObject.SetActive(true);

				if (gm.state.readyPlayers.Any(player => player.input.confirm.isPressed)) {
					gm.SwitchScene(gm.boardChoiceScene);
				}
			}
		}

		ChoosableAnimal findDesiredAnimal (Player player) {
			var inputPosition = player.input.joystick.position;

			if (inputPosition.magnitude > CHOICE_THRESHOLD) {
				return choosableAnimals.OrderBy(chooser => chooser.DistanceFrom(inputPosition)).First();
			}

			return null;
		}

		void unchoose (Player player) {
			var existingChoice = choosableAnimals.FirstOrDefault(animal => animal.player == player);

			if (existingChoice != null) {
				existingChoice.ResetChoice();
			}
		}

		bool allPlayersChosen () {
			return chosenPlayerCount() == gm.state.readyPlayers.Count();
		}

		int chosenPlayerCount () {
			return choosableAnimals.Count(animal => animal.player != null);
		}
	}
}
