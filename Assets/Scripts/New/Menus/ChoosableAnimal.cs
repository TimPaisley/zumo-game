using UnityEngine;
using UnityEngine.UI;

namespace Zumo {
	class ChoosableAnimal : MonoBehaviour {
		public Image selectionIndicator;
		public Animal animal;

		Vector2 basePosition;

		public Player player { get; private set; }

		public bool isChosen {
			get { return player != null; }
		}

		void Awake () {
			animal.transform.SetParent(null);
			DontDestroyOnLoad(animal.gameObject);

			animal.gameObject.SetActive(false);
			selectionIndicator.gameObject.SetActive(false);

			basePosition = new Vector2(
				-Mathf.Sin(selectionIndicator.transform.localEulerAngles.z * Mathf.Deg2Rad),
				Mathf.Cos(selectionIndicator.transform.localEulerAngles.z * Mathf.Deg2Rad)
			);
		}

		public void Choose (Player chosenPlayer) {
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
