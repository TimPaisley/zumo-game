﻿using System.Linq;
using UnityEngine;

namespace Zumo {
    class SplashScreen : MonoBehaviour {
        public Camera sceneCamera;

        GameManager gm;

        void Awake() {
            gm = FindObjectOfType<GameManager>();
            sceneCamera.gameObject.SetActive(false);
        }

        void Start() {
            gm.cameraManager.Use(sceneCamera, 0);
        }

        void Update() {
            if (gm.state.players.Any(player => player.input.confirm.isPressed)) {
                gm.SwitchScene(gm.readyUpScene);
            }
        }
    }
}
