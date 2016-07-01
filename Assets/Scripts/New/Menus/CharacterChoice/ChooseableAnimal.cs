using System;
using UnityEngine;
using UnityEngine.UI;

namespace Zumo {
	class ChooseableAnimal : MonoBehaviour {
		public Image selectionIndicator;
		public float animalOffset = 4.5f;

		Vector2 basePosition;
		GameObject animalMesh;

		public AnimalController animal { get; private set; }

		public PlayerController player { get; private set; }

		public bool isChosen {
			get { return player != null; }
		}

		void Awake () {
			ResetChoice();
		}

		public void Setup (AnimalController newAnimal, float angle) {
			animal = newAnimal;
			animalMesh = Instantiate(animal.mesh);

			basePosition = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));

			selectionIndicator.transform.RotateAround(selectionIndicator.transform.position, Vector3.forward, angle * Mathf.Rad2Deg);

			animalMesh.transform.position = new Vector3(basePosition.y * animalOffset + 1, 0, basePosition.x * animalOffset);
			animalMesh.transform.RotateAround(animalMesh.transform.position, Vector3.up, angle * Mathf.Rad2Deg + 90);
		}

		public void Choose (PlayerController chosenPlayer) {
			player = chosenPlayer;
			player.chosenAnimal = animal;

			selectionIndicator.gameObject.SetActive(true);
			selectionIndicator.color = player.color;
		}

		public void ResetChoice () {
			player = null;

			selectionIndicator.gameObject.SetActive(false);
		}

		public float DistanceFrom (Vector2 point) {
			return Vector2.Distance(point, basePosition);
		}
	}
}
