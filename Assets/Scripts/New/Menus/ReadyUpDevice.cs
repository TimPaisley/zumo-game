using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Zumo {
	public class ReadyUpDevice : MonoBehaviour {
		[Header("Child objects")]
		public Text leftPlayerText;
		public Text rightPlayerText;
		public RectTransform leftPlayerUnready;
		public RectTransform rightPlayerUnready;
		public RectTransform leftPlayerReady;
		public RectTransform rightPlayerReady;

		[Header("Audio")]
		public AudioSource readyUpSound;

		GameState state;
		Player leftPlayer;
		Player rightPlayer;

		Image image;

		void Awake() {
			state = FindObjectOfType<GameManager>().state;
			image = GetComponent<Image>();

            leftPlayerReady.gameObject.SetActive(false);
            rightPlayerReady.gameObject.SetActive(false);
		}

		void Update() {
			if (!state.readyPlayers.Contains(leftPlayer) && leftPlayer.input.dash.isPressed) {
				state.readyPlayers.Add(leftPlayer);
				leftPlayerUnready.gameObject.SetActive(false);
				leftPlayerReady.gameObject.SetActive(true);
				readyUpSound.Play();
			}

			if (!state.readyPlayers.Contains(rightPlayer) && rightPlayer.input.dash.isPressed) {
				state.readyPlayers.Add(rightPlayer);
				rightPlayerUnready.gameObject.SetActive(false);
				rightPlayerReady.gameObject.SetActive(true);
				readyUpSound.Play();
			}
		}

        public bool bothPlayersReady {
			get { return state.readyPlayers.Contains(leftPlayer) && state.readyPlayers.Contains(rightPlayer); }
        }

		public void Setup(IEnumerable<Player> players) {
			leftPlayer = players.First();
			rightPlayer = players.Last();

			leftPlayerText.text = leftPlayer.name;
			leftPlayerText.color = leftPlayer.color;
			rightPlayerText.text = rightPlayer.name;
			rightPlayerText.color = rightPlayer.color;
		}
	}
}
