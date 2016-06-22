using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Zumo.InputHelper;

namespace Zumo {
	class ReadyUpDevice : MonoBehaviour {
		[Header("Controller types")]
		public Sprite xboxController;
		public Sprite keyboardController;

		[Header("Child objects")]
		public Text leftPlayerText;
		public Text rightPlayerText;
		public RectTransform leftPlayerUnready;
		public RectTransform rightPlayerUnready;
		public RectTransform leftPlayerReady;
		public RectTransform rightPlayerReady;

		[Header("Audio")]
		public AudioSource readyUpSound;

		private PlayerController leftPlayer;
		private PlayerController rightPlayer;

		private Image image;

		void Start() {
			image = GetComponent<Image>();
		}

		void Update() {
			if (!leftPlayer.isReady && leftPlayer.input.dash.isPressed) {
				leftPlayer.isReady = true;
				leftPlayerUnready.gameObject.SetActive(false);
				leftPlayerReady.gameObject.SetActive(true);
				readyUpSound.Play();
			}

			if (!rightPlayer.isReady && rightPlayer.input.dash.isPressed) {
				rightPlayer.isReady = true;
				rightPlayerUnready.gameObject.SetActive(false);
				rightPlayerReady.gameObject.SetActive(true);
				readyUpSound.Play();
			}
		}

		public void Setup(IEnumerable<PlayerController> players) {
			leftPlayer = players.First();
			rightPlayer = players.Last();

			setupPlayerText();
			showControllerType(leftPlayer.input);
		}

		private void setupPlayerText() {
			leftPlayerText.text = leftPlayer.name;
			leftPlayerText.color = leftPlayer.color;
			rightPlayerText.text = rightPlayer.name;
			rightPlayerText.color = rightPlayer.color;
		}

		private void showControllerType(InputMap input) {
			if (input.inputType == InputType.XboxController) {
				image.sprite = xboxController;
				leftPlayerUnready.GetComponentInChildren<Text>().text = "LT";
				rightPlayerUnready.GetComponentInChildren<Text>().text = "RT";
			} else if (input.inputType == InputType.Keyboard) {
				image.sprite = keyboardController;
				leftPlayerUnready.GetComponentInChildren<Text>().text = "Space";
				rightPlayerUnready.GetComponentInChildren<Text>().text = "RShift";
			}
		}
	}
}
