using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Zumo {
    [RequireComponent(typeof(VirtualScene))]
    class SplashScene : MonoBehaviour {
        public VirtualScene nextScene;

        private VirtualScene vs;

        void Awake() {
            vs = GetComponent<VirtualScene>();
            vs.Activated += setupCamera;
        }

        void Update() {
            if (vs.gameManager.players.Any(player => player.input.confirm.isPressed)) {
                loadNextScene();
            }
        }

        private void setupCamera() {
            vs.cameraManager.Use(vs.cameraManager.splashCamera);
            vs.cameraManager.Shake(1, 5);
        }

        private void loadNextScene() {
            vs.cameraManager.StopShaking();

            nextScene.Activate();
        }
    }
}
