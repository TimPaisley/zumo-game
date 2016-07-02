using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Zumo.InputHelper;

namespace Zumo {
	class ReadyUpDevice : MonoBehaviour {
		[Header("Child objects")]
		public Text leftPlayerText;
		public Text rightPlayerText;
		public RectTransform leftPlayerUnready;
		public RectTransform rightPlayerUnready;
		public RectTransform leftPlayerReady;
		public RectTransform rightPlayerReady;

		[Header("Audio")]
		public AudioSource readyUpSound;

		private Player leftPlayer;
		private Player rightPlayer;

		private Image image;

		void Awake() {
			image = GetComponent<Image>();

            leftPlayerReady.gameObject.SetActive(false);
            rightPlayerReady.gameObject.SetActive(false);
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

        public bool bothPlayersReady {
            get { return leftPlayer.isReady && rightPlayer.isReady; }
        }

		public void Setup(IEnumerable<Player> players) {
			leftPlayer = players.First();
			rightPlayer = players.Last();

			setupPlayerText();
		}

		private void setupPlayerText() {
			leftPlayerText.text = leftPlayer.name;
			leftPlayerText.color = leftPlayer.color;
			rightPlayerText.text = rightPlayer.name;
			rightPlayerText.color = rightPlayer.color;
		}
	}
}
