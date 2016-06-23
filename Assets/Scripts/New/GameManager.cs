using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zumo.InputHelper;

namespace Zumo {
    class GameManager : MonoBehaviour {
        [Header("Menus")]
        public SceneLoader splashScene;
        public SceneLoader readyUpScene;
        public SceneLoader characterChoiceScene;
        public SceneLoader boardChoiceScene;

        [Header("Game Modes")]
        public SceneLoader deathmatchScene;

        public CameraManager cameraManager { get; private set; }
        public MusicManager musicManager { get; private set; }
        public PlayerController[] players { get; private set; }
        public GameOptions options { get; private set; }

        private SceneLoader currentScene;

        void Awake() {
            cameraManager = FindObjectOfType<CameraManager>();
            musicManager = FindObjectOfType<MusicManager>();
            players = findPlayers();
            options = new GameOptions();
        }

        void Start() {
            // Pausing for a moment with a blank screen allows physics objects in the room to come to rest
            StartCoroutine(loadInitialScene());
        }

        public IEnumerable<PlayerController> readyPlayers {
            get { return players.Where(player => player.isReady); }
        }

        public void SwitchScene(SceneLoader loader) {
            if (currentScene != null) currentScene.Unload();

            currentScene = loader;
            currentScene.Load();
        }

        private IEnumerator loadInitialScene() {
            yield return new WaitForSeconds(0.2f);

            SwitchScene(splashScene);
        }

        private PlayerController[] findPlayers() {
            var playerIndex = 0;
            var controllerInputs = FindControllerInputs.call();
            var keyboardInputs = FindKeyboardInputs.call();

            var players = new PlayerController[controllerInputs.Count() + keyboardInputs.Count()];

            foreach (var input in controllerInputs) {
                players[playerIndex++] = new PlayerController(playerIndex, input);
            }

            foreach (var input in keyboardInputs) {
                players[playerIndex++] = new PlayerController(playerIndex, input);
            }

            return players;
        }
    }
}
