using System.Collections;
using System.Linq;
using UnityEngine;

namespace Zumo {
    class SplashScene : MonoBehaviour {
        public Camera sceneCamera;

        private GameManager gm;

        void Awake() {
            gm = FindObjectOfType<GameManager>();
        }

        void Start() {
            gm.options.Reset();
            gm.cameraManager.Use(sceneCamera, 0);
        }

        void Update() {
            if (gm.players.Any(player => player.input.confirm.isPressed)) {
                gm.SwitchScene(gm.readyUpScene);
            }
        }
    }
}
