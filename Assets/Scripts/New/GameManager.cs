using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zumo.InputHelper;
using UnityEngine.SceneManagement;

namespace Zumo {
    class GameManager : MonoBehaviour {
        [Header("Menus")]
        public string splashScene;
        public string readyUpScene;
        public string animalChoiceScene;
        public string boardChoiceScene;

        [Header("Game Modes")]
        public string deathmatchScene;

        public CameraManager cameraManager { get; private set; }
        public MusicManager musicManager { get; private set; }
        public Player[] players { get; private set; }

        string currentScene;

        void Awake() {
            cameraManager = FindObjectOfType<CameraManager>();
            musicManager = FindObjectOfType<MusicManager>();
            players = findPlayers();
        }

        void Start() {
            // Pausing for a moment with a blank screen allows physics objects in the room to come to rest
            StartCoroutine(loadInitialScene());
        }

        public IEnumerable<Player> readyPlayers {
            get { return players.Where(player => player.isReady); }
        }

		public void SwitchScene(string name) {
			if (currentScene != null) {
				SceneManager.UnloadScene(currentScene);
			}

            currentScene = name;
			SceneManager.LoadScene(currentScene, LoadSceneMode.Additive);
        }

        IEnumerator loadInitialScene() {
            yield return new WaitForSeconds(0.2f);

            SwitchScene(splashScene);
        }

        Player[] findPlayers() {
            var playerIndex = 0;
            var controllerInputs = FindControllerInputs.call();
            var keyboardInputs = FindKeyboardInputs.call();

            var players = new Player[controllerInputs.Count() + keyboardInputs.Count()];

            foreach (var input in controllerInputs) {
                players[playerIndex++] = new Player(playerIndex, input);
            }

            foreach (var input in keyboardInputs) {
                players[playerIndex++] = new Player(playerIndex, input);
            }

            return players;
        }
    }
}
