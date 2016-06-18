using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Zumo.InputHelper;

namespace Zumo {
    class GameManager : MonoBehaviour {
        public VirtualScene initialScene;

        public CameraManager cameraManager { get; private set; }
        public PlayerController[] players { get; private set; }

        public IEnumerable<PlayerController> readyPlayers {
            get { return players.Where(player => player.isReady); }
        }

        void Awake() {
            cameraManager = FindObjectOfType<CameraManager>();
            setupPlayers();
        }

        void Start() {
            initialScene.Activate();
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
