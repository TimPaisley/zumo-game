using UnityEngine;
using UnityEngine.UI;

namespace Zumo {
	public class ChoosableAnimal : MonoBehaviour {
		public Image selectionIndicator;
		public Animal animal;

		GameState state;
		Vector2 basePosition;

		public Player player { get; private set; }

		public bool isChosen {
			get { return player != null; }
		}

		void Awake () {
			state = FindObjectOfType<GameManager>().state;

			animal.transform.SetParent(null);
			animal.gameObject.SetActive(false);
			DontDestroyOnLoad(animal.gameObject);

			selectionIndicator.gameObject.SetActive(false);

			basePosition = new Vector2(
				-Mathf.Sin(selectionIndicator.transform.localEulerAngles.z * Mathf.Deg2Rad),
				Mathf.Cos(selectionIndicator.transform.localEulerAngles.z * Mathf.Deg2Rad)
			);
		}

		public void Choose (Player chosenPlayer) {
			player = chosenPlayer;
			state.ChooseAnimal(chosenPlayer, animal);

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
