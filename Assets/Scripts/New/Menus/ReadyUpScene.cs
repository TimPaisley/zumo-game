using System;
using System.Linq;
using UnityEngine;

namespace Zumo {
	class ReadyUpScene : MonoBehaviour {
        public Camera sceneCamera;
		public ReadyUpDevice baseDeviceView;

        private GameManager gm;

        void Awake() {
            gm = FindObjectOfType<GameManager>();
        }

		void Start() {
            gm.cameraManager.Use(sceneCamera);

			createControllers();
		}

		void Update() {
		}

		private void createControllers() {
			foreach (var players in gm.players.GroupBy(player => player.input)) {
				var controllerView = Instantiate(baseDeviceView);
				controllerView.Setup(players);
			}
		}
	}
}
