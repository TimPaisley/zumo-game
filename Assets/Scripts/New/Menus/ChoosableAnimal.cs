using UnityEngine;
using UnityEngine.UI;

namespace Zumo {
	public class ChoosableAnimal : MonoBehaviour {
        const float CHOSEN_JUMP_MULTIPLIER = 300;

		public Image selectionIndicator;
		public Animal animal;

		GameState state;
        Animator animator;
        Rigidbody rigidBody;

		Vector2 basePosition;

		public Player player { get; private set; }

		public bool isChosen {
			get { return player != null; }
		}

		void Awake () {
            // Find and create the components we need
			state = FindObjectOfType<GameManager>().state;
            animator = GetComponentInChildren<Animator>();

            gameObject.CopyComponent(animal.GetComponent<BoxCollider>(), new string[] { "material", "center", "size" });
            rigidBody = gameObject.CopyComponent(animal.GetComponent<Rigidbody>(), new string[] { "mass" });

            animator.enabled = false;

            // Set up dependent objects
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

            var playerNameText = selectionIndicator.GetComponentInChildren<Text>();
            playerNameText.text = player.shortName;
            playerNameText.color = player.color;

            animator.enabled = true;
            rigidBody.AddForce(Vector3.up * rigidBody.mass * CHOSEN_JUMP_MULTIPLIER);
		}

		public void ResetChoice () {
			player = null;

			selectionIndicator.gameObject.SetActive(false);

            animator.enabled = false;
        }

		public float DistanceFrom (Vector2 point) {
			return Vector2.Distance(point, basePosition);
		}
	}
}
