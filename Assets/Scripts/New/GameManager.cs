using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zumo.InputHelper;

namespace Zumo {
    class GameManager : MonoBehaviour {
        [Header("Scenes")]
        public VirtualScene initialScene;

        public CameraManager cameraManager { get; private set; }
        public MusicManager musicManager { get; private set; }
        public PlayerController[] players { get; private set; }

        public IEnumerable<PlayerController> readyPlayers {
            get { return players.Where(player => player.isReady); }
        }

        void Awake() {
            cameraManager = FindObjectOfType<CameraManager>();
            musicManager = FindObjectOfType<MusicManager>();
            setupPlayers();
        }

        void Start() {
            initialScene.Load();
        }

        private void setupPlayers() {
            var playerIndex = 0;
            var controllerInputs = FindControllerInputs.call();
            var keyboardInputs = FindKeyboardInputs.call();

            players = new PlayerController[controllerInputs.Count() + keyboardInputs.Count()];

            foreach (var input in controllerInputs) {
                players[playerIndex++] = new PlayerController(playerIndex, input);
            }

            foreach (var input in keyboardInputs) {
                players[playerIndex++] = new PlayerController(playerIndex, input);
            }
        }
    }
}
