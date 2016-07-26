using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zumo.InputHelper;
using UnityEngine.SceneManagement;

namespace Zumo {
    public class GameManager : MonoBehaviour {
		public float bounceForce = 1f;
        public float blankScreenTime = 0.3f;

        [Header("Menus")]
        public string splashScene;
        public string readyUpScene;
        public string animalChoiceScene;
        public string boardChoiceScene;

        [Header("Game Modes")]
        public string deathmatchScene;

        public CameraManager cameraManager { get; private set; }
        public MusicManager musicManager { get; private set; }
        
		public GameState state { get; private set; }

        string currentScene;

        void Awake () {
            cameraManager = FindObjectOfType<CameraManager>();
            musicManager = FindObjectOfType<MusicManager>();
			state = new GameState(findPlayers());
        }

        void Start () {
            ensureCorrectSceneSetup();

            // Pausing for a moment with a blank screen allows physics objects in the room to come to rest
            StartCoroutine(loadInitialScene());
        }

		public void SwitchScene (string name) {
			if (currentScene != null) {
				SceneManager.UnloadScene(currentScene);
			}

            currentScene = name;
			SceneManager.LoadScene(currentScene, LoadSceneMode.Additive);
        }

        void ensureCorrectSceneSetup () {
            var persistentSceneName = gameObject.scene.name;
            var scenesToUnload = new List<Scene>(SceneManager.sceneCount);

            // Two-step process here because sceneCount changes when a scene is unloaded

            for (var i = 0; i < SceneManager.sceneCount; i++) {
                if (SceneManager.GetSceneAt(i).name != persistentSceneName) {
                    scenesToUnload.Add(SceneManager.GetSceneAt(i));
                }
            }

            foreach (var scene in scenesToUnload) {
                SceneManager.UnloadScene(scene.buildIndex);
            }
        }

        IEnumerator loadInitialScene () {
            yield return new WaitForSeconds(blankScreenTime);

            SwitchScene(splashScene);
        }

        Player[] findPlayers () {
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
