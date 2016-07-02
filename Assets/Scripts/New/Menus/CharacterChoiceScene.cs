using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Zumo {
	class CharacterChoiceScene : MonoBehaviour {
		const float CHOICE_THRESHOLD = 0.7f;

		public Camera sceneCamera;
		public GameObject nextSceneText;

		GameManager gm;
		ChooseableAnimal[] chooseableAnimals;

		void Awake () {
			gm = FindObjectOfType<GameManager>();
			chooseableAnimals = FindObjectsOfType<ChooseableAnimal>();

			nextSceneText.gameObject.SetActive(false);
			sceneCamera.gameObject.SetActive(false);
		}

		void Start () {
			gm.cameraManager.Use(sceneCamera);
		}

		void Update () {
			foreach (var player in gm.readyPlayers) {
				var desiredAnimal = findDesiredAnimal(player);

				if (desiredAnimal != null && !desiredAnimal.isChosen) {
					unchoose(player);
					desiredAnimal.Choose(player);
				}
			}

			if (allPlayersChosen()) {
				nextSceneText.gameObject.SetActive(true);

				if (gm.readyPlayers.Any(player => player.input.confirm.isPressed)) {
					gm.SwitchScene(gm.boardChoiceScene);
				}
			}
		}

		ChooseableAnimal findDesiredAnimal (PlayerController player) {
			var inputPosition = player.input.joystick.position;

			if (inputPosition.magnitude > CHOICE_THRESHOLD) {
				return chooseableAnimals.OrderBy(chooser => chooser.DistanceFrom(inputPosition)).First();
			}

			return null;
		}

		void unchoose (PlayerController player) {
			var existingChoice = chooseableAnimals.FirstOrDefault(animal => animal.player == player);

			if (existingChoice != null) {
				existingChoice.ResetChoice();
			}
		}

		bool allPlayersChosen () {
			return chosenPlayerCount() == gm.readyPlayers.Count();
		}

		int chosenPlayerCount () {
			return chooseableAnimals.Count(animal => animal.player != null);
		}
	}
}
