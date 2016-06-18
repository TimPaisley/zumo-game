using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zumo.InputHelper;

namespace Zumo {
    class GameManager : MonoBehaviour {
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

            players = new PlayerController[ControllerInput.instances.Count() + KeyboardInput.instances.Count()];

            foreach (var input in ControllerInput.instances) {
                players[playerIndex++] = new PlayerController(playerIndex, input);
            }

            foreach (var input in KeyboardInput.instances) {
                players[playerIndex++] = new PlayerController(playerIndex, input);
            }
        }
    }
}
