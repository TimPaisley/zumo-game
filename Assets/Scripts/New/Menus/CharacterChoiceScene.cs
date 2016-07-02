using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Zumo {
	class CharacterChoiceScene : MonoBehaviour {
		const float CHOICE_THRESHOLD = 0.7f;

		public Camera sceneCamera;
		public ChooseableAnimal baseChooseableAnimal;
		public GameObject nextSceneText;

		GameManager gm;
		List<ChooseableAnimal> chooseableAnimals = new List<ChooseableAnimal>();

		void Awake () {
			gm = FindObjectOfType<GameManager>();

			nextSceneText.gameObject.SetActive(false);
			baseChooseableAnimal.gameObject.SetActive(false);
			sceneCamera.gameObject.SetActive(false);
		}

		void Start () {
			gm.cameraManager.Use(sceneCamera);
			setupChooseableAnimals();
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

		void setupChooseableAnimals () {
			var animals = FindObjectsOfType<AnimalController>();
			var angle = 0f;
			var angleIncrement = (Mathf.PI * 2) / animals.Length;

			foreach (var animal in animals) {
				chooseableAnimals.Add(buildChooseableAnimal(animal, angle));
				animal.gameObject.SetActive(false);

				angle += angleIncrement;
			}
		}

		ChooseableAnimal buildChooseableAnimal (AnimalController animal, float angle) {
			var chooseableAnimal = Instantiate(baseChooseableAnimal);

			chooseableAnimal.Setup(animal, angle);
			chooseableAnimal.transform.SetParent(baseChooseableAnimal.transform.parent, false);
			chooseableAnimal.gameObject.SetActive(true);

			return chooseableAnimal;
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
